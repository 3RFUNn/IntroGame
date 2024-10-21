using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bolt") && gameObject.CompareTag("Asteroid"))
        {
            
            Destroy(collision.gameObject); // Destroy the bolt
            Destroy(gameObject); // Destroy the asteroid
        }
    }
}
