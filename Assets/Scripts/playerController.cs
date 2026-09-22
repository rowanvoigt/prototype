using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerController : MonoBehaviour
{
    // movement variables
    public float moveSpeed = 25f;

    // jump/fall variables
    public float jumpHeight = 21.5f;
    public float jumpScaling = 0.5f;
    public float fallSpeed = 21.8f;
    private bool isGrounded;

    // gravity variables
    public Vector3 gravity = Physics.gravity;
    private Vector3 velocity;

    // player variables
    private CharacterController controller;
    private Rigidbody rb;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {

        isGrounded = controller.isGrounded;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float speed = moveSpeed;

        controller.Move(move * speed * Time.deltaTime);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity.y);
        }
        if (velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            velocity.y += gravity.y * (1 / Mathf.Clamp(jumpScaling, 0.01f, 1) - 1) * Time.deltaTime;
        }

        if (velocity.y < 0)
        {
            velocity.y += gravity.y * (fallSpeed - 1) * Time.deltaTime;
        }

        velocity.y += Physics.gravity.y * Time.deltaTime;

        Vector3 moveCalculation = move * speed + Vector3.up * velocity.y;

        controller.Move(moveCalculation * Time.deltaTime);
    }
}