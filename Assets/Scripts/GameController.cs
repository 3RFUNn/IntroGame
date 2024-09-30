using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public List<GameObject> pickups; 
    public float detectionRadius = 5f; 

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

      
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        GameObject closestPickup = FindClosestPickup();
        HighlightPickups(closestPickup);
    }

    private GameObject FindClosestPickup()
    {
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject pickup in pickups)
        {
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
                    renderer.material.color = Color.blue; 
                }
                else
                {
                    renderer.material.color = Color.white;
                }
            }
        }
    }
}
