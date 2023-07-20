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
    private float targetDistanceInitial;
    private float targetDistance;
    private float maxTargetOffset = 3;

    [SerializeField]
    private float initialSpeed;
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
        speed = initialSpeed; 
        rb = GetComponent<Rigidbody2D>();   
        player = GameManager.Instance.GetPlayerReference();
        damagable = GetComponentInParent<IDamagable>();
        damagable.OnDeath += Damagable_OnDeath;

        targetDistance = UnityEngine.Random.Range(targetDistanceInitial - maxTargetOffset, targetDistanceInitial);
    }

    private void OnDestroy()
    {
        if (damagable != null)
            damagable.OnDeath -= Damagable_OnDeath;
    }

    public void SetSpeed(float speed)
    {
        this.initialSpeed = speed;
    }

    private void Damagable_OnDeath()
    {
        isAlive = false;
        speed = 0;
        cdAfterAttack = 999f;
        rb.velocity = Vector3.zero;
    }

    private void OnEnable()
    {
        isAlive = true;
        cdAfterAttack = 0f;
        speed = initialSpeed;
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
