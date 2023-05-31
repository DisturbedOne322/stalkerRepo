using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FlameballFallingState : FlameballBaseState
{
    private int damage = 1;
    private float puddleOffsetFromGround = 1.65f;
    public override void EnterState(Flameball manager)
    {
        //random spawn;
        //idea is to have a separate stage for boss fight with a new camera. Flameball spawns in this stage close to player
    }

    public override void UpdateState(Flameball manager)
    {
        manager.FallDown();
    }



    public override void OnTriggerStay2D(Flameball manager, Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.GetPlayerReference().GetDamaged(damage);
            manager.DestroySelf();
        }
        else
        {
            //spawn on the ground that was hit
            manager.transform.position = new Vector2(manager.transform.position.x, collision.gameObject.transform.position.y + puddleOffsetFromGround);
            manager.SwitchState(manager.puddleState);
        }
    }
}
