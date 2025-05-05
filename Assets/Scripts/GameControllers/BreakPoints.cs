using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BreakPoints : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private static int points;

    public static void AddPoints(int point)
    {
        points += point;
    }

    public static int GetPoints()
    {
        return points;
    }

    public static void ResetPoints()
    {
        points = 0;
    }

    public void SetTextPoints()
    {
        text.text = "Destruction Caused: " + points.ToString();
    }


}
