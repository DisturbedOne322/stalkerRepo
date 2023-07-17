using System.Collections;
using System.Collections.Generic;
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
    private GameObject executionerParent;
    [SerializeField]
    private GameObject executionerPrefab;
    private GameObject executioner;
    private ExecutionerTriggerArea[] executionerTriggerAreas;
    #endregion

    bool wtf = false;
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
        executionerTriggerAreas = executionerParent.GetComponentsInChildren<ExecutionerTriggerArea>();
        executioner = Instantiate(executionerPrefab, Vector3.zero , Quaternion.identity);
        executioner.SetActive(false);
        for(int i = 0; i < executionerTriggerAreas.Length;i++)
        {
            executionerTriggerAreas[i].OnPlayerEnteredTriggerArea += EnemySpawnManager_OnPlayerEnteredTriggerArea;
        }
        #endregion
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
