using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private float maxHealth = 10f;
    public float MaxHealth { get => maxHealth; }

    [Header("Sounds")]
    [SerializeField] private AudioClip hurtSound;

    public float Health { get; private set; }

    public UnityEvent OnDeath { get; private set; }
    public UnityEvent OnHealthChanged { get; private set; }

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        OnDeath = new UnityEvent();
        OnHealthChanged = new UnityEvent();
    }

    private void Start()
    {
        Health = maxHealth;
        HUD.CreateHealthbar(this);
    }

    public void TakeDamage(float damage)
    {
        Health = Mathf.Max(Health - damage, 0f);
        audioSource.PlayOneShot(hurtSound);
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
