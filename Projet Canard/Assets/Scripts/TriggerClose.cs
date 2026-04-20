using UnityEngine;

public class TriggerClose : MonoBehaviour
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
