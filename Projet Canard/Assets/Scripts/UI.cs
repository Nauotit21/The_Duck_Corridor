using UnityEngine;

public class LazyFollowUI : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float distance = 1.5f;
    [SerializeField] private float height = -0.1f;
    [SerializeField] private float followSpeed = 2f;
    [SerializeField] private float angleTrigger = 30f;

    private Quaternion _targetRotation;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        Vector3 targetPos = cameraTransform.position
                          + cameraTransform.forward * distance
                          + Vector3.up * height;

        float angle = Vector3.Angle(
            transform.position - cameraTransform.position,
            cameraTransform.forward);

        
        if (angle > angleTrigger)
        {
            transform.position = Vector3.Lerp(
                transform.position, targetPos,
                Time.deltaTime * followSpeed);
        }

        
        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180, 0);
    }
}