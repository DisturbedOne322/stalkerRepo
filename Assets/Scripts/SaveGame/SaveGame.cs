using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(IsPlayerInRange))]
public class SaveGame : MonoBehaviour
{
    [SerializeField]
    public int uniqueSavePointID;
    private IsPlayerInRange isPlayerInRange;
    private DisplayIcon displayIcon;

    private bool playerInRange;

    private void Start()
    {
        isPlayerInRange = GetComponent<IsPlayerInRange>();
        InputManager.Instance.OnInteract += Instance_OnInteract;
        isPlayerInRange.OnPlayerInRange += IsPlayerInRange_OnPlayerInRange;
    }

    private void IsPlayerInRange_OnPlayerInRange(bool obj)
    {
        playerInRange = obj;
    }

    private void Instance_OnInteract()
    {
        if(playerInRange)
        {
            Debug.Log("Saved");
            PlayerMovement player = GameManager.Instance.GetPlayerReference();

            SaveData saveData = new SaveData();
            saveData.savePoint = uniqueSavePointID;
            saveData.playerHealth = player.HealthPoints;
            saveData.bulletAmount = player.GetComponentInChildren<Shoot>().currentBulletNum;

            string SaveDataJSON = JsonUtility.ToJson(saveData);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveData.json", SaveDataJSON);

            Debug.Log("Saved");
            Debug.Log(SaveDataJSON);
        }
    }
}
