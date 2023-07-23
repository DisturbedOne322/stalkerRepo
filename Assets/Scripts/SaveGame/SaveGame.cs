using System;
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

    private CheckCanSaveGame checkCanSaveGame;

    private bool playerInRange;

    public event Action OnDisplayIcon;
    public static event Action OnGameSaved;

    private bool recentlySaved = false;

    private bool canSave;

    private void Start()
    {
        isPlayerInRange = GetComponent<IsPlayerInRange>();
        InputManager.Instance.OnInteract += Instance_OnInteract;
        isPlayerInRange.OnPlayerInRange += IsPlayerInRange_OnPlayerInRange;

        checkCanSaveGame = GetComponent<CheckCanSaveGame>();
        checkCanSaveGame.CanSaveGame += CheckCanSaveGame_CanSaveGame;
    }

    private void CheckCanSaveGame_CanSaveGame(bool canSave)
    {
        this.canSave = canSave;
    }

    private void IsPlayerInRange_OnPlayerInRange(bool obj)
    {
        playerInRange = obj;
        if (!playerInRange)
            recentlySaved = false;
    }

    private void Instance_OnInteract()
    {
        if (!canSave)
            return;

        if (recentlySaved)
            return;

        if (playerInRange)
        {
            
            PlayerMovement player = GameManager.Instance.GetPlayerReference();

            SaveData saveData = new SaveData();
            saveData.savePoint = uniqueSavePointID;
            player.RestoreFullHealth();
            saveData.bulletAmount = player.GetComponentInChildren<Shoot>().currentBulletNum;

            string SaveDataJSON = JsonUtility.ToJson(saveData);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveData.json", SaveDataJSON);

            OnGameSaved?.Invoke();
            OnDisplayIcon?.Invoke();

            recentlySaved = true;
        }
    }
}
