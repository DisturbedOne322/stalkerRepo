using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField]
    private Transform pointingDirection;

    private PlayerMovement player;

    public static float PlayerScale;

    private bool isAlive = true;

    private void Start()
    {
        player = GetComponent<PlayerMovement>();
        player.OnPlayerDied += Player_OnPlayerDied;
    }

    private void OnDestroy()
    {
        player.OnPlayerDied -= Player_OnPlayerDied;
    }

    private void Player_OnPlayerDied()
    {
        isAlive = false;
    }

    private void Update()
    {
        if (!isAlive)
            return;
        if(pointingDirection.position.x > transform.position.x)
        {
            var scale = transform.localScale;
            scale.x = 1;
            transform.localScale = scale;
        }
        if(pointingDirection.position.x < transform.position.x)
        {
            var scale = transform.localScale;
            scale.x = -1;
            transform.localScale = scale;
        }
        PlayerScale = transform.localScale.x;
    }
}
