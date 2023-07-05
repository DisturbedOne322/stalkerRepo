using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    [SerializeField]
    private int health;
    private int maxHealth;

    public event Action<int,int> OnHealthChange;

    public event Action OnDeath;
    private void Start()
    {
        maxHealth = health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        OnHealthChange?.Invoke(health, maxHealth);
        if(health <= 0)
        {
            OnDeath?.Invoke();
        }
    }

}
