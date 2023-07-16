using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDeathArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerMovement>().GetDamaged(99999);
            return;
        }

        //collision.gameObject.SetActive(false);
    }
}
