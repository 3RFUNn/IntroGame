using UnityEngine;

public class AsteroidRotation : MonoBehaviour
{
    public float tumble = 5f; // Adjust the tumble factor to control the rotation speed

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.angularVelocity = Random.insideUnitSphere * tumble;
    }
}