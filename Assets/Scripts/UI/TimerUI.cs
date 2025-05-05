using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    [SerializeField] RectTransform carUI;
    [SerializeField] Image carImage;
    [SerializeField] RectTransform fillUpUI;
    [SerializeField] RectTransform endPointOfFill;
    [SerializeField] List<Sprite> carImages = new List<Sprite>();
    [SerializeField][Range(0, 10)] float animationFlipbookSpeed = 0.5f;

    [SerializeField] TextMeshProUGUI timerText;

    public void UpdateUIPercentage(float percentageOf1)
    {
        fillUpUI.localScale = new Vector2(percentageOf1, 1);
        carUI.position = endPointOfFill.position;
    }
    public void SetTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);


        timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
    }
    public void Start()
    {
        StartCoroutine(AnimateCar());
    }   

    private IEnumerator AnimateCar()
    {
        int index = 0;
        while (true)
        {
            carImage.sprite = carImages[index];
            index++;
            if (index == carImages.Count)
            {
                index = 0;
            }
            yield return new WaitForSeconds(animationFlipbookSpeed);
        }
    }
}
