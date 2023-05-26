using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostVisuals : MonoBehaviour
{
    private Ghost ghost;

    private Animator animator;

    private readonly string ATTACK_ANIM = "IsAttacking";
    private readonly string DETECTED_ANIM = "DetectedPlayer";
    private readonly string DISAPPEAR_ANIM = "AttackFinished";
    private readonly string GET_DAMAGED_ANIM = "IsAttacked";

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        FocusedHeadlight.OnGhostFound += FocusedHeadlight_OnGhostFound;
        QTE.instance.OnQTEEnd += Instance_OnQTEEnd;
        ghost = GetComponent<Ghost>();
        ghost.OnAttack += Ghost_OnAttack;
        ghost.OnPlayerDetected += Ghost_OnPlayerDetected;
    }

    private void Ghost_OnPlayerDetected()
    {
        animator.SetBool(DETECTED_ANIM, true);
    }

    private void Ghost_OnAttack()
    {
        animator.SetBool(ATTACK_ANIM, true);
    }

    private void Instance_OnQTEEnd(IQTECaller caller)
    {
        Debug.Log("disappear");

        if (caller == ghost)
        {
            Debug.Log("I called");
            animator.SetTrigger(DISAPPEAR_ANIM);
        }
    }

    private void FocusedHeadlight_OnGhostFound()
    {
        animator.SetBool(GET_DAMAGED_ANIM, true);
    }

}
