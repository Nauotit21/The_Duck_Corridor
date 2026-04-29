using System.Collections;
using UnityEngine;

public class TelephoneAleatoire : MonoBehaviour
{
    [Header("Lumière de l'écran")]
    public Light lumiereEcran;

    [Header("Son du téléphone")]
    public AudioClip sonTelephone;
    private AudioSource audioSource;

    [Header("Durée d'allumage")]
    public float dureeAllumage = 3f;

    [Header("Intervalle aléatoire")]
    public float intervalleMin = 5f;
    public float intervalleMax = 15f;

    void Start()
    {
        lumiereEcran.enabled = false;

        // Ajoute automatiquement un AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; // 3D (le son vient du téléphone)
        audioSource.clip = sonTelephone;

        StartCoroutine(CycleAleatoire());
    }

    IEnumerator CycleAleatoire()
    {
        while (true)
        {
            float attente = Random.Range(intervalleMin, intervalleMax);
            yield return new WaitForSeconds(attente);

            // Allumer + jouer le son
            lumiereEcran.enabled = true;
            audioSource.Play();

            yield return new WaitForSeconds(dureeAllumage);

            // Éteindre
            lumiereEcran.enabled = false;
        }
    }
}