using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawAttack : MonoBehaviour
{
    private float attackAnimationCD = 0;
    private float attackAnimationCDTotal = 1.5f;

    private float damageCD = 0.5f;
    private float damageCDTotal = 0.5f;

    public const string CLAW_ATTACK_TRIGGER = "ClawAttack";

    [SerializeField]
    private Transform attackRange;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private ShieldMovement shieldMovement;

    [SerializeField]
    private AudioClip attackSound;

    private AudioSource audioSource;

    private PlayerMovement player;
    private Rigidbody2D playerRB;

    private float shieldYOffset;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameManager.Instance.GetPlayerReference();
        playerRB = player.GetComponent<Rigidbody2D>();
        shieldYOffset = player.GetComponent<CapsuleCollider2D>().size.y / 2;
    }

    // Update is called once per frame
    void Update()
    {
        attackAnimationCD -= Time.deltaTime;
        damageCD -= Time.deltaTime;

        if (attackAnimationCD > 0)
            return;

        if (player.transform.position.x > attackRange.position.x && transform.position.y < attackRange.position.y)
        {
            audioSource.PlayOneShot(attackSound);
            animator.SetTrigger(CLAW_ATTACK_TRIGGER);
            attackAnimationCD = attackAnimationCDTotal;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (damageCD > 0)
            return;
        if (collision.gameObject.CompareTag("Player"))
        {
            player.GetDamaged(2);
            playerRB.AddForce(new Vector2(-0.01f,0.01f), ForceMode2D.Impulse);
            damageCD = damageCDTotal;
        }
    }
}
