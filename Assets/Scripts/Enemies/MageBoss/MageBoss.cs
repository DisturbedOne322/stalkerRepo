using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class MageBoss : MonoBehaviour
{
    public event Action<int, int> OnHPChanged;
    public event Action<MageBoss> OnPlayerInClawAttackRange;
    [SerializeField]
    public WeakPoint[] collidersArray;

    public const string APPEAR_ANIM = "Base Layer.Appear";
    public const string DAMAGED_ANIM = "OnDamaged";
    public const string LASER_END_TRIGGER = "LaserAttackEnd";
    public const string FLAMEBALL_ANIM = "Base Layer.FlameballCast";
    public const string LASER_ANIM = "Base Layer.LaserCast";
    public const string LASER_PREPARE_ANIM = "Base Layer.LaserCastPrepare";
    public const string LASER_END_ANIM = "Base Layer.LaserCastEnd";
    public const string EXCALIBUR_ATTACK_TRIGGER = "ExcaliburAttack";
    public const string MAGIC_HOLE_ATTACK_TRIGGER = "MagicHoleAttack";
    public const string DEFEAT_ANIM_BOOL = "OnDefeat";
    public Animator animator;

    [SerializeField]
    private GameObject laserArm;
    [SerializeField]    
    private GameObject laserLight;
    [SerializeField]
    private GameObject wandArm;
    [SerializeField]
    private GameObject wandLight;

    [SerializeField]
    public GameObject sword;
    [SerializeField] 
    private GameObject swordArm;

    [SerializeField]
    private GameObject magicHoleArm;

    [SerializeField]
    public Excalibur excaliburAttack;
    [SerializeField]
    public Transform swordSpawnPoint;

    [SerializeField]
    public MagicHole magicHole;

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

    public MageBossFirstStageState firstStageState = new MageBossFirstStageState();
    public MageBossSecondStageState secondStageState = new MageBossSecondStageState();
    public MageBossThirdStageState thirdStageState = new MageBossThirdStageState();

    

    private void Awake()
    {
        flameballspawnManager = GetComponent<FlameballspawnManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        currentState = thirdStageState;
        animator = GetComponent<Animator>();

        for(int i = 0; i < collidersArray.Length; i++)
        {
            collidersArray[i].OnWeakPointBroken += MageBoss_OnWeakPointBroken;
        }
        SetNormalOutline();
        currentState.EnterState(this);
        currentState.OnCoreDestroyed += CurrentState_OnCoreDestroyed;
    }
    //called from anim event
    private void SwordThrown()
    {
        sword.SetActive(false);
    }
    // called from stage 3
    public void SwordReturned()
    {
        sword.SetActive(true);
    }

    private void CurrentState_OnCoreDestroyed(int arg1, int arg2)
    {
        OnHPChanged?.Invoke(arg1,arg2);
    }

    private void MageBoss_OnWeakPointBroken()
    {
        AnimatorClipInfo[] m_CurrentClipInfo = this.animator.GetCurrentAnimatorClipInfo(0);
        if (m_CurrentClipInfo[0].clip.name == "Idle")
            animator.SetTrigger(DAMAGED_ANIM);

        material.SetColor(outlineColor, damagedColor);
        material.SetFloat(outlineThickness, damagedThickness);
        SetNormalOutline();
    }

    public void SwitchState(MageBossBaseState nextState)
    {
        currentState.OnCoreDestroyed -= CurrentState_OnCoreDestroyed;
        currentState = nextState;
        currentState.EnterState(this);
        currentState.OnCoreDestroyed += CurrentState_OnCoreDestroyed;
    }

    public void EnableSecondStageArms()
    {
        laserArm.SetActive(true);
        laserLight.SetActive(true);
        wandArm.SetActive(true);
        wandLight.SetActive(true);
    }

    public void EnableThirdStageArms()
    {
        sword.SetActive(true);
        swordArm.SetActive(true);
        magicHoleArm.SetActive(true);
    }

    IEnumerator ReturnToNormalOutline()
    {
        Color c = new Color();
        c.a = 1;
        for (float red = 1f; red >= 0; red -= 0.1f)
        {
            c.r = red;
            material.SetColor(outlineColor, c);
            yield return new WaitForSeconds(.05f);
        }
    }

    public void ResetColliders()
    {
        foreach(var collider in collidersArray)
        {
            collider.ResetWeakPoint();
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
