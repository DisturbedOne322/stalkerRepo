using System.Collections;
using UnityEngine;

public class MageBossFirstStageState : MageBossBaseState
{
    private int health;
    private PlayerMovement player;

    private float currentAttackCD = 7f;
    private float cdBetweenAttacks = 7f;

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
    private float laserAnimationDuration = 5f;
    private float laserThickness = 0.5f;

    private enum State
    {
        Idle,
        FlameballCast,
        LaserCast,
        LaserPrepare
    }

    private State state;

    public override void EnterState(MageBoss manager)
    {
        attackSet[0] = FLAMEBALL_ATTACK;
        attackSet[1] = LASER_ATTACK;
        player = manager.player;
        health = manager.collidersArray.Length;
        manager.flameballspawnManager.OnAttackFinished += FlameballspawnManager_OnAttackFinished;
        manager.laser.OnAttackFinished += Laser_OnAttackFinished;

        for (int i = 0; i < manager.collidersArray.Length; i++)
        {
            manager.collidersArray[i].OnWeakPointBroken += MageBossFirstStageState_OnWeakPointBroken;
        }

        state = State.Idle;
    }

    private void Laser_OnAttackFinished(MageBoss manager)
    {
        state = State.Idle;
        SetCDBetweenAttacks();
        manager.animator.SetTrigger(MageBoss.LASER_END_TRIGGER);
    }

    private void FlameballspawnManager_OnAttackFinished()
    {
        state = State.Idle;
        SetCDBetweenAttacks();
    }

    private void MageBossFirstStageState_OnWeakPointBroken()
    {
        health--;
        //Update UI
    }

    public override void UpdateState(MageBoss manager)
    {
        currentAttackCD -= Time.deltaTime;
        if (currentAttackCD < 0 && state == State.Idle)
        {
            switch (GetRandomAttack())
            {
                case FLAMEBALL_ATTACK:
                    FlameballCast(manager);
                    break;
                case LASER_ATTACK:
                    LaserCast(manager);
                    break;
            }
        }
        if (state == State.LaserPrepare)
        {
            var a = manager.animator.GetCurrentAnimatorStateInfo(0);
            if (a.normalizedTime >= 0.99f)
            {
                manager.laser.InitializeLaser(laserAnimationDuration, laserThickness, manager);
                //manager.animator.Play(MageBoss.LASER_ANIM);
                state = State.LaserCast;
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
        manager.animator.Play(MageBoss.LASER_PREPARE_ANIM);
        state = State.LaserPrepare;
        lastAttack = LASER_ATTACK;
    }

    public override void OnCollisionEnter(TentacleStateManager manager, Collider2D collision)
    {
        throw new System.NotImplementedException();
    }
}
