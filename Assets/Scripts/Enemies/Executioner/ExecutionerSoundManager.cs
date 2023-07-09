using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionerSoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    private PlayerMovement player;

    [SerializeField]
    private AudioClip sliceSound1;
    [SerializeField] 
    private AudioClip sliceSound2;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameManager.Instance.GetPlayerReference();
    }


    private void PlaySliceSound1()
    {
        audioSource.PlayOneShot(sliceSound1);
    }
    private void PlaySliceSound2()
    {
        audioSource.PlayOneShot(sliceSound2);
    }
}
