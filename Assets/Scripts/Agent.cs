using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float attackDistance = 5f;
    public float attackSpeed = 1f;

    public float speed = 0.001f;

    public const int maxHealth = 2000;
    public int health = 2000;

    private HealthBar healthBar;

    void Start()
    {
        healthBar = gameObject.GetComponentInChildren<HealthBar>();
    }

    void Update()
    {
        
    }

    public void ReceiveAttack(int damage)
    {
        health = Mathf.Max(health - damage, 0);
        healthBar.UpdateHeath(health, maxHealth);
    }

    public bool IsDead()
    {
        return health == 0;
    }
}
