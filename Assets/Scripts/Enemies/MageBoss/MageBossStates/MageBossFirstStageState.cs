using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MageBossFirstStageState : MageBossBaseState
{
    private int health;
    private PlayerMovement player;

    private float currentCD = 0f;
    private float cdBetweenAttacks = 5f;

    private Vector3 currentFlameBallPos;
    private GameObject flameball;
    private float flameballSpawnOffset;
    private float spawnCDTotal = 1f;
    private float spawnCD = 0;


    public override void EnterState(MageBoss manager)
    {
        player = manager.player;
        health = manager.collidersArray.Length;
        flameball = manager.flameball;

        flameballSpawnOffset = flameball.GetComponent<BoxCollider2D>().size.x * 3 + 0.25f;

        currentFlameBallPos = manager.flameballSpawnPos.position;

        for (int i = 0; i < manager.collidersArray.Length; i++)
        {
            manager.collidersArray[i].OnWeakPointBroken += MageBossFirstStageState_OnWeakPointBroken;
        }
    }

    private void MageBossFirstStageState_OnWeakPointBroken()
    {
        health--;
        //Update UI
    }

    private void FlameBallWaveAttack()
    {
        spawnCD -= Time.deltaTime;
        if(spawnCD < 0)
        {
            spawnCD = spawnCDTotal;
            GameObject tempFlameball = GameObject.Instantiate(flameball, currentFlameBallPos, Quaternion.identity);
            currentFlameBallPos.x -= flameballSpawnOffset;
        }
    }

    public override void UpdateState(MageBoss manager)
    {
        FlameBallWaveAttack();
    }

    public override void OnCollisionEnter(TentacleStateManager manager, Collider2D collision)
    {
        throw new System.NotImplementedException();
    }
}
