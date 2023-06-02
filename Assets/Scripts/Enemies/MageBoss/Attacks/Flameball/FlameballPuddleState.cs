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

    private float lifeTimeTotal = 5;
    private float lifeTimeLeft = 5;
    private SpriteRenderer spriteRenderer;

    public override void EnterState(Flameball manager)
    {
        manager.animator.SetBool(PUDDLE_ANIM, true);
        manager.UpdateCollider();
        spriteRenderer = manager.spriteRenderer;
    }

    public override void UpdateState(Flameball manager)
    {
        lifeTimeLeft -= Time.deltaTime;
        Color currentAlpha = spriteRenderer.color;
        currentAlpha.a = Mathf.Min((lifeTimeLeft/lifeTimeTotal) * 2, 1);
        spriteRenderer.color = currentAlpha;
        if(lifeTimeLeft < 0)
        {
            manager.DestroySelf();
        }

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
