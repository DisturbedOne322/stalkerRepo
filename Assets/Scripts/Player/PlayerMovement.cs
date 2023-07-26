using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public event Action<StaminaState> OnStaminaStateChange;
    public event Action<GameManager.PlayerHealthStatus> OnHealthChanged;
    public event Action OnPlayerRespawned;
    public event Action OnPlayerTeleported;
    public event Action OnPlayerTeleportedArrived;
    public event Action OnPlayerDied;
    private Rigidbody2D rb2D;

    [SerializeField]
    private LayerMask groundLayerMask;

    private int healthPoints = 10;
    private readonly int maxHealthPoints = 10;

    public int HealthPoints
    {
        get { return healthPoints; }
    }

    public int MaxHealthPoint
    {
        get { return maxHealthPoints; }
    }

    [SerializeField]
    private readonly float originalSpeed = 0.85f;
    private float moveSpeed = 0.85f;
    private readonly float sprintSpeed = 1.35f;
    private readonly float backwardMoveSpeedMultiplier = 0.85f;

    private float stamina = 1;
    public float Stamina
    {
        get { return stamina; }
    }

    #region Stamina
    public enum StaminaState
    {
        Idle,
        Regen,
        Deplete
    }
    private readonly float jumpStaminaConsumption = 0.1f;
    private readonly float staminaRegen = 0.1f;
    private readonly float staminaSpentPerSecond = 0.1f;
    #endregion

    private bool isSprinting = false;
    private bool isMoving = false;
    private bool canMove = true;

    private readonly float jumpForce = 0.0065f;

    private CapsuleCollider2D capsuleCollider;

    private float distanceToTheGround;
    private bool isFalling = false;

    [SerializeField]
    private GameObject[] lightSources;

    public bool IsFalling
    {
        get { return isFalling; }
    }

    private bool isGrounded = true;
    public bool IsGrounded
    {
        get { return isGrounded; }
    }

    float movementDirection;
    private bool inAnimation = false;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        distanceToTheGround = capsuleCollider.bounds.extents.y / 2;
    }

    // Start is called before the first frame update
    void Start()
    {
        InputManager.Instance.OnJumpAction += InputManager_OnJumpAction;
        InputManager.Instance.OnSprintActionStarted += Instance_OnSprintActionStarted;
        InputManager.Instance.OnSprintActionEnded += Instance_OnSprintActionEnded;

        QTE.instance.OnQTEStart += QTE_OnQTEStart;
        QTE.instance.OnQTEEnd += QTE_OnQTEEnd;
        QTE.instance.OnQTERoundFailed += QTE_OnQTERoundFailed;
    }

    private void OnDestroy()
    {
        InputManager.Instance.OnJumpAction -= InputManager_OnJumpAction;
        InputManager.Instance.OnSprintActionStarted -= Instance_OnSprintActionStarted;
        InputManager.Instance.OnSprintActionEnded -= Instance_OnSprintActionEnded;

        QTE.instance.OnQTEStart -= QTE_OnQTEStart;
        QTE.instance.OnQTEEnd -= QTE_OnQTEEnd;
        QTE.instance.OnQTERoundFailed -= QTE_OnQTERoundFailed;
    }

    private void QTE_OnQTERoundFailed(int damage)
    {
        GetDamaged(damage);
    }

    private void QTE_OnQTEEnd(IQTECaller caller)
    {
        canMove = true;
    }

    private void QTE_OnQTEStart()
    {
        canMove = false;
    }

    private void Instance_OnSprintActionEnded()
    {
        isSprinting = false;
        moveSpeed = originalSpeed;
    }

    private void Instance_OnSprintActionStarted()
    {
        if(stamina > 0.1f)
        {
            isSprinting = true;
            moveSpeed *= sprintSpeed;
        }
    }

    private void InputManager_OnJumpAction()
    {
        if (inAnimation)
            return;
        if(!canMove)        
            return;
        

        if(IsGrounded)
        {
            if (stamina > jumpStaminaConsumption)
            {
                rb2D.AddForce(new Vector2(movementDirection, 1.5f) * jumpForce, ForceMode2D.Impulse);
                stamina -= jumpStaminaConsumption;
            }
        }
    }

    private void TurnLightsOff()
    {
        for(int i = 0; i < lightSources.Length; i++)
        {
            lightSources[i].SetActive(false);
        }
    }

    private void TurnLightsOn()
    {
        for (int i = 0; i < lightSources.Length; i++)
        {
            lightSources[i].SetActive(true);
        }
    }


    public void GetTeleported()
    {
        canMove = false;
        rb2D.velocity = Vector2.zero;
        TurnLightsOff();
        OnPlayerTeleported?.Invoke();
    }

    private void OnTeleportArrived()
    {
        canMove = true;
        TurnLightsOn();
        OnPlayerTeleportedArrived?.Invoke();
    }


    // Update is called once per frame
    void Update()
    {
        if (!canMove)       
            return;
        

        movementDirection = InputManager.Instance.GetMovementDirection();
        isMoving = movementDirection != 0;
        if (isMoving && isGrounded)
        {
            float moveSpeedMultiplier = (movementDirection > 0 && transform.localScale.x > 0)
                || (movementDirection < 0 && transform.localScale.x < 0) ? 1 : backwardMoveSpeedMultiplier;  
            rb2D.AddForce(new Vector2(movementDirection * moveSpeed * moveSpeedMultiplier * Time.deltaTime, 0));
            Idle.ReportAction();
        }
        isFalling = rb2D.velocity.y < -2f;
        isGrounded = CheckIsGrounded();

        if (isSprinting)
            DepleteStamina();
        if(IsGrounded)
            RegenStamina();
    }

    public void RestoreFullHealth()
    {
        healthPoints = MaxHealthPoint;
        OnHealthChanged?.Invoke(GameManager.PlayerHealthStatus.HighHP);
        OnPlayerRespawned?.Invoke();
        stamina = 1;
    }

    private void DepleteStamina()
    {
        if(isMoving)
        {
            stamina -= staminaSpentPerSecond * Time.deltaTime;
            OnStaminaStateChange?.Invoke(StaminaState.Deplete);
            if (stamina < 0)
            {
                stamina = 0;
                isSprinting = false;
                moveSpeed = originalSpeed;
            }
        }
    }

    private void RegenStamina()
    {
        if(!isMoving || !isSprinting)
        {
            OnStaminaStateChange?.Invoke(StaminaState.Regen);
            stamina += staminaRegen * Time.deltaTime;
            if (stamina > 1)
                stamina = 1;
        }
    }

    private bool CheckIsGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(capsuleCollider.bounds.center, new Vector2(1,2),0, Vector2.down, distanceToTheGround + 0.1f, groundLayerMask);
        BoxCastDrawer.Draw(rayHit,capsuleCollider.bounds.center, new Vector2(1, 0.001f), 0, Vector2.down, distanceToTheGround);
        return rayHit.collider != null;
    }

    public void GetDamaged(int damage)
    {
        if (healthPoints <= 0)
            return;

        healthPoints -= damage;
        OnHealthChanged?.Invoke(healthPoints * 1.0f / maxHealthPoints > 0.5f? GameManager.PlayerHealthStatus.HighHP:
            healthPoints * 1.0f / maxHealthPoints > 0.25f ? GameManager.PlayerHealthStatus.MidHP : GameManager.PlayerHealthStatus.LowHP);
        if(healthPoints <= 0)
        {
            OnPlayerDied?.Invoke();
        }
        SoundManager.Instance.PlayGetHurtSound();
    }

    public void GetCriticaldamage()
    {
        if (healthPoints > 3)
            healthPoints -= 3;
        else
            healthPoints = 1;
        OnHealthChanged?.Invoke(GameManager.PlayerHealthStatus.LowHP);
        SoundManager.Instance.PlayGetHurtSound();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TeleportDestination"))
        {
            OnTeleportArrived();
        }
    }
}

