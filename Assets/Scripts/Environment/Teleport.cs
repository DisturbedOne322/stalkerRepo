using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public static event Action<Vector3> OnTeleportedPlayer;
    [SerializeField]
    private Transform destination;

    [SerializeField]
    private Camera camera;

    [SerializeField]
    private GameObject nextArea;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement player = collision.gameObject.GetComponent<PlayerMovement>();

            Vector3 initialPos = player.transform.position;

            float yOffset = destination.position.y - transform.position.y;

            Vector3 point = new Vector3(destination.position.x, player.transform.position.y + yOffset, 0);
            player.transform.position = point;
            camera.transform.position = point;

            Vector3 delta = point - initialPos;
            OnTeleportedPlayer?.Invoke(delta);
            


            if (nextArea != null)
                nextArea.SetActive(true);
            gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}
