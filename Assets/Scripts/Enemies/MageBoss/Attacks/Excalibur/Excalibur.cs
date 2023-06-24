using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Excalibur : MonoBehaviour
{
    public event Action OnSwordRetured;


    private const string GROUND_HIT_ANIM_TRIGGER = "GhoundHit";
    private const string DURATION_ENDED_TRIGGER = "DurationEnded";

    private const string CRACK_APPEAR_ANIM_TRIGGER = "OnAppear";
    private const string CRACK_DISAPPEAR_ANIM_TRIGGER = "OnDisappear";

    private Animator animator;
    private Rigidbody2D rb;

    private PlayerMovement player;
    private Rigidbody2D playerRB;

    private float fallSpeed = 500f;

    private bool hitTheGround = false;
    private float stayOnGroundDuration = 5f;

    private bool posAligned = false;
    private bool angleAligned = false;

    //return to boss' hands
    [SerializeField]
    private GameObject target;

    private Vector2 positionVelocitySmDamp;
    private float angleVelocitySmDamp;

    //[SerializeField]
    //private GameObject floorCrack;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        playerRB = player.GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 spawnPosition)
    {
        transform.position = spawnPosition;
        posAligned = false;
        angleAligned = false;
        hitTheGround = false;
        fallSpeed = 500f;
        rb.freezeRotation = false;
        stayOnGroundDuration = 5f;
        player = GameManager.Instance.GetPlayerReference();

        transform.Rotate(new Vector3(0,0,Vector2.SignedAngle(-transform.up, transform.position - player.transform.position)), Space.Self);

        transform.rotation = Quaternion.Euler(0, 0, -transform.rotation.eulerAngles.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hitTheGround)
        {
            rb.AddForce(-transform.right * fallSpeed * Time.deltaTime);
        }
        else
        {
            stayOnGroundDuration -= Time.deltaTime;
            if (stayOnGroundDuration <= 0) 
            {
                //floorCrack.GetComponent<Animator>().SetTrigger(CRACK_DISAPPEAR_ANIM_TRIGGER);
                animator.SetTrigger(DURATION_ENDED_TRIGGER);


                AlignPosition();
                AlignRotation();
                if (posAligned && angleAligned)
                {
                    gameObject.SetActive(false);
                    OnSwordRetured?.Invoke();
                }
            }
        }
    }

    private void AlignPosition()
    {
        if (!posAligned)
        {
            transform.position = Vector2.SmoothDamp(transform.position, target.transform.position, ref positionVelocitySmDamp, 1f);

            if(transform.position.x <= target.transform.position.x + 0.1f && transform.position.x >= target.transform.position.x -0.1f)
            {
                if(transform.position.y <= target.transform.position.y + 0.1f && transform.position.y >= target.transform.position.y -0.1f)
                {
                   posAligned = true;
                }
            }
        }
    }

    private void AlignRotation()
    {
        if (!angleAligned)
        {
            float currentAngle = transform.rotation.eulerAngles.z;
            float targetAngle = target.transform.rotation.eulerAngles.z;

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.SmoothDamp(currentAngle, targetAngle, ref angleVelocitySmDamp, 1f)));

            if(transform.eulerAngles.z <= target.transform.eulerAngles.z + 2 && transform.eulerAngles.z >= target.transform.eulerAngles.z - 2)
            {
                angleAligned = true;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Vector2 throwDirection = new Vector2(0, 0.01f);
            float playerPosX = player.transform.position.x;
            throwDirection.x = playerPosX < transform.position.x ? -0.01f : 0.01f;

            playerRB.AddForce(throwDirection, ForceMode2D.Impulse);
            player.GetDamaged(2);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            animator.SetTrigger(GROUND_HIT_ANIM_TRIGGER);

            rb.velocity = Vector2.zero;
            fallSpeed = 0;
            Vector2 throwDirection1 = new Vector2(0, 0.01f);
            float playerPosX = player.transform.position.x;
            throwDirection1.x = playerPosX < transform.position.x ? -0.012f : 0.012f;
            playerRB.AddForce(throwDirection1, ForceMode2D.Impulse);

            //floorCrack.transform.position = collision.GetContact(0).point;
            //.GetComponent<Animator>().SetTrigger(CRACK_APPEAR_ANIM_TRIGGER);

            hitTheGround = true;
            
            rb.freezeRotation = true;
        }
    }
}
