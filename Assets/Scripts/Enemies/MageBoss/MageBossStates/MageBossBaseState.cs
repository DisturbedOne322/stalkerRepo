using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MageBossBaseState
{
    public abstract void EnterState(MageBoss manager);
    public abstract void UpdateState(MageBoss manager);
    public abstract void OnCollisionEnter(TentacleStateManager manager, Collider2D collision);
}
