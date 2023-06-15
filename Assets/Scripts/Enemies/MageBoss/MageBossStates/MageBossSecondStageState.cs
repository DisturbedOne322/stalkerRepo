using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBossSecondStageState : MageBossBaseState
{
    public override event Action<int, int> OnCoreDestroyed;
    public override void EnterState(MageBoss manager)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(MageBoss manager)
    {
        throw new System.NotImplementedException();
    }

    public override void OnCollisionEnter(TentacleStateManager manager, Collider2D collision)
    {
        throw new System.NotImplementedException();
    }

}
