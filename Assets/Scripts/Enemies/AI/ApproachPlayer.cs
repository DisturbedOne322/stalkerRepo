using System;
using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();   
        player = GameManager.Instance.GetPlayerReference();
    }

    private void FixedUpdate()
    {
        Vector2 vectorToPlayer = (player.transform.position - transform.position).normalized;
        bool playerInRange = Vector2.Distance(player.transform.position, transform.position) < targetDistance;

        if (!playerInRange)
            rb.AddForce(vectorToPlayer * speed);

        OnPlayerInRange?.Invoke(playerInRange);
    }
}
