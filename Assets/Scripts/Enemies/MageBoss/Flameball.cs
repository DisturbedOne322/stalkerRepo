using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flameball : MonoBehaviour
{
    private Transform spawnPos;
    
    private float speed = 2;
    public Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;


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
        transform.Translate(new Vector2(0, -speed * Time.deltaTime));
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
