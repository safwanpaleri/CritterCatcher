using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private BreakPoints breakPoints;
    [SerializeField] private GameObject postprocess;

    public void EnableGameOverScreen()
    {
        gameOverScreen.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        postprocess.SetActive(true);
        FindAnyObjectByType<FirstPersonController>().TurnPlayerControlsOff();
        FindAnyObjectByType<FirstPersonController>().TurnOffPauseControl();
        breakPoints.SetTextPoints();
    }

    public void DisableGameOverScreen()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        postprocess.SetActive(false);
        FindAnyObjectByType<FirstPersonController>().TurnPlayerControlsOn();
        FindAnyObjectByType<FirstPersonController>().TurnOnPauseControl();
        BreakPoints.ResetPoints();
        gameOverScreen.SetActive(false);
    }
    //play again, main menu, exit

    public void PlayAgain()
    {
        DisableGameOverScreen();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Mainmenu()
    {
        DisableGameOverScreen();
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
