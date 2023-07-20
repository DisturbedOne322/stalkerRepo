using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class LoadData : MonoBehaviour
{
    public static LoadData Instance;

    private SaveData saveData;
    public event Action<int> OnGameLoaded;

    private bool saveDataExists = false;

    [SerializeField]
    private Transform initialSpawnPoint;

    [SerializeField]
    private GameObject loadScreen;
    private Animator loadScreenAnimator;
    private const string LOAD_SCREEN_TRIGGER = "OnLoad";

    private PlayerMovement player;

    private int LastCheckpointID;

    private void Awake()
    {
        Instance = this;
        loadScreenAnimator = loadScreen.GetComponent<Animator>();
        LoadSaveData();
    }

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.05f);
        player = GameManager.Instance.GetPlayerReference();
        player.OnPlayerDied += Player_OnPlayerDied;

        LoadGame(player);
    }

    private void Player_OnPlayerDied()
    {
        loadScreenAnimator.SetTrigger(LOAD_SCREEN_TRIGGER);
        StartCoroutine(LoadLastCheckpointAfterDeath());
    }

    private void LoadGame(PlayerMovement player)
    {
        OnGameLoaded?.Invoke(LastCheckpointID);
        if (saveDataExists)
        {
            GameObject[] savePoints = GameObject.FindGameObjectsWithTag("SavePoint");
            for (int i = 0; i < savePoints.Length; i++)
            {
                if (savePoints[i].GetComponent<SaveGame>().uniqueSavePointID == saveData.savePoint)
                {
                    player.GetComponentInChildren<Shoot>().currentBulletNum = saveData.bulletAmount;
                    player.transform.position = savePoints[i].transform.position;
                }
            }
        }
        else
        {
            player.transform.position = initialSpawnPoint.position;
        }

    }

    private IEnumerator LoadLastCheckpointAfterDeath()
    {
        yield return new WaitForSeconds(4.3f);

        player.RestoreFullHealth();
        LoadSaveData();
        LoadGame(player);
    }

    private void LoadSaveData()
    {
        try
        {
            string data = System.IO.File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
            saveData = JsonUtility.FromJson<SaveData>(data);
            LastCheckpointID = saveData.savePoint;
            saveDataExists = true;

        }
        catch
        {
            Debug.LogError("Save does not exist");
            LastCheckpointID = -1;
        }
    }
}
