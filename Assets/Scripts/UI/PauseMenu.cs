using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject postprocess;
    [SerializeField] private LevelMusic levelMusic;

    public UnityEvent paused;
    public UnityEvent resumed;


    public void EnablePauseScreen()
    {
        pauseScreen.SetActive(true);
        paused.Invoke();
        postprocess.SetActive(true);
    }

    public void DisablePauseScreen()
    {
        pauseScreen.SetActive(false);
        resumed.Invoke();
        postprocess.SetActive(false);
    }

    public void OnPauseButtonPressed()
    {
        if(levelMusic == null)
            levelMusic = FindAnyObjectByType<LevelMusic>();
        levelMusic.m_AudioSource.Pause();
        Time.timeScale = 0f;
        EnablePauseScreen();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void OnResumeButtonPressed()
    {
        levelMusic.m_AudioSource.Play();
        DisablePauseScreen();
        Time.timeScale = 1.0f;
        FindAnyObjectByType<PlayerInputHandler>().SetPausePressed(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnExitButtonPressed()
    {
        Application.Quit();
    }
}
