using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    private static HUD instance;

    [SerializeField] private Healthbar healthbarPrefab;
    [SerializeField] private RectTransform healthbarParent;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public static void CreateHealthbar(Enemy enemy)
    {
        Healthbar healthbar = Instantiate(instance.healthbarPrefab, instance.healthbarParent);
        healthbar.SetTarget(enemy);
    }
}
