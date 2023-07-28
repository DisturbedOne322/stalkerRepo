using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    [SerializeField]
    private Transform destination;

    [SerializeField]
    private Camera camera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();
            Vector3 point = new Vector3(destination.position.x, player.transform.position.y, 0);

            player.transform.position = point;
            camera.transform.position = point;
        }
    }
}
