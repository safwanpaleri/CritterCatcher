using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronAnimation : MonoBehaviour
{
    public float liquid = 100;
    public float random = 1.0f;
    public bool upordown;
    public SkinnedMeshRenderer water; 
   

    // Update is called once per frame
    void Update()
    {

       
        water.SetBlendShapeWeight(0, liquid);
        if (liquid > 99)
        {
            upordown = false;
          random =  Random.Range(0.1f, 1.0f);
        }
        if (liquid < 1)
        {
            upordown = true;
            random = Random.Range(0.1f, 1.0f);
        }


        if (upordown == true)
        {
            liquid += random;
        }
        if (upordown == false)
        {
            liquid -= random;
        }

    }
}
