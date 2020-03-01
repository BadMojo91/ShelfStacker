using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 targetPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            GotoClickPoint();
    }

    private void GotoClickPoint()
    {
        Vector3 mPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mPos.z = 0;
        targetPosition = mPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(targetPosition, .1f);
    }
}