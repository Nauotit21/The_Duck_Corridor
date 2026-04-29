using UnityEngine;
using UnityEngine.Events;

public class DuckManager : MonoBehaviour
{
    public static DuckManager Instance { get; private set; }

    [Header("Paramètres")]
    [SerializeField] private int ducksRequiredToWin = 5;

    [Header("Événements")]
    public UnityEvent<int> onDuckCollected;   // passe le nouveau total
    public UnityEvent onVictory;

    private int _collectedCount = 0;
    public int CollectedCount => _collectedCount;
    public int DucksRequired  => ducksRequiredToWin;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>Appelé par DuckCollectible quand un canard est saisi.</summary>
    public void RegisterDuckCollected()
    {
        if (_collectedCount >= ducksRequiredToWin) return;

        _collectedCount++;
        onDuckCollected?.Invoke(_collectedCount);

        Debug.Log($"[DuckManager] Canards collectés : {_collectedCount}/{ducksRequiredToWin}");

        if (_collectedCount >= ducksRequiredToWin)
            TriggerVictory();
    }

    private void TriggerVictory()
    {
        Debug.Log("[DuckManager] VICTOIRE !");
        onVictory?.Invoke();
    }
}
