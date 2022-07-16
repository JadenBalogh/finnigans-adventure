using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float groundedDist = 0.2f;
    [SerializeField] private LayerMask groundMask;

    private bool isGrounded = true;

    private new Rigidbody2D rigidbody2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 inputX = Vector2.right * Input.GetAxis("Horizontal") * moveSpeed;
        Vector2 inputY = Vector2.up * rigidbody2D.velocity.y;

        bool isJumping = false;
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            inputY = Vector2.zero;
            isJumping = true;
        }

        rigidbody2D.velocity = inputX + inputY;

        if (isJumping)
        {
            rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.down, groundedDist, groundMask);
    }
}
