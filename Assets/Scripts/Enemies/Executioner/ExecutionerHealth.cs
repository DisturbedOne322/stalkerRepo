using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class ExecutionerHealth : MonoBehaviour, IDamagable
{
    private int healthPoint;

    [SerializeField]
    private int maxHealth;

    private bool underLight = false;

    public void TakeDamage(int damage)
    {
        if(underLight)
        {
            healthPoint -= damage;
            if (healthPoint <= 0)
                OnDeath?.Invoke();
        }
    }

    public event Action OnDeath;

    private void OnEnable()
    {
        healthPoint = maxHealth;
    }

    public void SetHealthTo1()
    {
        healthPoint = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        ExecutionerVisuals executionerVisuals = GetComponent<ExecutionerVisuals>();
        executionerVisuals.OnLighten += ExecutionerVisuals_OnLighten;
    }

    private void ExecutionerVisuals_OnLighten(bool obj)
    {
        underLight = obj;
    }
}
