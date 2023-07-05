using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class ApproachPlayer : MonoBehaviour
{
    private PlayerMovement player;
    private Rigidbody2D rb;

    public event Action<bool> OnPlayerInRange;

    //approach player until this distance
    [SerializeField]
    private float targetDistance;

    [SerializeField]
    private float speed;

    private bool isAlive = true;

    private IDamagable damagable;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
        player = GameManager.Instance.GetPlayerReference();
        damagable = GetComponentInParent<IDamagable>();
        damagable.OnDeath += Damagable_OnDeath;
    }

    private void Damagable_OnDeath()
    {
        isAlive = false;
    }

    private void FixedUpdate()
    {
        if(!isAlive)
        {
            return;
        }

        Vector2 vectorToPlayer = (player.transform.position - transform.position).normalized;
        bool playerInRange = Vector2.Distance(player.transform.position, transform.position) < targetDistance;

        if (!playerInRange)
            rb.AddForce(vectorToPlayer * speed);

        OnPlayerInRange?.Invoke(playerInRange);
    }
}
