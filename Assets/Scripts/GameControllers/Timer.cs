using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

using UnityEngine;
using TMPro;
public class Timer : MonoBehaviour
{
    [SerializeField] float remainingTime;
    public UnityEvent timeReachedZero;
    [SerializeField] TimerUI timerUI;
    float startTime;

    private void Start()
    {
        startTime = remainingTime;
    }
    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            timeReachedZero.Invoke();
            return;
        }

        timerUI.SetTime(remainingTime);
        timerUI.UpdateUIPercentage(1 - (remainingTime / startTime));
    }
}
