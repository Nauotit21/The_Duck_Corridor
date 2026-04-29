using UnityEngine;

public class TriggerDistant : MonoBehaviour
{
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            audioSource.Play();
        }
    }
}
