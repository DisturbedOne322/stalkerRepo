using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Checkpoints : MonoBehaviour
{
    public event Action<int> OnSpawnNextAreaEnemies;
    public int checkpointID;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            OnSpawnNextAreaEnemies?.Invoke(checkpointID + 1);
            this.enabled = false;
        }
    }
}
