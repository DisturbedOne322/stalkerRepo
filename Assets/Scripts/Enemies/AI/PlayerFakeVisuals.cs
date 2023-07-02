using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFakeVisuals : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private ApproachPlayer approachPlayer;


    private const string RUNNING_ANIM_BOOL = "IsRunning";
    private const string AIMING_ANIM_BOOL = "IsAiming";

    // Start is called before the first frame update
    void Start()
    {
        approachPlayer.OnPlayerInRange += ApproachPlayer_OnPlayerInRange;
    }

    private void ApproachPlayer_OnPlayerInRange(bool inRange)
    {
        animator.SetBool(RUNNING_ANIM_BOOL, !inRange);
        animator.SetBool(AIMING_ANIM_BOOL, inRange);
    }
}
