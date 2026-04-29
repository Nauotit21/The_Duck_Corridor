using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChangeurMusique : MonoBehaviour
{
    [Header("Musiques")]
    public AudioClip[] musiques;
    private int indexActuel = 0;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("Flashlight")]
    public Light flashlight;              // Ta lampe
    public Color[] couleurs;              // Couleurs associées

    [Header("Cooldown entre chaque presse")]
    public float cooldown = 1f;
    private bool peutChanger = true;

    void Start()
    {
        if (musiques.Length > 0)
        {
            audioSource.clip = musiques[0];
            audioSource.Play();
        }

        // Initialiser couleur
        if (flashlight != null && couleurs.Length > 0)
        {
            flashlight.color = couleurs[0];
        }

        var interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        interactable.activated.AddListener(OnBoutonPresse);
    }

    void OnBoutonPresse(ActivateEventArgs args)
    {
        Debug.Log("Bouton pressé !");
        if (!peutChanger) return;
        StartCoroutine(ChangerMusique());
    }

    IEnumerator ChangerMusique()
    {
        peutChanger = false;

        // Passe à la musique suivante
        indexActuel = (indexActuel + 1) % musiques.Length;

        // Change musique
        audioSource.clip = musiques[indexActuel];
        audioSource.Play();

        // 🔥 Change couleur de la lampe
        if (flashlight != null && couleurs.Length > 0)
        {
            flashlight.color = couleurs[indexActuel % couleurs.Length];
        }

        yield return new WaitForSeconds(cooldown);
        peutChanger = true;
    }
}