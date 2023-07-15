using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Elevator))]
public class ElevatorSoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip elevatorArrivedBeep;
    [SerializeField]
    private AudioClip elevatorArrivedScreech;
    [SerializeField]
    private AudioClip elevatorDepartedScreech;
    [SerializeField]
    private AudioClip elevatorDoorsOpen;

    private PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        GetComponent<Elevator>().OnArrived += ElevatorSoundManager_OnArrived;
        GetComponent<Elevator>().OnDeparted += ElevatorSoundManager_OnDeparted;

        player = GameManager.Instance.GetPlayerReference();
    }

    private void Update()
    {
        audioSource.volume = DynamicSoundVolume.GetDynamicVolume(100, Vector2.Distance(transform.position, player.transform.position));
    }

    private void ElevatorSoundManager_OnDeparted()
    {
        audioSource.PlayOneShot(elevatorDepartedScreech);
        audioSource.PlayOneShot(elevatorDoorsOpen);
        audioSource.Play();
    }

    private void ElevatorSoundManager_OnArrived()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(elevatorArrivedScreech);
        audioSource.PlayOneShot(elevatorArrivedBeep);
        audioSource.PlayOneShot(elevatorDoorsOpen);
    }
}
