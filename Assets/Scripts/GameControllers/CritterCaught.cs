using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CritterCaught : MonoBehaviour{
   
    public TMP_Text caughtText;

    public int critterCount = 5;

    public UnityEvent collectedAll;

    public void Start()
    {
        critterCount = FindObjectsByType<CreatureAI>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
    }
    public void FixedUpdate()
    {
        caughtText.text = "Critters: " + critterCount.ToString();
        if (critterCount <=0) 
        {
            collectedAll.Invoke();
        }
    }

    public void decrementCounter()
    {
        critterCount-=1;
    }

    


}
