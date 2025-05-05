using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameEnd : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FindFirstObjectByType<CritterCaught>().collectedAll.AddListener(EndGame);
        FindFirstObjectByType<Timer>().timeReachedZero.AddListener(EndGame);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        BreakPoints.ResetPoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EndGame()
    {
        //SceneManager.LoadScene(0);
        FindFirstObjectByType<GameOverMenu>().EnableGameOverScreen();
    }
}
