using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 5f;
    public Vector2 xBounds = new Vector2(-5f, 5f);
    public Vector2 zBounds = new Vector2(-15f, 0f);
    private Rigidbody rb;
    private InputAction moveAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        moveAction = InputManager.Instance.moveAction;
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 inputVector = InputManager.Instance.moveAction.ReadValue<Vector2>();
        Vector3 movement = new Vector3(inputVector.x, 0, inputVector.y) * speed;
        rb.AddForce(movement);

        // Restrict the ship's position within the game area
        Vector3 clampedPosition = rb.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, xBounds.x, xBounds.y);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, zBounds.x, zBounds.y);
        rb.position = clampedPosition;

        // Add rotation for lateral movements
        float rotationValue = -inputVector.x * rotationSpeed;
        rb.rotation = Quaternion.Euler(0, 0, rotationValue);
    }
}