using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private Vector2 targetPosition;
    private Rigidbody2D rb;
    private Vector3 position;
    public TileBase tile;
    public Tilemap tileMap;

    public float movementSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            GotoClickPoint();

        position = Vector2.MoveTowards(transform.position, targetPosition, movementSpeed * Time.deltaTime);
        // InputUpdate();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(position);
    }

    private void GotoClickPoint()
    {
        Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mPos.z = 0;

        FindWaypoint(mPos);

        //tile = tileMap.GetTile(targetPosition);
    }

    private Vector2 org;

    private void FindWaypoint(Vector2 pos)
    {
        GameObject node = null;
        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            org = wp.transform.position;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, wp.transform.position - transform.position);
            if (hit)
            {
            }
            else
            {
                Debug.Log(wp);
                if (node == null || Vector3.Distance(pos, wp.transform.position) < Vector3.Distance(node.transform.position, pos))
                    node = wp;
            }
        }

        if (node != null)
            targetPosition = node.transform.position;
    }

    private void InputUpdate()
    {
        float isoAngle = 0.16f;
        float pos1 = 0.0f, pos2 = 0.0f;
        float xPos = 0.5f, yPos = 0.5f;
        if (Input.GetAxis("Vertical") > 0)
        {
            pos1 = xPos + isoAngle;
            pos2 = yPos - isoAngle;
        }

        if (Input.GetAxis("Vertical") < 0)
        {
            pos1 = -xPos - isoAngle;
            pos2 = -yPos + isoAngle;
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            pos1 = xPos + isoAngle;
            pos2 = -yPos + isoAngle;
        }

        if (Input.GetAxis("Horizontal") < 0)
        {
            pos1 = -xPos - isoAngle;
            pos2 = yPos - isoAngle;
        }

        Vector2 direction = new Vector3(pos1, pos2);
    }

    private void OnDrawGizmos()
    {
        // Gizmos.DrawSphere(targetPosition, 1f);
        //Gizmos.DrawSphere(position, 0.1f);
        // Gizmos.DrawLine(transform.position, org);
        // Gizmos.DrawRay(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
}