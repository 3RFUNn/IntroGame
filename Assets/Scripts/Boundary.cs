using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Destroy the game object that collides with the boundary
        Destroy(other.gameObject);
    }
}