using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainScreen;
    [SerializeField] private GameObject howToPlayScreen;
    [SerializeField] private CutscenePlayer cutscenePlayer;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void EnableMainScreen()
    {
        mainScreen.SetActive(true);
    }

    public void DisableMainScreen()
    {
        mainScreen.SetActive(false);
    }

    public void OnHowToPlayPressed()
    {
        howToPlayScreen.SetActive(true);
        DisableMainScreen();
    }

    public void OnReturnPressed()
    {
        howToPlayScreen.SetActive(false);
        EnableMainScreen();
    }
    public void OnPlayButtonPressed()
    {
        DisableMainScreen();
        cutscenePlayer.PlayCutSceneImages();
    }


    public void OnExitButtonPressed()
    {
        Application.Quit();
    }

}
