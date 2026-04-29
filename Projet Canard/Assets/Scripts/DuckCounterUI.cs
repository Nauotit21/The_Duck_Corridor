using System.Collections;
using UnityEngine;
using TMPro;

public class DuckCounterUI : MonoBehaviour
{
    [Header("Compteur")]
    [SerializeField] private TextMeshProUGUI counterText;
    [Tooltip("Format du texte. {0} = collectés, {1} = requis.")]
    [SerializeField] private string counterFormat = "🦆 {0} / {1}";

    [Header("Écran de victoire")]
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private TextMeshProUGUI victoryText;
    [SerializeField] private string victoryMessage = "Bravo !\nTous les canards\nsont sauvés !";

    [Header("Animation victoire")]
    [SerializeField] private float victoryFadeInDuration = 1.2f;

    void Start()
    {
        // État initial
        if (victoryPanel != null) victoryPanel.SetActive(false);

        UpdateCounter(0);

        // Abonnement aux événements du DuckManager
        if (DuckManager.Instance != null)
        {
            DuckManager.Instance.onDuckCollected.AddListener(UpdateCounter);
            DuckManager.Instance.onVictory.AddListener(ShowVictory);
        }
        else
        {
            Debug.LogWarning("[DuckCounterUI] DuckManager introuvable dans la scène !");
        }
    }

    void OnDestroy()
    {
        if (DuckManager.Instance != null)
        {
            DuckManager.Instance.onDuckCollected.RemoveListener(UpdateCounter);
            DuckManager.Instance.onVictory.RemoveListener(ShowVictory);
        }
    }

    private void UpdateCounter(int collected)
    {
        if (counterText == null) return;

        int required = DuckManager.Instance != null ? DuckManager.Instance.DucksRequired : 5;
        counterText.text = string.Format(counterFormat, collected, required);
    }

    private void ShowVictory()
    {
        if (victoryPanel == null) return;

        if (victoryText != null)
            victoryText.text = victoryMessage;

        StartCoroutine(FadeInVictory());
    }

    private IEnumerator FadeInVictory()
    {
        victoryPanel.SetActive(true);

        // Fade-in via CanvasGroup si présent, sinon apparition directe
        CanvasGroup cg = victoryPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 0f;
            float elapsed = 0f;
            while (elapsed < victoryFadeInDuration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Clamp01(elapsed / victoryFadeInDuration);
                yield return null;
            }
            cg.alpha = 1f;
        }
    }
}
