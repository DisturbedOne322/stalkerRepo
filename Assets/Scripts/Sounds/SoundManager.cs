using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField]
    private AudioSource mainAudioSource;

    [SerializeField] 
    private AudioSource focusedLightAudioSource;

    [SerializeField] 
    private AudioSource focusedLightCoolingAudioSource;


    [SerializeField]
    private AudioClipsSO audioClipsSO;

    [SerializeField]
    private AudioClip qteAudioClip;

    [SerializeField]
    private AudioClip[] headlightBrokenAudioClipArray;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        mainAudioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        QTE.instance.OnQTEStart += QTE_OnQTEStart;
    }

    private void QTE_OnQTEStart()
    {
        mainAudioSource.PlayOneShot(qteAudioClip);
    }

    public void PlayShootSound()
    {
        mainAudioSource.PlayOneShot(audioClipsSO.shootSound);
    }
    public void PlayMagInSound()
    {
        mainAudioSource.PlayOneShot(audioClipsSO.magInSound);
    }
    public void PlayMagOutSound()
    {
        mainAudioSource.PlayOneShot(audioClipsSO.magOutSound);
    }
    public void PlayReloadSound()
    {
        mainAudioSource.PlayOneShot(audioClipsSO.reloadSound);
    }
    public void PlayGetHurtSound()
    {
        mainAudioSource.PlayOneShot(audioClipsSO.getHurtSound);
    }

    public void PlayBoneCrackSound()
    {
        mainAudioSource.PlayOneShot(audioClipsSO.boneCrackSound);
    }

    public void PlayFocusedLightSound(bool isFocusedLightEnabled)
    {
        if (isFocusedLightEnabled)
        {
            focusedLightCoolingAudioSource.Stop();
            focusedLightAudioSource.Play();
        }
        else
        {
            focusedLightAudioSource.Stop();
            focusedLightCoolingAudioSource.Play();
        }
    }

    public void PlayNoAmmoSound()
    {
        mainAudioSource.PlayOneShot(audioClipsSO.noAmmoSound);
    }

    public void PlayStepsSound()
    {
        mainAudioSource.PlayOneShot(audioClipsSO.stepSound[Random.Range(0,audioClipsSO.stepSound.Length)]);
    }

    public void PlayBrokenHeadlightSound()
    {
        mainAudioSource.PlayOneShot(headlightBrokenAudioClipArray[Random.Range(0,headlightBrokenAudioClipArray.Length)]);
    }

    public void PlayExhaleSound(float volume)
    {
        mainAudioSource.PlayOneShot(audioClipsSO.exhaleSound, volume);
    }
    public void PlayInhaleSound(float volume)
    {
        mainAudioSource.PlayOneShot(audioClipsSO.inhaleSound, volume);
    }
}
