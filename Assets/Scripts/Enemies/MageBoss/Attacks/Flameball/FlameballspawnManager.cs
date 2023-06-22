using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameballspawnManager : MonoBehaviour
{

    private GameObject[] pool;

    private int waveNumberTotal;
    private int currentWave;
    private float cdBetweenWaves;
    private float cdBetweenWavesTotal;

    private int spawnAmountTotal;
    private int spawnedAmount;

    private float spawnCD;
    private float spawnCDTotal;

    private float scale;

    private float fallSpeed;

    private float flameballSpawnOffset;

    [SerializeField]
    private Transform flameballSpawnStartPos;
    private Vector3 flameballSpawnPos;
    private Vector3 additionalOffset;
    private bool additionalOffsetOption = false;

    [SerializeField]
    private GameObject flameballPrefab;

    private bool attackFinished = true;

    public event Action OnAttackFinished;

    private float animationDelay = 4f;
    private float animationDelayTotal = 4f;


    private void Start()
    {
        flameballSpawnOffset = flameballPrefab.GetComponent<BoxCollider2D>().size.x * 4 + 0.25f;
        pool = new GameObject[20];
        for(int i = 0; i < pool.Length;i++)
        {
            pool[i] = Instantiate(flameballPrefab);
            pool[i].SetActive(false);
        }
    }

    public void InitializeFlameballAttackProperties(int waveNumberTotal, int spawnAmountTotal, float spawnCDTotal, float cdBetweenWaves, float fallSpeed, float scale)
    {
        flameballSpawnPos = flameballSpawnStartPos.position;
        this.waveNumberTotal = waveNumberTotal;
        this.spawnCDTotal = spawnCDTotal;
        this.spawnAmountTotal = spawnAmountTotal;
        this.fallSpeed = fallSpeed;
        this.cdBetweenWaves = cdBetweenWaves;
        this.scale = scale;
        cdBetweenWavesTotal = cdBetweenWaves;
        additionalOffset = Vector3.zero;
        additionalOffsetOption = false;
        attackFinished = false;
    }

    public void InitializeFlameballAttackProperties(int waveNumberTotal, int spawnAmountTotal, float spawnCDTotal, float cdBetweenWaves, float fallSpeed, float scale, bool additionalOffsetOption, Vector3 additionalOffset)
    {
        flameballSpawnPos = flameballSpawnStartPos.position;
        this.waveNumberTotal = waveNumberTotal;
        this.spawnCDTotal = spawnCDTotal;
        this.spawnAmountTotal = spawnAmountTotal;
        this.fallSpeed = fallSpeed;
        this.cdBetweenWaves = cdBetweenWaves;
        this.scale = scale;
        cdBetweenWavesTotal = cdBetweenWaves;
        this.additionalOffset = additionalOffset;
        this.additionalOffsetOption = additionalOffsetOption;
        attackFinished = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (attackFinished)
        {
            return;
        }

        animationDelay -= Time.deltaTime;
        if(animationDelay > 0)
        {
            return;
        }

        spawnCD -= Time.deltaTime;

        if (spawnedAmount >= spawnAmountTotal)
        {
            cdBetweenWaves -= Time.deltaTime;
            if (cdBetweenWaves < 0)
            {
                cdBetweenWaves = cdBetweenWavesTotal;
                flameballSpawnPos = flameballSpawnStartPos.position;
                currentWave++;
                spawnCD = spawnCDTotal;
                if (additionalOffsetOption)
                {
                    flameballSpawnPos.x -= additionalOffset.x * currentWave;
                }
                if (currentWave >= waveNumberTotal)
                {
                    currentWave = 0;
                    attackFinished = true;
                    OnAttackFinished?.Invoke();
                    animationDelay = animationDelayTotal;
                }
                spawnedAmount = 0;
            }

        }

        if (spawnCD > 0)
            return;
        
        spawnCD = spawnCDTotal;
        for(int i = 0;i < pool.Length; i++)
        {
            if (!pool[i].active)
            {
                pool[i].SetActive(true);
                pool[i].GetComponent<Flameball>().SwitchState(new FlameballFallingState());
                pool[i].transform.position = flameballSpawnPos;
                pool[i].GetComponent<Flameball>().Speed = fallSpeed;
                pool[i].gameObject.transform.localScale = Vector3.one *  scale;
                break;
            }
        }
        flameballSpawnPos.x -= flameballSpawnOffset;
        spawnedAmount++;
        
        
    }
}
