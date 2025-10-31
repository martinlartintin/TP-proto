using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 35f;

    [Header("Zoom")]
    public float zoomSpeed = 100f;
    public float minZoom = 5f;
    public float maxZoom = 40f;

    [Header("Límites del mapa")]
    public float minX = -300f;
    public float maxX = 300f;
    public float minZ = -300f;
    public float maxZ = 300f;

    [Header("Posición inicial")]
    public Vector3 startPosition = new Vector3(200f, 75f, 75f);

    private Vector3 initialDirection;

    void Start()
    {
        transform.position = startPosition;

        initialDirection = new Vector3(0f, -0.7f, 0.7f).normalized;
        transform.rotation = Quaternion.LookRotation(initialDirection, Vector3.up);
    }

    void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Keyboard.current.wKey.isPressed)
            movement += new Vector3(0f, 0f, 1f);
        if (Keyboard.current.sKey.isPressed)
            movement += new Vector3(0f, 0f, -1f);
        if (Keyboard.current.aKey.isPressed)
            movement += new Vector3(-1f, 0f, 0f);
        if (Keyboard.current.dKey.isPressed)
            movement += new Vector3(1f, 0f, 0f);

        movement.Normalize();
        transform.position += movement * moveSpeed * Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(initialDirection, Vector3.up);

        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            float newHeight = transform.position.y + (scroll * zoomSpeed * Time.deltaTime * initialDirection.y);

            if (newHeight >= minZoom && newHeight <= maxZoom)
            {
                Vector3 zoomMovement = initialDirection * scroll * zoomSpeed * Time.deltaTime;
                transform.position += zoomMovement;
            }
        }

        Vector3 clampedPos = transform.position;
        clampedPos.x = Mathf.Clamp(clampedPos.x, minX, maxX);
        clampedPos.z = Mathf.Clamp(clampedPos.z, minZ, maxZ);
        transform.position = clampedPos;

        float currentHeight = transform.position.y;
        float clampedHeight = Mathf.Clamp(currentHeight, minZoom, maxZoom);
        transform.position = new Vector3(transform.position.x, clampedHeight, transform.position.z);
    }
}
