using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MageBossBaseAttack
{
    public event Action<MageBoss> OnAttackFinished;
    [SerializeField]
    private LineRenderer lineRenderer;
    private Material laserMaterial;

    [SerializeField]
    private Transform shootPoint;

    private RaycastHit2D hit;

    [SerializeField]
    private LayerMask groundLayer;

    private PlayerMovement player;

    private float smoothDampTimeVertical = 0.5f;
    private float smoothDampTimeHorizontal = 2f;
    private float velocityHorizontal = 0;
    private float velocityVertical = 0;

    private float duration;

    private bool attackFinished = true;

    private MageBoss caller;


    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackFinished)
            return;

        duration -= Time.deltaTime;
        if(duration < 0)
        {
            OnAttackFinished?.Invoke(caller);
            attackFinished = true;
        }

        hit = Physics2D.Raycast(transform.position, -transform.up, 10, groundLayer);        
        lineRenderer.SetPosition(0, transform.position);
        Vector2 linePos2 = new Vector2();

        linePos2.y = Mathf.SmoothDamp(lineRenderer.GetPosition(1).y, hit.point.y, ref velocityVertical, smoothDampTimeVertical);
        linePos2.x = Mathf.SmoothDamp(lineRenderer.GetPosition(1).x, player.transform.position.x, ref velocityHorizontal, smoothDampTimeHorizontal);
        lineRenderer.SetPosition(1, linePos2);
    }
                                //number of laser animation repeats, 1 repeat = 1 sec
    public void InitializeLaser(float duration, float laserThickness, MageBoss caller)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetWidth(laserThickness,laserThickness);
        this.duration = duration;
        this.caller = caller;
        attackFinished = false;
    }
}
