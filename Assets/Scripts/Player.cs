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
    [SerializeField] private LuckEventTier[] luckEventTiers;
    [SerializeField] private LayerMask enemyMask;

    [Header("Animation")]
    [SerializeField] private Animation2D idleAnim;
    [SerializeField] private Animation2D moveAnim;
    [SerializeField] private Animation2D jumpAnim;
    [SerializeField] private Animation2D attackAnim;

    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip swingSound;

    private bool isGrounded = true;
    private bool canAttack = true;
    private Vector2 facingDir = Vector2.right;
    private WaitForSeconds attackWait;

    private AudioSource audioSource;
    private Animator2D animator2D;
    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator2D = GetComponent<Animator2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        attackWait = new WaitForSeconds(attackCooldown);

        GameManager.SetPlayer(this);
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
            audioSource.PlayOneShot(jumpSound);
        }

        if (isGrounded)
        {
            animator2D.Play(inputX.x != 0 ? moveAnim : idleAnim, true);
        }
        else
        {
            animator2D.Play(jumpAnim, true);
        }

        if (inputX.x != 0)
        {
            facingDir = new Vector2((int)Mathf.Sign(inputX.x), 0);
            spriteRenderer.flipX = rigidbody2D.velocity.x < 0;
        }

        if (canAttack && Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(_AttackCooldown());
            animator2D.Play(attackAnim, false, true);
            audioSource.PlayOneShot(swingSound);

            RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one, 0, facingDir, attackRange, enemyMask);
            if (hit && hit.collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                float roll = Random.value;
                float cumulativeThreshold = 0f;
                foreach (LuckEventTier tier in luckEventTiers)
                {
                    cumulativeThreshold += tier.probability;
                    if (roll <= cumulativeThreshold)
                    {
                        Instantiate(tier.triggerEffectPrefab, hit.point, Quaternion.identity);
                        audioSource.PlayOneShot(tier.triggerSound);
                        tier.luckEvents[Random.Range(0, tier.luckEvents.Length)].Invoke(this, enemy);
                        break;
                    }
                }
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

    [System.Serializable]
    public class LuckEventTier
    {
        public float probability = 0.1f;
        public GameObject triggerEffectPrefab;
        public AudioClip triggerSound;
        public LuckEvent[] luckEvents;
    }
}
