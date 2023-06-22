using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField]
    private Transform pointingDirection;

    public static float PlayerScale;

    private void Update()
    {
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
