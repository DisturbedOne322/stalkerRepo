using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameballPuddleState : FlameballBaseState
{
    private const string PUDDLE_ANIM = "PuddleAnim";

    private bool canInflictDamage = true;
    private float puddleDamageCD = 0f;
    private float puddleDamageTickRate = 1f;
    private int damage = 1;

    public override void EnterState(Flameball manager)
    {
        manager.animator.SetBool(PUDDLE_ANIM, true);
        manager.UpdateCollider();
    }

    public override void UpdateState(Flameball manager)
    {
        puddleDamageCD -= Time.deltaTime;
        if(puddleDamageCD < 0)
        {
            canInflictDamage = true;
        }
    }

    public override void OnTriggerStay2D(Flameball manager, Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (canInflictDamage)
            {
                GameManager.Instance.GetPlayerReference().GetDamaged(damage);
                canInflictDamage = false;
                puddleDamageCD = puddleDamageTickRate;
            }
        }
    }
}
