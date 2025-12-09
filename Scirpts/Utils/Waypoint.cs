using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private Color color;
    private float sphereSize = 0.3f;
    public List<Transform> Waypoints => waypoints;
    void Start()
    {
        SetWaypoints();
    }

    void SetWaypoints()
    {
        foreach (Transform waypoint in transform)
        {
            waypoints.Add(waypoint);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        foreach (Transform waypoint in transform)
        {
            Gizmos.DrawSphere(waypoint.position, sphereSize);
        }
        for (int i = 0; i < transform.childCount- 1; i++)
        {
            Transform start = transform.GetChild(i);
            Transform end = transform.GetChild(i+1);
            Gizmos.DrawLine(start.position, end.position);
        }
    }
}
