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

    [SerializeField]
    private float cdAfterAttack = 0;
    private float lastAttackTime;

    private bool inCD = false;

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

    private void OnDestroy()
    {
        damagable.OnDeath -= Damagable_OnDeath;
    }

    private void Damagable_OnDeath()
    {
        isAlive = false;
    }

    private void FixedUpdate()
    {
        if(inCD)
        {
            if(Time.time > lastAttackTime + cdAfterAttack)            
                inCD = false;            
            return;
        }

        if(!isAlive)
        {
            return;
        }

        Vector2 vectorToPlayer = (player.transform.position - transform.position).normalized;
        bool playerInRange = Vector2.Distance(player.transform.position, transform.position) < targetDistance;

        if (!playerInRange)
            rb.AddForce(vectorToPlayer * speed);
        else
        {
            rb.velocity = Vector3.zero;
            lastAttackTime = Time.time;
            inCD = true;
        }

        OnPlayerInRange?.Invoke(playerInRange);


    }
}
