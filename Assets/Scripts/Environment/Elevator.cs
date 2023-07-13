using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public event Action OnArrived;
    public event Action OnDeparted;

    private Terminal terminal;    

    [SerializeField]
    private Transform endPoint;

    [SerializeField]
    private Transform startPoint;

    private float elevatorSmDampVelocity;
    private readonly float elevatorSmoothTime = 3;

    // Start is called before the first frame update
    void Start()
    {
        terminal = GetComponentInParent<Terminal>();
        terminal.OnCallElevator += Terminal_OnCallElevator;
    }

    private void Terminal_OnCallElevator()
    {
        StartCoroutine(CallElevator());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnDeparted?.Invoke();
    }

    private IEnumerator CallElevator()
    {
        while (Vector2.Distance(transform.position, startPoint.position) > 0.2f)
        {
            Vector2 newPosition = transform.position;
            newPosition.y = Mathf.SmoothDamp(transform.position.y, startPoint.position.y, ref elevatorSmDampVelocity, elevatorSmoothTime);
            transform.position = newPosition;
            yield return null;
        }
        OnArrived?.Invoke();
    }
}
