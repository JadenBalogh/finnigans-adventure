using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float followDist = 6f;
    [SerializeField] private float yDeathThreshold = -5f;

    [Header("Combat")]
    [SerializeField] private float maxHealth = 10f;
    public float MaxHealth { get => maxHealth; }

    [Header("Animation")]
    [SerializeField] private Animation2D idleAnim;
    [SerializeField] private Animation2D moveAnim;
    [SerializeField] private Animation2D hurtAnim;

    [Header("Sounds")]
    [SerializeField] private AudioClip hurtSound;

    public float Health { get; private set; }

    public UnityEvent OnDeath { get; private set; }
    public UnityEvent OnHealthChanged { get; private set; }

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
        OnDeath = new UnityEvent();
        OnHealthChanged = new UnityEvent();
    }

    private void Start()
    {
        Health = maxHealth;
        HUD.CreateHealthbar(this);
    }

    private void Update()
    {
        if (transform.position.y <= yDeathThreshold)
        {
            Destroy(gameObject);
        }

        Vector2 playerOffset = GameManager.Player.transform.position - transform.position;

        if (playerOffset.sqrMagnitude < followDist * followDist)
        {
            Vector2 moveDir = Vector2.right * Mathf.Sign(playerOffset.x);
            rigidbody2D.velocity = moveDir * moveSpeed + Vector2.up * rigidbody2D.velocity.y;
            animator2D.Play(moveAnim, true);
            spriteRenderer.flipX = rigidbody2D.velocity.x > 0;
        }
        else
        {
            rigidbody2D.velocity = Vector2.zero;
            animator2D.Play(idleAnim, true);
        }
    }

    public void TakeDamage(float damage)
    {
        Health = Mathf.Max(Health - damage, 0f);
        audioSource.PlayOneShot(hurtSound);
        animator2D.Play(hurtAnim, false, true);
        OnHealthChanged.Invoke();
        if (Health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath.Invoke();
        Destroy(gameObject);
    }
}
