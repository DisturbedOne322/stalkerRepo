using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{

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


    // Start is called before the first frame update
    void Start()
    {
        tentacleIdleSpawnPoints = tentacleIdleStateParent.GetComponentsInChildren<Transform>();
        tentacleAttackSpawnPoints = tentacleAttackStateParent.GetComponentsInChildren<Transform>();

        SpawnIdleTentacles();
        SpawnAttackTentacles();
    }

    // Update is called once per frame
    void Update()
    {
        
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
