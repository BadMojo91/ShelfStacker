using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public List<GameObject> waypoints = new List<GameObject>();

    private void Start()
    {
        FindOtherWaypointsWithoutObstruction();
    }

    private void FindOtherWaypointsWithoutObstruction()
    {
        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            if (Physics2D.Raycast(transform.position, wp.transform.position - transform.position))
                return;
            else
                waypoints.Add(wp);
        }
    }

    private void Update()
    {
        DebugUpdate();
    }

    private int tempcounter = 0;
    private float temptimer = 0;

    private void DebugUpdate()
    {
        if (waypoints != null || waypoints.Count > 0)
        {
            temptimer += Time.deltaTime;
            if (temptimer > 2)
            {
                tempcounter++;

                temptimer = 0;
            }

            if (tempcounter >= waypoints.Count || tempcounter < 0)
                tempcounter = 0;
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints.Count > 0)
            Gizmos.DrawRay(transform.position, (waypoints[tempcounter].transform.position - transform.position));
    }
}