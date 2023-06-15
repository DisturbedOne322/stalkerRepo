using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HellHoundSoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField]
    private HellHoundAudioClipsSO hellHoundAudioClipsSO;

    private HellHound hellHound;

    [SerializeField]
    private PlayerInRange playerDetection;

    private PlayerMovement player;

    //the maximum distance when something can be heard, the volume is 0 at more distance
    private float maxVolumeDistance = 20f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameManager.Instance.GetPlayerReference();

        hellHound = GetComponent<HellHound>();
        playerDetection.OnPlayerInRange += PlayerDetection_OnPlayerInRange;
        hellHound.OnAggressiveStateChange += HellHound_OnAggressiveStateChange;
        hellHound.OnHellHoundAttack += HellHound_OnHellHoundAttack;
        hellHound.OnSuccessfulHit += HellHound_OnSuccessfulHit;
    }
    private void Update()
    {
        audioSource.volume = DynamicSoundVolume.GetDynamicVolume(maxVolumeDistance, Vector2.Distance(transform.position, player.transform.position));

        //audioSource.volume = Mathf.Clamp(1 - Vector2.Distance(transform.position,player.transform.position) / maxVolumeDistance,0, 1);
    }
    private void HellHound_OnSuccessfulHit()
    {
        audioSource.PlayOneShot(hellHoundAudioClipsSO.AttackAudioClipArray[Random.Range(0, hellHoundAudioClipsSO.AttackAudioClipArray.Length)], audioSource.volume);
    }

    private void HellHound_OnHellHoundAttack()
    {
        audioSource.PlayOneShot(hellHoundAudioClipsSO.BarkAudioClipArray[Random.Range(0, hellHoundAudioClipsSO.BarkAudioClipArray.Length)], audioSource.volume);
    }

    private void HellHound_OnAggressiveStateChange()
    {
        //audioSource.Stop();
    }

    private void PlayerDetection_OnPlayerInRange(PlayerMovement player)
    {
        audioSource.clip = hellHoundAudioClipsSO.GrowlThenBarkAudioClip;
        audioSource.Play();
    }
}
