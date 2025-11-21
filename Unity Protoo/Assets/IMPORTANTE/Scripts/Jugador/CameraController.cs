using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("Velocidad del enfoque")]
    public float smoothSpeed = 3f;

    [Header("Distancia / ángulo del enfoque")]
    public Vector3 offset = new Vector3(0, 50, -40);

    private Transform target;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Awake()
    {
        Instance = this;

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void FocusOn(Transform newTarget)
    {
        Debug.Log("🎥 Enfocando a " + newTarget.name);
        target = newTarget;
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            target = null;
        }

        if (target == null)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                initialPosition,
                smoothSpeed * Time.deltaTime
            );


            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                initialRotation,
                smoothSpeed * Time.deltaTime
            );

            return;
        }

        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.LookAt(target);
    }
}