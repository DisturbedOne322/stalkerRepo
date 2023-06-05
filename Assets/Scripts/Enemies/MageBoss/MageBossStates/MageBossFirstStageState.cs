using UnityEngine;

public class MageBossFirstStageState : MageBossBaseState
{
    private int health;
    private PlayerMovement player;

    private float currentAttackCD = 5f;
    private float cdBetweenAttacks = 5f;

    private Vector3 flameballSpawnStartPos;
    private Vector3 currentFlameBallPos;
    private GameObject flameball;

    private float flameballSpawnOffset;
    private float spawnCD = 0;
    private float spawnCDTotal = 0.5f;
    private float currentFlameBallWave = 0;
    private float flameBallWavesTotal = 2;
    private float flameBallsPerWave = 5;
    private float flameBallsSpawned = 0;


    private enum State
    {
        Idle,
        FlameballCast
    }

    private State state;

    public override void EnterState(MageBoss manager)
    {
        player = manager.player;
        health = manager.collidersArray.Length;
        flameball = manager.flameball;

        flameballSpawnOffset = flameball.GetComponent<BoxCollider2D>().size.x * 4 + 0.25f;
        flameballSpawnStartPos = manager.flameballSpawnPos.position;
        for (int i = 0; i < manager.collidersArray.Length; i++)
        {
            manager.collidersArray[i].OnWeakPointBroken += MageBossFirstStageState_OnWeakPointBroken;
        }

        state = State.Idle;
    }

    private void MageBossFirstStageState_OnWeakPointBroken()
    {
        health--;
        //Update UI
    }

    private void FlameBallWaveAttack()
    {
        spawnCD -= Time.deltaTime;

        if(flameBallsSpawned >= flameBallsPerWave)
        {
            currentFlameBallWave++;

            if (currentFlameBallWave >= flameBallWavesTotal)
            {
                state = State.Idle;
                SetCDBetweenAttacks();
                return;
            }
            flameBallsSpawned = 0;
            ResetFlameballSpawnPos();
        }

        if(spawnCD < 0)
        {
            spawnCD = spawnCDTotal;
            GameObject tempFlameball = GameObject.Instantiate(flameball, currentFlameBallPos, Quaternion.identity);
            currentFlameBallPos.x -= flameballSpawnOffset;
            flameBallsSpawned++;
        }
    }

    public override void UpdateState(MageBoss manager)
    {
        currentAttackCD -= Time.deltaTime;
        if(currentAttackCD < 0 && state == State.Idle)
        {
            manager.animator.Play(MageBoss.FLAMEBALL_ANIM);
        }
        switch(state)
        {
            case State.Idle:
                break;
            case State.FlameballCast:
                FlameBallWaveAttack();
                break;
        }
    }

    private void SetCDBetweenAttacks()
    {
        currentAttackCD = cdBetweenAttacks;
    }

    private void ResetFlameballSpawnPos()
    {
        currentFlameBallPos = flameballSpawnStartPos;
    }


    public override void FlameballCast(MageBoss manager)
    {
        ResetFlameballSpawnPos();

        state = State.FlameballCast;
        flameBallsSpawned = 0;
        currentFlameBallWave = 0;
    }

    public override void OnCollisionEnter(TentacleStateManager manager, Collider2D collision)
    {
        throw new System.NotImplementedException();
    }
}
