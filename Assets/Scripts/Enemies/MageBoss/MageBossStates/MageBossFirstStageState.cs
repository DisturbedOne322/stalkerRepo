using System.Collections;
using UnityEngine;

public class MageBossFirstStageState : MageBossBaseState
{
    public override event System.Action<int, int> OnCoreDestroyed;
    private int health = 8;

    private Animator animator;

    private float currentAttackCD = 6f;
    private float cdBetweenAttacks = 4f;

    private const string FLAMEBALL_ATTACK = "Flameball";
    private const string LASER_ATTACK = "Laser";
    private string lastAttack;
    private string[] attackSet = new string[2];


    //flameball
    private float spawnCDTotal = 1f; // cd between each flameball
    private float cdBetweenWaves = 3f;
    private int waveNumberTotal = 1;
    private int spawnAmountTotal = 10;
    private float fallSpeed = 11;
    private float scale = 1;

    //laser
    private float laserAnimationDuration = 7;
    private float laserThickness = 0.3f;

    private bool defeated = false;

    private float stateChangeDelay = 12f;

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
        animator = manager.GetComponent<Animator>();
        animator.Play(MageBoss.APPEAR_ANIM);
        attackSet[0] = FLAMEBALL_ATTACK;
        attackSet[1] = LASER_ATTACK;
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
        animator.SetTrigger(MageBoss.LASER_END_TRIGGER);
    }

    private void FlameballspawnManager_OnAttackFinished()
    {
        state = State.Idle;
        SetCDBetweenAttacks();
    }

    private void MageBossFirstStageState_OnWeakPointBroken()
    {
        health--;
        OnCoreDestroyed?.Invoke(health, 8);
        if (health <= 0)
        {
            animator.SetBool(MageBoss.DEFEAT_ANIM_BOOL, true);
            defeated = true;
        }
        //Update UI
    }

    public override void UpdateState(MageBoss manager)
    {
        if(defeated)
        {
            stateChangeDelay -= Time.deltaTime;
            if (stateChangeDelay <= 0)
            {
                for (int i = 0; i < manager.collidersArray.Length; i++)
                {
                    manager.collidersArray[i].OnWeakPointBroken -= MageBossFirstStageState_OnWeakPointBroken;
                }
                manager.flameballspawnManager.OnAttackFinished -= FlameballspawnManager_OnAttackFinished;
                manager.laser.OnAttackFinished -= Laser_OnAttackFinished;
                manager.SwitchState(manager.secondStageState);
            }
            
            return;
        }
        currentAttackCD -= Time.deltaTime;
        if (currentAttackCD < 0 && state == State.Idle)
        {
            //switch (GetRandomAttack())
            //{
            //    case FLAMEBALL_ATTACK:
            //        FlameballCast(manager);
            //        break;
            //    case LASER_ATTACK:
                    LaserCast(manager);
            //        break;
            //}
        }
        if (state == State.LaserPrepare)
        {
            AnimatorClipInfo[] m_CurrentClipInfo = this.animator.GetCurrentAnimatorClipInfo(0);
            if (m_CurrentClipInfo[0].clip.name == "LaserCast")
            {
                manager.laser.InitializeLaser(laserAnimationDuration, laserThickness, manager);
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
        manager.PlayFlyUpSound();
        manager.flameballspawnManager.InitializeFlameballAttackProperties(waveNumberTotal, spawnAmountTotal, spawnCDTotal,cdBetweenWaves, fallSpeed, scale, true, new Vector3(-1,0,0));
        manager.animator.Play(MageBoss.FLAMEBALL_ANIM);
        state = State.FlameballCast;
        lastAttack = FLAMEBALL_ATTACK;
    }

    private void LaserCast(MageBoss manager)
    {
        manager.PlayFlyUpSound();
        manager.animator.Play(MageBoss.LASER_PREPARE_ANIM);
        state = State.LaserPrepare;
        lastAttack = LASER_ATTACK;
    }

    public override void OnCollisionEnter(TentacleStateManager manager, Collider2D collision)
    {
        throw new System.NotImplementedException();
    }
}
