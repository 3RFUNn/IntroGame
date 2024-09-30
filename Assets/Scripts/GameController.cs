using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private GameObject pickup;

    private void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
    }
#
    lineRenderer.SetPosition(0 , startPosition);
    lineRenderer.SetPosition(1 , endPosition);
    lineRenderer.startWidth = 0.1f;
    lineRenderer.endWidth = 0.1f;
}
