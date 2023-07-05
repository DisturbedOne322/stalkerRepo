using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject loadScreen;

    [SerializeField]
    private GameObject optionsWindow;

    [SerializeField]
    private Animator playerMainMenuAnimator;

    private const string PLAYER_PLAY_ANIM = "Base Layer.PlayerMainMenuPlay";
    private const string PLAYER_EXIT_ANIM = "Base Layer.PlayerMainMenuExit";

    private readonly float exitAnimDuration = 6;
    private readonly float playAnimDuration = 4;

    public static bool playerActed { get; private set; }

    private void Start()
    {
        playerActed = false;
    }

    public void OnExitPressed()
    {
        playerMainMenuAnimator.Play(PLAYER_EXIT_ANIM);
        playerActed = true;
        StartCoroutine(ExitGame(exitAnimDuration));
    }

    public void OnPlayPressed()
    {
        playerMainMenuAnimator.Play(PLAYER_PLAY_ANIM);
        playerActed = true;
        StartCoroutine(PlayGame(playAnimDuration));
    }

    private IEnumerator ExitGame(float delay)
    {
        loadScreen.SetActive(true);
        yield return new WaitForSeconds(delay);
        Application.Quit();
    }

    private IEnumerator PlayGame(float delay)
    {
        loadScreen.SetActive(true);
        yield return new WaitForSeconds(delay);
        LoadScene();
    }

    private void LoadScene()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    public void OpenOptionsWindow()
    {
        optionsWindow.SetActive(true);
    }
}
