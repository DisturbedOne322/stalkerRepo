using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flameball : MageBossBaseAttack
{
    private Transform spawnPos;
    private Vector2 fallDirection = new Vector2 (-0.77f, -0.77f);
    private float speed = 10;
    public float Speed
    {
        get;
        set;
    }
    public Animator animator;
    [SerializeField]
    public SpriteRenderer spriteRenderer;


    [SerializeField]
    private Sprite sprite;

    private bool isFalling = true;

    public FlameballBaseState currentState;
    public FlameballFallingState fallingState = new FlameballFallingState();
    public FlameballPuddleState puddleState = new FlameballPuddleState();

    private float spriteSizeMultiplier = 6;
    private float colliderOffset = -0.4f;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        currentState = fallingState;
        currentState.EnterState(this);
    }

    private void Update()
    {
        currentState.UpdateState(this);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        currentState.OnTriggerStay2D(this, collision);
    }

    public void UpdateCollider()
    {
        GetComponent<BoxCollider2D>().size = new Vector2(sprite.bounds.size.x * spriteSizeMultiplier, sprite.bounds.size.y * spriteSizeMultiplier / 2);
        GetComponent<BoxCollider2D>().offset = new Vector2(0, colliderOffset);
    }

    public void SwitchState(FlameballBaseState nextState)
    {
        currentState = nextState;
        currentState.EnterState(this);
    }

    public void FallDown()
    {
        transform.Translate(-transform.up * speed * Time.deltaTime);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
