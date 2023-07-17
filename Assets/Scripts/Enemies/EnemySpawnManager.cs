using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Tentacle")]
    #region Tentacles
    [SerializeField]
    private GameObject tentaclePrefab;

    [SerializeField] 
    private GameObject tentacleIdleStateParent;
    [SerializeField]
    private GameObject tentacleAttackStateParent;

    private Transform[] tentacleIdleSpawnPoints;
    private Transform[] tentacleAttackSpawnPoints;
    #endregion
    [Header("Executioner")]
    #region Executioner
    [SerializeField]
    private GameObject executionerParentTransform;
    [SerializeField]
    private GameObject executionerPrefab;
    private GameObject executioner;
    private ExecutionerTriggerArea[] executionerTriggerAreas;
    #endregion
    [Header("Executioner Mini Boss Fight")]
    #region Executiner Mini Boss Fight
    [SerializeField]
    private ExecutionerMiniBossFightAreaTrigger executionerMiniBossFightTriggerArea;
    public event Action OnBossFightStarted;
    public event Action OnBossFightFinished;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        #region TentacleSpawn
        tentacleIdleSpawnPoints = tentacleIdleStateParent.GetComponentsInChildren<Transform>();
        tentacleAttackSpawnPoints = tentacleAttackStateParent.GetComponentsInChildren<Transform>();

        SpawnIdleTentacles();
        SpawnAttackTentacles();
        #endregion

        #region Executiner Subscribe to trigger area events
        executionerTriggerAreas = executionerParentTransform.GetComponentsInChildren<ExecutionerTriggerArea>();
        executioner = Instantiate(executionerPrefab, Vector3.zero , Quaternion.identity);
        executioner.SetActive(false);
        for(int i = 0; i < executionerTriggerAreas.Length;i++)
        {
            executionerTriggerAreas[i].OnPlayerEnteredTriggerArea += EnemySpawnManager_OnPlayerEnteredTriggerArea;
        }
        #endregion

        executionerMiniBossFightTriggerArea.OnPlayerStartedBossFight += ExecutionerMiniBossFightTriggerArea_OnPlayerStartedBossFight; ;
    }

    private bool bossFightStarted = false;

    private void ExecutionerMiniBossFightTriggerArea_OnPlayerStartedBossFight(Transform[] spawnPositions)
    {
        if(bossFightStarted)      
            return;
        
        bossFightStarted = true;
        OnBossFightStarted?.Invoke();
        StartCoroutine(SpawnExecutioners(spawnPositions));
    }

    private IEnumerator SpawnExecutioners(Transform[] spawnPositions)
    {
        float delayBetweenSpawns = 4f;
        for(int i = 1; i < spawnPositions.Length; i++)
        {
            GameObject exec = Instantiate(executionerPrefab, spawnPositions[i].position, Quaternion.identity);
            exec.GetComponent<ExecutionerHealth>().SetHealthTo1();
            exec.GetComponent<ApproachPlayer>().SetSpeed(0.2f);
            exec.GetComponent<DissolveOnDeath>().SetDissolveTicktime(0.015f);
            delayBetweenSpawns -= 0.05f;
            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        yield return new WaitForSeconds(8);
        OnBossFightFinished?.Invoke();
    }


    private void EnemySpawnManager_OnPlayerEnteredTriggerArea(Transform parent)
    {
        executioner.SetActive(true);
        executioner.transform.position = parent.position;
    }

    private void SpawnIdleTentacles()
    {
        tentaclePrefab.GetComponent<TentacleStateManager>().initialState = TentacleStateManager.InitialState.idleState;
        for (int i = 1; i < tentacleIdleSpawnPoints.Length; i++)
        {
            GameObject tentacleIdle = Instantiate(tentaclePrefab, tentacleIdleSpawnPoints[i]);
        }
    }

    private void SpawnAttackTentacles()
    {
        tentaclePrefab.GetComponent<TentacleStateManager>().initialState = TentacleStateManager.InitialState.attackState;

        for (int i = 1; i < tentacleAttackSpawnPoints.Length; i++)
        {
            GameObject tentacleIdle = Instantiate(tentaclePrefab, tentacleAttackSpawnPoints[i]);
        }
    }
}
