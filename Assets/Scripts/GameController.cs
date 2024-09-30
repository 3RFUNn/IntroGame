using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public List<GameObject> pickups; // List of pickups to track
    public float detectionRadius = 5f; // Radius to check for closest pickup

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Set the line renderer properties
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // Initialize the line renderer positions
        lineRenderer.positionCount = 2; // Assuming you want two positions
        lineRenderer.enabled = false; // Start with line disabled
    }

    private void Update()
    {
        GameObject closestPickup = FindClosestPickup();
        HighlightPickups(closestPickup);
        UpdateLineRenderer(closestPickup);
    }

    private GameObject FindClosestPickup()
    {
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject pickup in pickups)
        {
            if (!pickup.activeInHierarchy) // Check if pickup is active
                continue;

            float distance = Vector3.Distance(transform.position, pickup.transform.position);
            if (distance < closestDistance && distance <= detectionRadius)
            {
                closestDistance = distance;
                closest = pickup;
            }
        }

        return closest;
    }

    private void HighlightPickups(GameObject closestPickup)
    {
        foreach (GameObject pickup in pickups)
        {
            Renderer renderer = pickup.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (pickup == closestPickup)
                {
                    renderer.material.color = Color.blue; // Change color to blue if closest
                }
                else
                {
                    renderer.material.color = Color.white; // Change color to white otherwise
                }
            }
        }
    }

    private void UpdateLineRenderer(GameObject closestPickup)
    {
        if (closestPickup != null && closestPickup.activeInHierarchy) // Check if closest pickup is active
        {
            lineRenderer.SetPosition(0, transform.position); // Start position (player)
            lineRenderer.SetPosition(1, closestPickup.transform.position); // End position (closest pickup)
            lineRenderer.enabled = true; // Enable line rendering
        }
        else
        {
            lineRenderer.enabled = false; // Disable line rendering if no valid pickup
        }
    }
}
