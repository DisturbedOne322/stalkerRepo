using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField]
    private Material material;

    private float dissolveAmount = 1;
    private float dissolvePerSec = 0.15f;
    private bool dissolve = false;

    public event Action OnDissolved;

    private void Awake()
    {
        material.SetFloat("_dissolveAmount", 1);
    }

    private void Update()
    {
        DissolveOnDeath();
    }

    public float DissolveOnDeath()
    {
        if (dissolve)
        {
            dissolveAmount -= Time.deltaTime * dissolvePerSec;
            material.SetFloat("_dissolveAmount", dissolveAmount);

            if (dissolveAmount < 0)
            {
                OnDissolved?.Invoke();
                dissolve = false;
            }
        }
        return dissolveAmount;
    }

    public void StartDissolving()
    {
        dissolve = true;
    }
}
