using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInRange : MonoBehaviour
{
    public event Action<PlayerMovement> OnPlayerInRange;
    [SerializeField]
    private Transform leftDetectionPoint;
    [SerializeField] 
    private Transform rightDetectionPoint;

    private bool foundPlayer = false;

    [SerializeField]
    private LayerMask playerLayerMask;

    HellHound hellHound;

    private void FixedUpdate()
    {
        FindPlayerInRange();
    }

    private void Start()
    {
        hellHound = gameObject.GetComponentInParent<HellHound>();
        hellHound.OnDamageTaken += HellHound_OnDamageTaken;
    }

    private void OnDestroy()
    {
        hellHound.OnDamageTaken -= HellHound_OnDamageTaken;
    }

    private void HellHound_OnDamageTaken()
    {
        if (!foundPlayer)
        {
            OnPlayerInRange?.Invoke(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>());
            foundPlayer = true;
        }
    }

    private void FindPlayerInRange()
    { 
        Collider2D collider = Physics2D.OverlapArea(leftDetectionPoint.position, rightDetectionPoint.position, playerLayerMask);

        if (collider != null && !foundPlayer) 
        {
            foundPlayer = true;
            OnPlayerInRange?.Invoke(collider.gameObject.GetComponent<PlayerMovement>());
        }
    }
}
