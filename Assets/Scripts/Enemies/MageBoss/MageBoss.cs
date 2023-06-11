using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageBoss : MonoBehaviour
{
    public event Action<MageBoss> OnPlayerInClawAttackRange;
    [SerializeField]
    public WeakPoint[] collidersArray;

    private const string DAMAGED_ANIM = "OnDamaged";
    public const string LASER_END_TRIGGER = "LaserAttackEnd";
    public const string FLAMEBALL_ANIM = "Base Layer.FlameballCast";
    public const string LASER_ANIM = "Base Layer.LaserCast";
    public const string LASER_PREPARE_ANIM = "Base Layer.LaserCastPrepare";
    public const string LASER_END_ANIM = "Base Layer.LaserCastEnd";
    public Animator animator;

    public FlameballspawnManager flameballspawnManager;
    [SerializeField]
    public Laser laser;

    [SerializeField]
    private Material material;

    private const string outlineColor = "_OutlineColor";
    private const string outlineThickness = "_OutlineThickness";

    private Vector4 normalColor = new Vector4(0,0,0,1);
    private Vector4 damagedColor = new Vector4(1,0,0,1);

    private float normalThickness = 0;
    private float damagedThickness = 1;

    public PlayerMovement player;

    private MageBossBaseState currentState;

    private MageBossFirstStageState firstStageState = new MageBossFirstStageState();
    private MageBossSecondStageState secondStageState = new MageBossSecondStageState();
    private MageBossThirdStageState thirdStageState = new MageBossThirdStageState();


    //first stage attacks
    [SerializeField]
    public GameObject flameball;

    public Transform flameballSpawnPos;

    [SerializeField]
    private Transform clawAttackRange;

    private void Awake()
    {
        flameballspawnManager = GetComponent<FlameballspawnManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        currentState = firstStageState;
        currentState.EnterState(this);
        animator = GetComponent<Animator>();
        for(int i = 0; i < collidersArray.Length; i++)
        {
            collidersArray[i].OnWeakPointBroken += MageBoss_OnWeakPointBroken;
        }
        SetNormalOutline();
    }

    private void MageBoss_OnWeakPointBroken()
    {
        animator.SetTrigger(DAMAGED_ANIM);
        material.SetColor(outlineColor, damagedColor);
        material.SetFloat(outlineThickness, damagedThickness);
    }

    IEnumerator ReturnToNormalOutline()
    {
        Color c = new Color();
        c.a = 1;
        for (float red = 1f; red >= 0; red -= 0.1f)
        {
            c.r = red;
            material.SetColor(outlineColor, c);
            yield return new WaitForSeconds(.1f);
        }
    }
    private void SetNormalOutline()
    {
        StartCoroutine("ReturnToNormalOutline");
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }
}
