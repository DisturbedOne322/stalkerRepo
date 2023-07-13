using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorFence : MonoBehaviour
{
    [SerializeField]
    private Elevator elevator;

    [SerializeField]
    private Transform loweredPosition;

    [SerializeField]
    private Transform raisedPosition;

    private float fanceSmDampVelocity;
    private readonly float fenceSmoothTime = 1;

    private float fenceLowerDistance;

    // Start is called before the first frame update
    void Start()
    {
        fenceLowerDistance = GetComponent<BoxCollider2D>().bounds.size.y;
        elevator.OnArrived += Elevator_OnArrived;
        elevator.OnDeparted += Elevator_OnDeparted;
    }

    private void Elevator_OnDeparted()
    {
        StartCoroutine(RaiseFence());
    }

    private void Elevator_OnArrived()
    {
        StartCoroutine(LowerFence());
    }

    private IEnumerator LowerFence()
    {
        while (Vector2.Distance(transform.position, loweredPosition.position) > 0.2f)
        {
            Vector2 newPosition = transform.position;
            newPosition.y = Mathf.SmoothDamp(transform.position.y, loweredPosition.position.y, ref fanceSmDampVelocity, fenceSmoothTime);
            transform.position = newPosition;
            yield return null;
        }
    }

    private IEnumerator RaiseFence()
    {
        while (Vector2.Distance(transform.position, raisedPosition.position) > 0.2f)
        {
            Vector2 newPosition = transform.position;
            newPosition.y = Mathf.SmoothDamp(transform.position.y, raisedPosition.position.y, ref fanceSmDampVelocity, fenceSmoothTime);
            transform.position = newPosition;
            yield return null;
        }
    }
}
