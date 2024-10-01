using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI velocityText;
    public List<GameObject> pickups; // List of pickups to track
    public float detectionRadius = 5f; // Radius to check for closest pickup
    
    private Vector3 previousPosition;
    private Vector3 velocity;

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Set the line renderer properties
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // Initialize the line renderer positions
        lineRenderer.positionCount = 2; // Assuming you want two positions
        lineRenderer.enabled = false; // Start with line disabled

        previousPosition = transform.position;
    }

    private void Update()
    {
        GameObject closestPickup = FindClosestPickup();
        HighlightPickups(closestPickup);
        UpdateLineRenderer(closestPickup);
        var position = gameObject.transform.position;
        positionText.text = "Position :" + 
                            position.x.ToString("0.0") 
                            + "," + position.z.ToString("0.0");
        
        
        // Calculate velocity based on the difference in position over time
        var position1 = transform.position;
        velocity = (position1 - previousPosition) / Time.deltaTime;

        // Update previous position
        previousPosition = position1;

        // Optional: print the velocity for debugging
        velocityText.text = "Velocity: " + velocity.magnitude.ToString("0.0");

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
                distanceText.text = "Distance: " + closestDistance.ToString("0.0");
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
