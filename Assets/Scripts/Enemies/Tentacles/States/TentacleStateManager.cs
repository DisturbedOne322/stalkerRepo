using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleStateManager : MonoBehaviour, IQTECaller
{
    public Animator animator;
    private BoxCollider2D boxCollider2D;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public PlayerMovement player;
    [SerializeField]
    private Transform playerHoldPoint;


    private TentaclesBaseState currentState;
    private TentaclesBaseState prevState;

    public TentacleAttackState attackState = new TentacleAttackState();
    public TentacleDisappearState disappearState = new TentacleDisappearState();
    public TentacleIdleState idleState = new TentacleIdleState();
    public TentacleShrinkState shrinkState = new TentacleShrinkState();
    public TentacleSpreadState spreadState = new TentacleSpreadState();

    public bool underLight = false;
    private float lastLightTime;
    private float shrinkDelay = 0.25f;

    public bool pullingPlayer = false;
    public bool playerFreed = false;

    [SerializeField]
    public LayerMask playerLayer;

    private AudioSource audioSource;
    private float maxVolumeDistance = 20f;

    [SerializeField]
    private AudioClip idleSound;
    [SerializeField]
    private AudioClip shrinkAndSpreadSound;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        currentState = Random.Range(0,1f) > 0.5f? attackState:idleState;
        prevState = currentState;
        currentState.EnterState(this);

    }

    // Start is called before the first frame update
    void Start()
    {
        FocusedHeadlight.OnTentacleFound += FocusedHeadlight_OnTentacleFound;
        player = GameManager.Instance.GetPlayerReference();
        QTE.instance.OnQTEEnd += Instance_OnQTEEnd;
    }

    private void Instance_OnQTEEnd(IQTECaller caller)
    {
        if(caller == this)
        {
            pullingPlayer = false;
            playerFreed = true;
        }
    }

    private void FocusedHeadlight_OnTentacleFound(TentacleStateManager obj)
    {
        if (obj == this)
        {
            underLight = true;
            lastLightTime = Time.time;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
        underLight = lastLightTime + shrinkDelay > Time.time;

        Vector2 spriteSize = spriteRenderer.bounds.size;
        boxCollider2D.size = spriteSize;
        Vector2 newColliderPosition = new Vector2(0, spriteRenderer.gameObject.transform.localPosition.y);
        boxCollider2D.offset = newColliderPosition;

        if (pullingPlayer)
        {
            player.transform.position = playerHoldPoint.position;
        }

        audioSource.volume = 1 - (Vector2.Distance(transform.position, player.transform.position) / maxVolumeDistance);
    }

    public void SwitchState(TentaclesBaseState newState)
    {
        prevState = currentState;
        currentState = newState;
        currentState.EnterState(this);
    }

    public TentaclesBaseState GetPrevState() 
    {
        return prevState;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        pullingPlayer = true;
        currentState.OnCollisionEnter(this, collision);
    }

    public void SetIdleSound()
    {
        audioSource.clip = idleSound;
    }

    public void SetShrinkSpreadSound()
    {
        audioSource.clip = shrinkAndSpreadSound;
    }
}
