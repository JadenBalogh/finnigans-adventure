using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private float yOffset = 0.7f;
    [SerializeField] private RectTransform fillComponent;

    private Enemy target;

    private void Update()
    {
        if (target != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up * yOffset);
        }
    }

    public void SetTarget(Enemy target)
    {
        this.target = target;
        OnTargetHealthChanged();
        target.OnHealthChanged.AddListener(OnTargetHealthChanged);
        target.OnDeath.AddListener(OnTargetDeath);
    }

    private void OnTargetHealthChanged()
    {
        fillComponent.anchorMax = new Vector2(target.Health / target.MaxHealth, 1);
    }

    private void OnTargetDeath()
    {
        Destroy(gameObject);
    }
}
