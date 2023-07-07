using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class LoadData : MonoBehaviour
{
    private SaveData saveData;

    private bool saveDataExists = false;

    private void Awake()
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

    // Start is called before the first frame update
    void Start()
    {
        if(saveDataExists)
        {
            GameObject[] savePoints = GameObject.FindGameObjectsWithTag("SavePoint");
            PlayerMovement player = GameManager.Instance.GetPlayerReference();
            player.HealthPoints = saveData.playerHealth;
            player.GetComponentInChildren<Shoot>().currentBulletNum = saveData.bulletAmount;
            for(int i = 0; i < savePoints.Length; i++)
            {
                if (savePoints[i].GetComponent<SaveGame>().uniqueSavePointID == saveData.savePoint) 
                {
                    player.transform.position = savePoints[i].transform.position;
                    return;
                }
            }
        }
    }

}
