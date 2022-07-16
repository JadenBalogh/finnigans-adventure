using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DamageEvent", fileName = "DamageEvent", order = 51)]
public class DamageEvent : LuckEvent
{
    [SerializeField] private float minDamage = 1f;
    [SerializeField] private float maxDamage = 2f;

    public override void Invoke(Player source, Enemy target)
    {
        Debug.Log("Invoked!");
        float damage = Random.Range(minDamage, maxDamage);
        target.TakeDamage(damage);
    }
}
