using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> Tutorials = new List<GameObject>();
    [SerializeField] private GameObject RightButton;
    [SerializeField] private GameObject LeftButton;
    int currentIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onRightButtonPreseed()
    {
        Tutorials[currentIndex].SetActive(false);
        Tutorials[++currentIndex].SetActive(true);
        if(currentIndex == Tutorials.Count - 1)
        {
            RightButton.SetActive(false);
        }
        if(currentIndex > 0)
            LeftButton.SetActive(true);
    }

    public void onLeftButtonPressed()
    {
        Tutorials[currentIndex].SetActive(false);
        Tutorials[--currentIndex].SetActive(true);
        if (currentIndex == 0)
        {
            LeftButton.SetActive(false);
        }
        if (currentIndex < Tutorials.Count - 1)
            RightButton.SetActive(true);
    }

}
