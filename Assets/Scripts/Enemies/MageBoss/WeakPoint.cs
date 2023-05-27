using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : MonoBehaviour
{
    public event Action OnWeakPointBroken;
    private BoxCollider2D collider;
    private ParticleSystem particleSystem;
    int health;

    private void Awake()
    {
        health = UnityEngine.Random.Range(4, 8);
    }

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        particleSystem = transform.parent.GetComponent<ParticleSystem>();
    }

    public void GetDamage()
    {
        Debug.Log("he");

        health -= 1;
        if (health < 0)
        {
            collider.enabled = false;
            OnWeakPointBroken?.Invoke();
            particleSystem.Stop();  
        }
    }
}
