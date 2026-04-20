using System.Collections;
using UnityEngine;

public class GhostObject : MonoBehaviour
{
    [Header("Timing")]
    public float minTimeBetweenAppearances = 1f;
    public float maxTimeBetweenAppearances = 3f;
    public float maxVisibleDuration = 3f;

    [Header("Détection regard")]
    public float detectionAngle = 15f;

    [Header("Feedback")]
    public float fadeOutDuration = 0.3f;

    [Header("Références")]
    public GameObject ghostVisual; // glisse le GhostQuad ici dans l'Inspector

    private Transform _playerCamera;
    private Renderer _renderer;
    private float _timer;
    private float _nextAppearTime;
    private bool _isVisible;
    private float _visibleSince;

    void Start()
    {
        _renderer = ghostVisual.GetComponent<Renderer>();
        _playerCamera = Camera.main.transform;

        if (_renderer == null)
            Debug.LogError("GhostObject : aucun Renderer trouvé sur ghostVisual !");
        if (_playerCamera == null)
            Debug.LogError("GhostObject : Camera.main est null !");

        ghostVisual.SetActive(false);
        ScheduleNextAppearance();
        Debug.Log($"GhostObject : démarré, prochaine apparition dans {_nextAppearTime:F1}s");
    }

    void Update()
    {
        _timer += Time.deltaTime;

        if (!_isVisible)
        {
            if (_timer >= _nextAppearTime)
                Appear();
        }
        else
        {
            if (IsPlayerLooking())
            {
                Debug.Log("GhostObject : joueur regarde → FadeOut");
                StartCoroutine(FadeOut());
                return;
            }

            if (Time.time - _visibleSince >= maxVisibleDuration)
            {
                Debug.Log("GhostObject : durée max atteinte → FadeOut");
                StartCoroutine(FadeOut());
            }
        }
    }

    bool IsPlayerLooking()
    {
        Vector3 directionToObject = (ghostVisual.transform.position - _playerCamera.position).normalized;
        float angle = Vector3.Angle(_playerCamera.forward, directionToObject);
        return angle < detectionAngle;
    }

    void Appear()
    {
        Debug.Log("GhostObject : Appear() appelé");
        ghostVisual.SetActive(true);

        Color c = _renderer.material.color;
        c.a = 1f;
        _renderer.material.color = c;

        _isVisible = true;
        _visibleSince = Time.time;
    }

    IEnumerator FadeOut()
    {
        _isVisible = false;
        float elapsed = 0f;
        Color c = _renderer.material.color;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            _renderer.material.color = c;
            yield return null;
        }

        ghostVisual.SetActive(false);
        Debug.Log("GhostObject : objet désactivé");
        ScheduleNextAppearance();
    }

    void ScheduleNextAppearance()
    {
        _timer = 0f;
        _nextAppearTime = Random.Range(minTimeBetweenAppearances, maxTimeBetweenAppearances);
        Debug.Log($"GhostObject : prochaine apparition dans {_nextAppearTime:F1}s");
    }
}
