using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterFly : MonoBehaviour
{
    // Start is called before the first frame update
    public float Flutter = 100;
    
    public bool upordown;
    public SkinnedMeshRenderer wings;


    // Update is called once per frame
    void Update()
    {


        wings.SetBlendShapeWeight(0, Flutter);
        if (Flutter > 99)
        {
            upordown = false;
            
        }
        if (Flutter < 1)
        {
            upordown = true;
            
        }


        if (upordown == true)
        {
            Flutter += 5;
        }
        if (upordown == false)
        {
            Flutter -= 5;
        }

    }
}
