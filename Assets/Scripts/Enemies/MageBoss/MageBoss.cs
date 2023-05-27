using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBoss : MonoBehaviour
{
    [SerializeField]
    public WeakPoint[] collidersArray;

    public PlayerMovement player;

    private MageBossBaseState currentState;

    private MageBossFirstStageState firstStageState = new MageBossFirstStageState();
    private MageBossSecondStageState secondStageState = new MageBossSecondStageState();
    private MageBossThirdStageState thirdStageState = new MageBossThirdStageState();


    //first stage attacks
    [SerializeField]
    private GameObject flameball;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        currentState = firstStageState;
        currentState.EnterState(this);
    }
    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }
}
