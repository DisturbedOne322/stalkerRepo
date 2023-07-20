using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using static UnityEditor.PlayerSettings;

public class EnemySpawnManager : MonoBehaviour
{

    #region Tentacles
    [Header("Tentacle")]
    [SerializeField]
    private GameObject tentaclePrefab;
    #endregion
    #region Executioner
    [Header("Executioner")]
    [SerializeField]
    private GameObject executionerPrefab;
    private GameObject executioner;

    [Header("Hell hound")]
    #region Hell hound
    [SerializeField]
    private GameObject hellHoundPrefab;
    [SerializeField]
    private GameObject corpsePrefab;
    #endregion

    #endregion
    #region Executiner Mini Boss Fight
    [Header("Executioner Mini Boss Fight")]
    [SerializeField]
    private ExecutionerMiniBossFightAreaTrigger executionerMiniBossFightTriggerArea;
    public event Action OnBossFightStarted;
    public event Action OnBossFightFinished;
    #endregion
    #region Ghost
    [Header("Ghost, must be only 1 in a map section")]

    [SerializeField]
    private GameObject ghostPrefab;
    private GameObject ghost;
    #endregion
    #region Teleporter
    [Header("Teleporter, must be only 1 in a map section")]

    [SerializeField]
    private GameObject teleportedPrefab;
    private GameObject teleporter;
    #endregion
    #region Player Fake
    [Header("Player fake")]
    [SerializeField]
    private GameObject fakePrefab;
    #endregion

    List<GameObject> spawnedEnemies = new List<GameObject>();
    List<GameObject> spawnedExecutioners = new List<GameObject>();

    #region Checkpoints
    [SerializeField]
    private Checkpoints[] checkpoints;
    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        LoadData.Instance.OnGameLoaded += LoadData_OnGameLoaded;

        executioner = Instantiate(executionerPrefab, Vector3.zero, Quaternion.identity);
        executioner.SetActive(false);

        ghost = Instantiate(ghostPrefab, Vector3.zero, Quaternion.identity);
        ghost.SetActive(false);

        teleporter = Instantiate(teleportedPrefab, Vector3.zero, Quaternion.identity);
        teleporter.SetActive(false);


