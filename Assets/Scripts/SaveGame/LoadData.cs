using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class LoadData : MonoBehaviour
{
    private SaveData saveData;

    private bool saveDataExists = false;

    [SerializeField]
    private Transform initialSpawnPoint;

    [SerializeField]
    private GameObject loadScreen;
    private Animator loadScreenAnimator;
    private const string LOAD_SCREEN_TRIGGER = "OnLoad";

    private PlayerMovement player;

    private void Awake()
    {
        loadScreenAnimator = loadScreen.GetComponent<Animator>();
        LoadSaveData();
    }

    // Start is called before the first frame update
    void Start()
    {
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
        if (saveDataExists)
        {
            GameObject[] savePoints = GameObject.FindGameObjectsWithTag("SavePoint");
            for (int i = 0; i < savePoints.Length; i++)
            {
                if (savePoints[i].GetComponent<SaveGame>().uniqueSavePointID == saveData.savePoint)
                {
                    player.GetComponentInChildren<Shoot>().currentBulletNum = saveData.bulletAmount;
                    player.transform.position = savePoints[i].transform.position;
                    return;
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
            saveDataExists = true;

        }
        catch
        {
            Debug.LogError("Save does not exist");
        }
    }
}
