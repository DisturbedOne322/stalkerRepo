using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Elevator : MonoBehaviour
{
    public event Action OnArrived;
    public event Action OnDeparted;

    private PlayerMovement player;

    [SerializeField]
    private Terminal terminalUpper;
    [SerializeField]
    private Terminal terminalButtom;

    [SerializeField]
    private Transform endPoint;

    [SerializeField]
    private Transform startPoint;

    private float elevatorSmDampVelocity;
    private readonly float elevatorSmoothTime = 6;

    private Vector3 destination;

    private bool moving = false;

    // Start is called before the first frame update
    void Start()
    {
        terminalUpper.OnCallElevator += TerminalUpper_OnCallElevator;
        terminalButtom.OnCallElevator += TerminalButtom_OnCallElevator;
        player = GameManager.Instance.GetPlayerReference();
    }

    private void TerminalButtom_OnCallElevator()
    {
        StartCoroutine(CallElevator(endPoint.position));
        OnDeparted?.Invoke();
    }

    private void TerminalUpper_OnCallElevator()
    {
        StartCoroutine(CallElevator(startPoint.position));
        OnDeparted?.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (moving)
            return;

        OnDeparted?.Invoke();

        destination = Vector2.Distance(transform.position, startPoint.position) 
            > Vector2.Distance(transform.position, endPoint.position) ? 
            startPoint.position : endPoint.position;

        StartCoroutine(CallElevator(destination));

        player.transform.parent = this.transform;

        moving = true;
    }

    private IEnumerator CallElevator(Vector3 destination)
    {
        yield return new WaitForSeconds(1.5f);
        while (Vector2.Distance(transform.position, destination) > 5f)
        {
            Vector2 newPosition = transform.position;
            newPosition.y = Mathf.SmoothDamp(transform.position.y, destination.y, ref elevatorSmDampVelocity, elevatorSmoothTime);
            transform.position = newPosition;
            yield return null;
        }
        player.transform.parent = null;
        OnArrived?.Invoke();
        moving = false;
    }
}
