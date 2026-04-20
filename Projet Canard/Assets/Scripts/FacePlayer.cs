using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _cam;

    void Start()
    {
        _cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        // L'objet regarde dans la même direction que la caméra (pas vers elle)
        transform.rotation = Quaternion.LookRotation(transform.position - _cam.position);
    }
}
