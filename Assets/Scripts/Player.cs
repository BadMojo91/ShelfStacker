using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public MouseLook mouseController;

    //Public
    public Vector3 cameraPosition;

    public Vector3 crouchCameraPosition;
    public float walkSpeed;

    public float runSpeed;
    public int itemCount = 5;
    public float fillSpeed;
    public float fillVariance;

    public float gravityScale;

    //Private
    private CharacterController controller;

    private Bay currentBay;

    private bool crouch;

    public enum InteractionType
    {
        Fill, Remove
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
    }

    private void Update()
    {
        mouseController.MouseUpdate(transform, Camera.main.transform);

        //movement
        MoveUpdate();
        if (currentBay)
            if (currentBay.Filling)
                UserInterface.filling = true;
            else
                UserInterface.filling = false;

        if (crouch)
            Camera.main.transform.localPosition = crouchCameraPosition;
        else
            Camera.main.transform.localPosition = cameraPosition;
    }

    private void SendRaycast(InteractionType interaction)
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit))
        {
            if (hit.collider.GetComponent<Bay>())
            {
                currentBay = hit.collider.GetComponent<Bay>();
                switch (interaction)
                {
                    case InteractionType.Fill:
                        StartCoroutine(hit.collider.GetComponent<Bay>().Fill(itemCount, fillSpeed, fillVariance));
                        break;

                    case InteractionType.Remove:
                        hit.collider.GetComponent<Bay>().RemoveItem(transform);
                        break;
                }
                //Debug.Log("eh");
            }
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        SendRaycast(InteractionType.Fill);
    }

    public void RemoveItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        SendRaycast(InteractionType.Remove);
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            crouch = true;
        }
        if (context.canceled)
        {
            crouch = false;
        }
    }

    private void MoveUpdate()
    {
        Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        dir = transform.TransformDirection(dir);
        dir *= Input.GetButton("Run") ? runSpeed : walkSpeed;

        if (!controller.isGrounded)
            dir.y -= gravityScale;

        controller.Move(dir * Time.deltaTime);
    }
}