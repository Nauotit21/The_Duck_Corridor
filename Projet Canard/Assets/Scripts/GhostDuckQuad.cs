using System.Collections;
using UnityEngine;

public class GhostDuckQuad : MonoBehaviour
{
    [Header("Apparition")]
    public GameObject duckQuad;         // L'objet enfant "Quad"
    public float minWaitTime = 5f;
    public float maxWaitTime = 15f;

    [Header("Détection Lampe Torche")]
    public Light flashlight;            // La Light de ton joueur
    public float detectionRange = 10f;
    public float detectionAngle = 25f;
    public LayerMask obstructionLayers; // Pour ne pas voir à travers les murs

    [Header("Feedback Audio")]
    public AudioSource audioSource;
    public AudioClip detectionSound;
    [Range(0f, 1f)] public float soundVolume = 3f; // Réglage du volume ici !

    [Header("Feedback Visuel")]
    public float fadeOutDuration = 0.5f;

    private bool _isPresent = false;
    private bool _isCaught = false;
    private float _timer;
    private float _nextAppearanceTime;
    private Renderer _quadRenderer;

    void Start()
    {
        // Récupération du MeshRenderer du Quad
        if (duckQuad != null)
            _quadRenderer = duckQuad.GetComponent<Renderer>();

        // Initialisation : on cache le canard au début
        if (duckQuad != null) duckQuad.SetActive(false);

        ScheduleNext();

        // Sécurité pour l'audio
        if (audioSource != null) audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (!_isPresent)
        {
            // Compte à rebours avant apparition
            _timer += Time.deltaTime;
            if (_timer >= _nextAppearanceTime)
            {
                Appear();
            }
        }
        else if (!_isCaught) // Si le canard est là et n'a pas encore été détecté
        {
            if (IsIlluminatedByFlashlight())
            {
                Caught();
            }
        }
    }

    bool IsIlluminatedByFlashlight()
    {
        if (flashlight == null || !flashlight.enabled || !flashlight.gameObject.activeInHierarchy)
            return false;

        Vector3 directionToDuck = (duckQuad.transform.position - flashlight.transform.position);
        float distance = directionToDuck.magnitude;

        // 1. Vérification distance
        if (distance > detectionRange) return false;

        // 2. Vérification angle
        float angle = Vector3.Angle(flashlight.transform.forward, directionToDuck.normalized);
        if (angle > detectionAngle) return false;

        // 3. Vérification obstacles (Raycast)
        RaycastHit hit;
        if (Physics.Raycast(flashlight.transform.position, directionToDuck.normalized, out hit, detectionRange, obstructionLayers))
        {
            if (hit.transform != duckQuad.transform && !hit.transform.IsChildOf(duckQuad.transform))
                return false;
        }

        return true;
    }

    void Appear()
    {
        _isCaught = false;
        _isPresent = true;
        duckQuad.SetActive(true);

        // Reset l'opacité à 100%
        if (_quadRenderer != null)
        {
            Color c = _quadRenderer.material.color;
            c.a = 1f;
            _quadRenderer.material.color = c;
        }
    }

    void Caught()
    {
        _isCaught = true; // Empêche de relancer le son ou la coroutine en boucle

        // Joue le son avec le volume choisi
        if (audioSource != null && detectionSound != null)
        {
            audioSource.PlayOneShot(detectionSound, soundVolume);
        }

        StartCoroutine(FadeOutAndHide());
    }

    IEnumerator FadeOutAndHide()
    {
        float elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            if (_quadRenderer != null)
            {
                Color c = _quadRenderer.material.color;
                c.a = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
                _quadRenderer.material.color = c;
            }
            yield return null;
        }

        duckQuad.SetActive(false);
        _isPresent = false;
        ScheduleNext();
    }

    void ScheduleNext()
    {
        _timer = 0f;
        _nextAppearanceTime = Random.Range(minWaitTime, maxWaitTime);
    }

    // Dessine les lignes de détection dans l'éditeur (Fenêtre Scene)
    private void OnDrawGizmos()
    {
        if (flashlight == null || duckQuad == null) return;
        Gizmos.color = _isPresent ? Color.green : Color.red;
        Gizmos.DrawLine(flashlight.transform.position, duckQuad.transform.position);
    }
}
