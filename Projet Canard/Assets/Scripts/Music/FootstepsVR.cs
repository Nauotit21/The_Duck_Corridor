using UnityEngine;

public class FootstepsVR : MonoBehaviour
{
    [Header("Joueur")]
    public Transform player; // XR Origin ou Camera

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip footstepSound;

    [Header("Réglage")]
    public float seuil = 0.01f;

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = player.position;

        audioSource.clip = footstepSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, lastPosition);

        if (distance > seuil)
        {
            // 🎧 Le joueur bouge
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            // 🔇 Le joueur ne bouge pas
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        lastPosition = player.position;
    }
}
