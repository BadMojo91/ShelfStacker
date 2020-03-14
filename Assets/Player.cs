using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Player stuff
    public GameObject player;

    public MouseLook mLook;

    //Movement
    public float walkSpeed, runSpeed, jumpForce, gravity, maxHeight;

    private float oldHeight, oldForce;
    private bool isJumping;
    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        mLook.MouseUpdate(transform, Camera.main.transform);

        //movement
        MoveUpdate();

        if (Input.GetButtonDown("Fire1"))
            Use();
    }

    private void Use()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.TransformDirection(Vector3.forward), out hit))
        {
            Debug.Log(hit.collider);
            if (hit.collider.GetComponent<Fixture>())
            {
                Debug.Log("eh");
                hit.collider.GetComponent<Fixture>().RemoveItem(transform);
            }
        }
    }

    private void MoveUpdate()
    {
        Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        dir = transform.TransformDirection(dir);
        //dir *= Input.GetButton("Run") ? runSpeed : walkSpeed;
        dir *= walkSpeed;
        //====== jump  ========
        if (controller.isGrounded && Input.GetButton("Jump"))
        {
            oldHeight = maxHeight; //get current height plus max height
            isJumping = true;
        }
        oldHeight -= Time.deltaTime;
        if (isJumping && oldHeight > 0)
        {
            oldForce = jumpForce;
        }
        if (isJumping && transform.position.y >= oldHeight)
        {
            isJumping = false;
        }
        else if (isJumping && Physics.CheckSphere(transform.position + Vector3.up, 1f, 1, QueryTriggerInteraction.Ignore))
        {
            isJumping = false;
            oldForce = 0;
        }

        if (!isJumping && oldForce > 0f)
        {
            oldForce -= Time.deltaTime;
        }
        //=====================

        oldForce -= gravity * Time.deltaTime;
        dir.y = oldForce;
        controller.Move(dir * Time.deltaTime);
    }
}