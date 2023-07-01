using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private PlayerMovement player;

    private Transform lookTarget;

    [SerializeField]
    private Transform objectToRotate;

    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        lookTarget = player.transform;
    }

    private void LateUpdate()
    {
        int mult = 1;
        mult = lookTarget.position.x > transform.position.x ? 1 : -1;

        objectToRotate.right = (lookTarget.position - transform.position) * mult;
        //head.Rotate(0,0,1);
    }
}
