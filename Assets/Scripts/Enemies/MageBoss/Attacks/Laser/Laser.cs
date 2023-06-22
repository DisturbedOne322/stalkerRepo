using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : MageBossBaseAttack
{
    public event Action<MageBoss> OnAttackFinished;
    [SerializeField]
    private LineRenderer lineRenderer;

    private RaycastHit2D hit;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private LayerMask playerLayer;

    private float trackOffsetX = 0.5f;

    private PlayerMovement player;

    private readonly float smoothDampTimeVertical = 0.5f;
    private readonly float smoothDampTimeHorizontal = 1.5f;
    private float velocityHorizontal = 0;
    private float velocityVertical = 0;

    private float duration;

    private AudioSource audioSource;
    [SerializeField]
    private AudioClip laserCharge;
    [SerializeField]
    private AudioClip laserFalling;
    private float audioDelayDuration = 1.5f;

    private bool attackFinished = true;

    private MageBoss caller;

    private float damageCD = 0f;
    private float damageCDTotal = 1f;
    private int damage = 1;

    private FocusedHeadlight focusedHeadlight;


    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
        focusedHeadlight = player.GetComponentInChildren<FocusedHeadlight>();
    }

    // Update is called once per frame
    void Update()
    {
        damageCD -= Time.deltaTime;

        if (attackFinished)
            return;

        duration -= Time.deltaTime;
        if(duration < 0)
        {
            OnAttackFinished?.Invoke(caller);
            attackFinished = true;
            audioSource.Stop();
            audioSource.PlayOneShot(laserFalling);
        }
        SetLaserPosition();
        DamagePlayer();

    }
    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(laserCharge);
        audioSource.PlayDelayed(audioDelayDuration);
    }
    private void SetLaserPosition()
    {
        lineRenderer.SetPosition(0, transform.position);

        Vector2[] focusedHeadlightBoxPoints = focusedHeadlight.GetFocusedHeadlightBoxPoints();

        bool hitHeadlight = false;

        //focused headlight is turned on
        if (focusedHeadlightBoxPoints[0] != Vector2.zero && focusedHeadlightBoxPoints[1] != Vector2.zero)
        {
            Vector2 intersection = LineIntersection.FindIntersectionPoint(focusedHeadlightBoxPoints[0], focusedHeadlightBoxPoints[1], lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));
            if (intersection == Vector2.zero)
                return;
            //limit intersection point to the area of box
            float yTopBoxCoord = focusedHeadlightBoxPoints[0].y;
            float yBottomBoxCoord = focusedHeadlightBoxPoints[1].y;

            float xLeftBoxCoord = focusedHeadlightBoxPoints[0].x < focusedHeadlightBoxPoints[1].x ? focusedHeadlightBoxPoints[0].x : focusedHeadlightBoxPoints[1].x;
            float xRightBoxCoord = focusedHeadlightBoxPoints[0].x >= focusedHeadlightBoxPoints[1].x ? focusedHeadlightBoxPoints[0].x : focusedHeadlightBoxPoints[1].x;

            if (intersection.x <= xRightBoxCoord && intersection.x >= xLeftBoxCoord && intersection.y <= yTopBoxCoord && intersection.y >= yBottomBoxCoord)
            {
                lineRenderer.SetPosition(1, intersection);
                hitHeadlight = true;
            }
        }

        if(!hitHeadlight)
        {
            Vector2 linePos2 = new Vector2();
            hit = Physics2D.Raycast(transform.position, -transform.up, 10, groundLayer);

            trackOffsetX = lineRenderer.GetPosition(1).x > player.transform.position.x ? 1f : -1f;

            linePos2.y = Mathf.SmoothDamp(lineRenderer.GetPosition(1).y, hit.point.y, ref velocityVertical, smoothDampTimeVertical);
            linePos2.x = Mathf.SmoothDamp(lineRenderer.GetPosition(1).x, player.transform.position.x - trackOffsetX, ref velocityHorizontal, smoothDampTimeHorizontal);

            lineRenderer.SetPosition(1, linePos2);
        }
    }

    private void DamagePlayer()
    {
        hit = Physics2D.Raycast(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0), Vector2.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1)), playerLayer);
        if(hit)
        {
            if (damageCD > 0)
                return;
            player.GetDamaged(damage);
            damageCD = damageCDTotal;
        }
    }

    //number of laser animation repeats, 1 repeat = 1 sec
    public void InitializeLaser(float duration, float laserThickness, MageBoss caller)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = laserThickness;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
        this.duration = duration;
        this.caller = caller;
        attackFinished = false;
    }
}
