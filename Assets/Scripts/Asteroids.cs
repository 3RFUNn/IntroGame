using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroids : MonoBehaviour
{
    public GameObject asteroidPrefab;
    public float spawnRadius = 4f;
   
    public float spawnInterval = 4f; // Interval in seconds

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnAsteroid", 0f, spawnInterval);
    }

    void SpawnAsteroid()
    {
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),0f,15f);
        Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
    }
}
