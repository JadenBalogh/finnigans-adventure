using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float groundedDist = 0.2f;
    [SerializeField] private LayerMask groundMask;

    [Header("Combat")]
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private LayerMask enemyMask;

    private bool isGrounded = true;
    private bool canAttack = true;
    private WaitForSeconds attackWait;

    private new Rigidbody2D rigidbody2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        attackWait = new WaitForSeconds(attackCooldown);
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

        if (canAttack && Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(_AttackCooldown());

            RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.right, attackRange, enemyMask);
            if (hit && hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.TakeDamage(1f);
                Instantiate(hitEffectPrefab, hit.point, Quaternion.identity);
            }
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.BoxCast(transform.position, Vector2.one, 0, Vector2.down, groundedDist, groundMask);
    }

    private IEnumerator _AttackCooldown()
    {
        canAttack = false;
        yield return attackWait;
        canAttack = true;
    }
}
