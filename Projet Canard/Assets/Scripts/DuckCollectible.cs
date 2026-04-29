using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class DuckCollectible : MonoBehaviour
{
    [Header("Disparition")]
    [Tooltip("Délai en secondes avant que le canard disparaisse après avoir été saisi.")]
    [SerializeField] private float disappearDelay = 2f;

    [Tooltip("Cocher pour téléporter très loin au lieu de désactiver l'objet.")]
    [SerializeField] private bool teleportInsteadOfDisable = false;
    private static readonly Vector3 FarAwayPosition = new Vector3(0f, -9999f, 0f);

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grabInteractable;
    private bool _collected = false;

    void Awake()
    {
        _grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
    }

    void OnEnable()
    {
        _grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    void OnDisable()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (_collected) return;
        _collected = true;

        // Notifie le manager
        if (DuckManager.Instance != null)
            DuckManager.Instance.RegisterDuckCollected();

        // Lance la coroutine de disparition
        StartCoroutine(DisappearAfterDelay());
    }

    private IEnumerator DisappearAfterDelay()
    {
        yield return new WaitForSeconds(disappearDelay);

        // Force le lâcher si le joueur tient encore le canard
        if (_grabInteractable.isSelected)
            _grabInteractable.interactionManager.CancelInteractableSelection(
                (UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)_grabInteractable);

        if (teleportInsteadOfDisable)
        {
            // On coupe le rigidbody puis on téléporte
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) { rb.linearVelocity = Vector3.zero; rb.isKinematic = true; }
            transform.position = FarAwayPosition;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
