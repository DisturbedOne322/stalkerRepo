using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBossThirdStageState : MageBossBaseState
{
    public override event System.Action<int, int> OnCoreDestroyed;
    private int health = 8;

    private Animator animator;

    private float currentAttackCD = 2f;
    private float cdBetweenAttacks = 2f;

    private const string FLAMEBALL_ATTACK = "Flameball";
    private const string LASER_ATTACK = "Laser";
    private const string EXCALIBUR_ATTACK = "Excalibur";
    private const string MAGIC_HOLE_ATTACK = "MagicHole";
    private string lastAttack;
    private string[] attackSet = new string[4];

    private MageBoss manager;

    private float switchStateDelay = 10f;

    //flameball
    private float spawnCDTotal = 0.25f; // cd between each flameball
    private float cdBetweenWaves = 0.25f;
    private int waveNumberTotal = 4;
    private int spawnAmountTotal = 3;
    private float fallSpeed = 20;
    private float scale = 2f;

    //laser
    private float laserAnimationDuration = 13;
    private float laserThickness = 0.9f;

    //excalibur
    private float excaliburSpawnDelay = 3f;

    //magic hole
    private float magicHoleDuration = 5;
    private float pullingForce = 0.015f;

    private bool defeated = false;

    private enum State
    {
        Idle,
        FlameballCast,
        LaserCast,
        LaserPrepare,
        ExcaliburCast
    }

    private State state;

    public override void EnterState(MageBoss manager)
    {
        this.manager = manager;
        manager.ResetColliders();
        manager.EnableThirdStageArms();
        animator = manager.GetComponent<Animator>();
        animator.Rebind();
        animator.Update(0);
        animator.Play(MageBoss.APPEAR_ANIM);

        attackSet[0] = FLAMEBALL_ATTACK;
        attackSet[1] = LASER_ATTACK;
        attackSet[2] = EXCALIBUR_ATTACK;
        attackSet[3] = MAGIC_HOLE_ATTACK;
        health = manager.collidersArray.Length;
        manager.flameballspawnManager.OnAttackFinished += FlameballspawnManager_OnAttackFinished;
        manager.laser.OnAttackFinished += Laser_OnAttackFinished;

        for (int i = 0; i < manager.collidersArray.Length; i++)
        {
            manager.collidersArray[i].OnWeakPointBroken += MageBossFirstStageState_OnWeakPointBroken;
        }

        state = State.Idle;

        manager.excaliburAttack.OnSwordRetured += ExcaliburAttack_OnSwordRetured;
    }

    private void ExcaliburAttack_OnSwordRetured()
    {
        manager.SwordReturned();
        state = State.Idle;
        SetCDBetweenAttacks();
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
            animator.SetTrigger(MageBoss.DEFEAT_ANIM_BOOL);
            defeated = true;
        }
        //Update UI
    }

    public override void UpdateState(MageBoss manager)
    {
        if (defeated)
        {
            switchStateDelay -= Time.deltaTime;
            if (switchStateDelay <= 0)
            {
                //
            }
            return;
        }
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
                case EXCALIBUR_ATTACK:
                    ExcaliburCast(manager);
                    break;
                case MAGIC_HOLE_ATTACK:
                    MagicHoleCast(manager); 
                break;
            }
        }
        if (state == State.LaserPrepare)
        {
            var a = manager.animator.GetCurrentAnimatorStateInfo(0);
            if (a.normalizedTime >= 0.99f)
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
            index = UnityEngine.Random.Range(0, attackSet.Length);
        } while (attackSet[index] == lastAttack);
        return attackSet[index];
    }

    private void MagicHoleCast(MageBoss manager)
    {
        manager.magicHole.Initialize(magicHoleDuration, pullingForce);
        manager.animator.SetTrigger(MageBoss.MAGIC_HOLE_ATTACK_TRIGGER);
        SetCDBetweenAttacks();
        lastAttack = MAGIC_HOLE_ATTACK;
    }

    private void ExcaliburCast(MageBoss manager)
    {
        manager.StartCoroutine(SpawnExcalibur(manager));
        manager.animator.SetTrigger(MageBoss.EXCALIBUR_ATTACK_TRIGGER);
        state = State.ExcaliburCast;
        lastAttack = EXCALIBUR_ATTACK;
    }

    IEnumerator SpawnExcalibur(MageBoss manager)
    {
        yield return new WaitForSeconds(excaliburSpawnDelay);

        manager.excaliburAttack.gameObject.SetActive(true);
        manager.excaliburAttack.Initialize(manager.swordSpawnPoint.position);
    }

    private void FlameballCast(MageBoss manager)
    {
        manager.flameballspawnManager.InitializeFlameballAttackProperties(waveNumberTotal, spawnAmountTotal, spawnCDTotal, cdBetweenWaves, fallSpeed, scale, true, new Vector3(-1, 0, 0));
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
