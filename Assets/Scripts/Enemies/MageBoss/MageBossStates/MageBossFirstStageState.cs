using System.Collections;
using UnityEngine;

public class MageBossFirstStageState : MageBossBaseState
{
    private int health;
    private PlayerMovement player;

    private float currentAttackCD = 7f;
    private float cdBetweenAttacks = 7f;

   // private Vector3 flameballSpawnStartPos;
    //private Vector3 currentFlameBallPos;
    //private GameObject flameball;

    private const string FLAMEBALL_ATTACK = "Flameball";
    private const string LASER_ATTACK = "Laser";
    private string lastAttack;
    private string[] attackSet = new string[2];

    //flameball
    private float spawnCDTotal = 1f; // cd between each flameball
    private float cdBetweenWaves = 2f;
    private int waveNumberTotal = 2;
    private int spawnAmountTotal = 5;
    private float fallSpeed = 10;

    //laser
    private float laserAnimationDurationTotal = 10f;
    private float laserAnimationDuration = 10f;


    private enum State
    {
        Idle,
        FlameballCast,
        LaserCast,
    }

    private State state;

    public override void EnterState(MageBoss manager)
    {
        attackSet[0] = FLAMEBALL_ATTACK;
        attackSet[1] = LASER_ATTACK;
        player = manager.player;
        health = manager.collidersArray.Length;
        //flameball = manager.flameball;
        manager.flameballspawnManager.OnAttackFinished += FlameballspawnManager_OnAttackFinished;


        for (int i = 0; i < manager.collidersArray.Length; i++)
        {
            manager.collidersArray[i].OnWeakPointBroken += MageBossFirstStageState_OnWeakPointBroken;
        }

        state = State.Idle;
    }

    private void FlameballspawnManager_OnAttackFinished()
    {
        state = State.Idle;
        SetCDBetweenAttacks();
        currentAttackCD = cdBetweenAttacks;
    }

    private void MageBossFirstStageState_OnWeakPointBroken()
    {
        health--;
        //Update UI
    }

    public override void UpdateState(MageBoss manager)
    {
        currentAttackCD -= Time.deltaTime;
        if(currentAttackCD < 0 && state == State.Idle)
        {
            switch(GetRandomAttack())
            {
                case FLAMEBALL_ATTACK:
                    FlameballCast(manager);
                    break;
                case LASER_ATTACK:
                    LaserCast(manager);
                    break;
            }
        }
    }

    private void SetCDBetweenAttacks()
    {
        currentAttackCD = cdBetweenAttacks;
    }

    private string GetRandomAttack()
    {
        int index = -1;
        do
        {
            index = Random.Range(0, attackSet.Length);
        } while (attackSet[index] == lastAttack);
        return attackSet[index];
    }

    private void FlameballCast(MageBoss manager)
    {
        manager.flameballspawnManager.InitializeFlameballAttackProperties(waveNumberTotal, spawnAmountTotal, spawnCDTotal,cdBetweenWaves, fallSpeed, true, new Vector3(-1,0,0));
        manager.animator.Play(MageBoss.FLAMEBALL_ANIM);
        state = State.FlameballCast;
        lastAttack = FLAMEBALL_ATTACK;
    }

    private void LaserCast(MageBoss manager)
    {
        manager.laser.InitializeLaser(3,0,0);
        manager.animator.Play(MageBoss.LASER_ANIM);
        state = State.LaserCast;
        lastAttack = LASER_ATTACK;
    }


    public override void OnCollisionEnter(TentacleStateManager manager, Collider2D collision)
    {
        throw new System.NotImplementedException();
    }
}
