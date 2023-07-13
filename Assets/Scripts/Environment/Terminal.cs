using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Terminal : MonoBehaviour
{
    private IsPlayerInRange isPlayerInRange;

    public event Action OnCallElevator;

    [SerializeField]
    private Elevator elevator;

    [SerializeField]
    private Light2D light;

    private bool playerInRange;

    private void Awake()
    {
        isPlayerInRange = GetComponent<IsPlayerInRange>();
    }
    private void Start()
    {
        isPlayerInRange.OnPlayerInRange += IsPlayerInRange_OnPlayerInRange;
        InputManager.Instance.OnInteract += Instance_OnInteract;
        elevator.OnArrived += Elevator_OnArrived;
    }

    private void Elevator_OnArrived()
    {
        light.intensity = 2;
    }

    private void Instance_OnInteract()
    {
        if(playerInRange)
        {
            light.intensity = 5;
            OnCallElevator?.Invoke();
        }
    }

    private void IsPlayerInRange_OnPlayerInRange(bool obj)
    {
        playerInRange = obj;
    }
}
