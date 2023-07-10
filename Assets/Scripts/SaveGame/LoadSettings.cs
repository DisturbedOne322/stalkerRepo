using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class LoadSettings : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    private SettingsSaveData saveData;

    private const string MASTER_VOLUME = "Master Volume";
    private const string EFFECTS_VOLUME = "Effects Volume";
    private const string MUSIC_VOLUME = "Music Volume";

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            string data = System.IO.File.ReadAllText(Application.persistentDataPath + "/Settings.json");
            saveData = JsonUtility.FromJson<SettingsSaveData>(data);

            audioMixer.SetFloat(MASTER_VOLUME, saveData.masterVolume);
            audioMixer.SetFloat(EFFECTS_VOLUME, saveData.effectsVolume);
            audioMixer.SetFloat(MUSIC_VOLUME, saveData.musicVolume);
        }
        catch
        {
            Debug.LogError("Settings save data does not exist");
            audioMixer.SetFloat(MASTER_VOLUME, 1);
            audioMixer.SetFloat(EFFECTS_VOLUME, 1);
            audioMixer.SetFloat(MUSIC_VOLUME, 1);
        }
    }
}
