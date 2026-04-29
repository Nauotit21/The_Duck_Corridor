using System.Collections;
using UnityEngine;

public class GhostDuck : MonoBehaviour
{
    [Header("Apparition")]
    public GameObject duckVisual; // L'objet qui a le SpriteRenderer
    public float minWaitTime = 5f;
    public float maxWaitTime = 15f;

    [Header("Détection Lampe")]
    public Light flashlight;
    public float detectionRange = 10f;
    public float detectionAngle = 25f;
    public LayerMask obstructionLayers;

    [Header("Feedback")]
    public AudioSource audioSource;
    public AudioClip detectionSound;
    public float fadeOutDuration = 0.5f;

    private bool _isPresent = false;
    private bool _isCaught = false; // Le "verrou" pour le son
    private float _timer;
    private float _nextAppearanceTime;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        // On récupère le SpriteRenderer au lieu du Renderer classique
        _spriteRenderer = duckVisual.GetComponentInChildren<SpriteRenderer>();

        duckVisual.SetActive(false);
        ScheduleNext();

        if (audioSource != null) audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!_isPresent)
        {
            _timer += Time.deltaTime;
            if (_timer >= _nextAppearanceTime) Appear();
        }
        else if (!_isCaught) // On ne vérifie que si on ne l'a pas déjà touché
        {
            if (IsDuckInFlashlight()) Caught();
        }
    }

    bool IsDuckInFlashlight()
    {
        if (flashlight == null || !flashlight.enabled || !flashlight.gameObject.activeInHierarchy)
            return false;

        Vector3 directionToDuck = (duckVisual.transform.position - flashlight.transform.position);
        float distance = directionToDuck.magnitude;

        if (distance > detectionRange) return false;

        float angle = Vector3.Angle(flashlight.transform.forward, directionToDuck.normalized);
        if (angle > detectionAngle) return false;

        RaycastHit hit;
        if (Physics.Raycast(flashlight.transform.position, directionToDuck.normalized, out hit, detectionRange, obstructionLayers))
        {
            if (hit.transform != duckVisual.transform && !hit.transform.IsChildOf(duckVisual.transform))
                return false;
        }

        return true;
    }

    void Appear()
    {
        _isCaught = false; // Reset le verrou
        _isPresent = true;
        duckVisual.SetActive(true);

        if (_spriteRenderer != null) {
            Color c = _spriteRenderer.color;
            c.a = 1f;
            _spriteRenderer.color = c;
        }
    }

    void Caught()
    {
        _isCaught = true; // On verrouille immédiatement pour empêcher un second appel

        if (audioSource != null && detectionSound != null)
            audioSource.PlayOneShot(detectionSound);

        StartCoroutine(FadeOutAndHide());
    }

    IEnumerator FadeOutAndHide()
    {
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            if (_spriteRenderer != null) {
                Color c = _spriteRenderer.color;
                c.a = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                _spriteRenderer.color = c;
            }
            yield return null;
        }

        duckVisual.SetActive(false);
        _isPresent = false;
        ScheduleNext();
    }

    void ScheduleNext()
    {
        _timer = 0f;
        _nextAppearanceTime = Random.Range(minWaitTime, maxWaitTime);
    }
}