        //executionerMiniBossFightTriggerArea.OnPlayerStartedBossFight += ExecutionerMiniBossFightTriggerArea_OnPlayerStartedBossFight; ;
    }

    private bool gameLoaded = false;

    private void LoadData_OnGameLoaded(int lastCheckpointID)
    {
        if(gameLoaded)
        {
            RespawnEnemies();
        }    
        gameLoaded = true;
        // + 1 to spawn enemies for the new checkpoint areas only
        lastCheckpointID = -1;
        for (int i = lastCheckpointID + 1; i < checkpoints.Length; i++)
        {
            SpawnEnemies(i);
        }
    }

    private void RespawnEnemies()
    {
        for(int i = spawnedEnemies.Count - 1; i >= 0;i--)
        {
            Destroy(spawnedEnemies[i]);
            spawnedEnemies.RemoveAt(i);
        }

        //foreach(GameObject enemy in spawnedExecutioners)
        //{
        //    Destroy(enemy);
        //}

        executioner = Instantiate(executionerPrefab, Vector3.zero, Quaternion.identity);
        executioner.SetActive(false);

        ghost = Instantiate(ghostPrefab, Vector3.zero, Quaternion.identity);
        ghost.SetActive(false);

        teleporter = Instantiate(teleportedPrefab, Vector3.zero, Quaternion.identity);
        teleporter.SetActive(false);
    }

    private void SpawnEnemies(int i)
    {
        spawnedEnemies.AddRange(SpawnHellHoundPack(checkpoints[i].GetComponentsInChildren<HellHoundParentObject>()));
        spawnedEnemies.AddRange(SpawnAttackTentacles(checkpoints[i].GetComponentsInChildren<TentacleAttackParentObject>()));
        spawnedEnemies.AddRange(SpawnIdleTentacles(checkpoints[i].GetComponentsInChildren<TentaclesIdleParentObject>()));
        ListenToOnPlayerEnterExecutionerTriggers(checkpoints[i].GetComponentsInChildren<ExecutionerTriggerArea>());
        spawnedEnemies.Add(SpawnGhost(checkpoints[i].GetComponentInChildren<GhostParentObject>()));
        spawnedEnemies.Add(SpawnTeleporter(checkpoints[i].GetComponentInChildren<TeleporterParentObject>()));
        spawnedEnemies.AddRange(SpawnPlayerFakes(checkpoints[i].GetComponentsInChildren<PlayerFakeParentObject>()));
    }

    private void ListenToOnPlayerEnterExecutionerTriggers(ExecutionerTriggerArea[] exec)
    {
        if (exec != null)
        {
            for(int i = 0; i < exec.Length;i++)
                exec[i].OnPlayerEnteredTriggerArea += Exec_OnPlayerEnteredTriggerArea;
        }
    }
    private void Exec_OnPlayerEnteredTriggerArea(Transform parent)
    {
        executioner.SetActive(true);
        executioner.transform.position = parent.position;
    }

    private List<GameObject> SpawnHellHoundPack(HellHoundParentObject[] parent)
    {
        if (parent == null)
            return null;

        List<GameObject> spawned = new List<GameObject>();

        for(int i = 0; i < parent.Length;i++)
        {
            GameObject corpse = Instantiate(corpsePrefab, new Vector3(0,0,0), Quaternion.identity);
            corpse.transform.position = parent[i].transform.position;
            corpse.transform.parent = parent[i].transform;

            spawned.Add(corpse);

            int numOfDogs = UnityEngine.Random.Range(1, 4);

            int side = 1;

            float[] occipiedPositions = new float[numOfDogs];
            float spawnOffset = 0.5f;

            for (int j = 0; j < numOfDogs; j++)
            {
                float offset;
                do
                {
                    offset = UnityEngine.Random.Range(-1.5f, -0.6f) * side;
                } while (IsPositionOccupied(offset, occipiedPositions, spawnOffset));

                occipiedPositions[j] = offset;

                GameObject temp = Instantiate(hellHoundPrefab, parent[i].transform);
                temp.transform.position += new Vector3(offset, 0, 0);

                spawned.Add(temp);

                Vector3 scale = temp.transform.localScale;
                scale.x *= side;
                temp.transform.localScale = scale;

                side *= -1;
            }
        }

        return spawned;
    }

    private bool IsPositionOccupied(float position, float[] positions, float offset)
    {
        for(int i = 0; i < positions.Length;i++)
        {
            if(position < positions[i] + offset && position > positions[i] - offset)
            {
                return true;
            }
        }

        return false;
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
            exec.GetComponent<ApproachPlayer>().SetSpeed(0.15f);
            exec.GetComponent<DissolveOnDeath>().SetDissolveTicktime(0.015f);

            spawnedExecutioners.Add(exec);

            delayBetweenSpawns -= 0.05f;
            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        yield return new WaitForSeconds(8);
        OnBossFightFinished?.Invoke();
    }

    private List<GameObject> SpawnIdleTentacles(TentaclesIdleParentObject[] parent)
    {
        if(parent == null) 
            return null;

        List<GameObject> spawned = new List<GameObject>();  

        tentaclePrefab.GetComponent<TentacleStateManager>().initialState = TentacleStateManager.InitialState.idleState;

        for (int i = 0; i < parent.Length; i++)
        {
            spawned.Add(Instantiate(tentaclePrefab, parent[i].transform));
        }

        return spawned;
    }

    private List<GameObject> SpawnAttackTentacles(TentacleAttackParentObject[] parent)
    {
        if (parent == null)
            return null;

        List<GameObject> spawned = new List<GameObject>();

        tentaclePrefab.GetComponent<TentacleStateManager>().initialState = TentacleStateManager.InitialState.attackState;

        for (int i = 0; i < parent.Length; i++)
        {
            spawned.Add(Instantiate(tentaclePrefab, parent[i].transform));
        }
        return spawned;
    }

    private GameObject SpawnGhost(GhostParentObject parent)
    {
        if (parent == null)
            return null;

        ghost.SetActive(true);
        ghost.transform.parent = parent.transform;
        ghost.transform.position = parent.transform.position;

        return ghost;
    }

    private GameObject SpawnTeleporter(TeleporterParentObject parent)
    {
        if (parent == null)
            return null;

        teleporter.SetActive(true);
        teleporter.transform.parent = parent.transform;
        teleporter.transform.position = parent.transform.position;

        return teleporter;
    }

    private List<GameObject> SpawnPlayerFakes(PlayerFakeParentObject[] parent)
    {
        if (parent == null)
            return null;

        List<GameObject> spawned = new List<GameObject>();
        for(int i = 0;i < parent.Length;i++)
        {
            GameObject temp = Instantiate(fakePrefab, parent[i].transform);
            temp.SetActive(true);
            spawned.Add(temp);
        }

        return spawned;
    }
}
