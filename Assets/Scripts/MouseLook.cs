using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MouseLook
{
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool invertedMouse;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;

    public void MouseUpdate(Transform player, Transform head)
    {
        m_CharacterTargetRot = player.localRotation;
        m_CameraTargetRot = head.localRotation;

        float yRot = Input.GetAxis("Mouse X") * XSensitivity;
        float xRot = Input.GetAxis("Mouse Y") * YSensitivity;
        if (invertedMouse)
            xRot = -xRot;

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

        player.localRotation = m_CharacterTargetRot;
        head.localRotation = m_CameraTargetRot;
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, -90, 90);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}