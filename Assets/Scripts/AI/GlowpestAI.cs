using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GlowpestAI : CreatureAI
{
    [Header("GlowPest AI Specifics")]
    public List<BreakableObject> breakableObjects = new List<BreakableObject>();
    private BreakableObject currentObject;

    [Space]
    [Header("GlowPest AI Behaviours")]
    [SerializeField][Range(0,2)][Tooltip("Distance from object to consider roam completed")]
    private float distanceToObject = 1.0f;
    [SerializeField][Range(0, 5)][Tooltip("The distance around a breakable object to move around")]
    private float randomDistanceAroundObject = 1.0f;
    public float Flutter = 100;

    public bool upordown;
    public SkinnedMeshRenderer wings;

    public override void Start()
    {
        base.Start();
        // Try and get all breakable objects if none assigned
        if (breakableObjects.Count < 1)
        {
            breakableObjects = FindObjectsByType<BreakableObject>(FindObjectsSortMode.None).ToList();
        }



        SwitchBehaviour();
    }

    public override void Update()
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
            Flutter += 25;
        }
        if (upordown == false)
        {
            Flutter -= 25;
        }




        // Moving towards roaming position
        if (isRoaming)
        {
            var val = Vector3.Distance(transform.position, selectedPos);
            if (val < distanceToObject)
            {
                animator.SetTrigger("Idle");

                StartCoroutine(Roam_Coroutine());
                isRoaming = false;
            }
        }

        if (currentObject.isBroken)
        {
            breakableObjects.Remove(currentObject);
            StartCoroutine(ResetPosition_Coroutine());
            StopCoroutine(Roam_Coroutine());
            StopCoroutine(Hover_Coroutine());
            Roam();
        }
    }

    public override void Roam()
    {
        if(breakableObjects.Count < 1)
            return;
        StopCoroutine(Hover_Coroutine());
        var random = Random.Range(0, breakableObjects.Count);
        var roamPos = breakableObjects[random];
        transform.LookAt(roamPos.gameObject.transform);
        navMeshAgent.SetDestination(roamPos.gameObject.transform.position);
        Debug.Log("Start Flying Animation");
        animator.SetTrigger("Flying");
        selectedPos = roamPos.gameObject.transform.position;
        // Dirty fix to reset hovers position if the same object is chosen twice
        // Bug happens where ResetPosition_Coroutine doesn't work correctly if the same object is chosen for roaming
        // Leading to hovering underneath the map
        if (currentObject == roamPos)
            meshObject.transform.SetLocalPositionAndRotation(meshOriginalRelativePosition, Quaternion.identity);

        currentObject = roamPos;
        isRoaming = true;
    }

    public override IEnumerator Roam_Coroutine()
    {
       
        StartCoroutine(Hover_Coroutine());
        yield return new WaitForSeconds(roamingDelay);
        StopCoroutine(Hover_Coroutine());
        StartCoroutine(ResetPosition_Coroutine());
        Roam();

    }

    public IEnumerator Hover_Coroutine()
    {
        // Sets a new position around the object to move around
        var roamPos = currentObject.transform.position;
        float xRandomDist = Random.Range(-randomDistanceAroundObject, randomDistanceAroundObject);
        float zRandomDist = Random.Range(-randomDistanceAroundObject, randomDistanceAroundObject);
        navMeshAgent.SetDestination(new Vector3(roamPos.x + xRandomDist, roamPos.y, roamPos.z + zRandomDist));

        // Hovers up and down
        float hoverHeight = 1.0f;
        float timer = -1f;
        while (timer < 1.0f)
        {
            meshObject.transform.position += new Vector3(0,hoverHeight * Mathf.Sign(timer) * Time.deltaTime,0);
            timer += Time.deltaTime;
            yield return null;
        }

        // Stops recursive hovers if we have started moving to a new object
        if (!isRoaming) 
            StartCoroutine(Hover_Coroutine());

    }

    public IEnumerator ResetPosition_Coroutine()
    {
        // Lerps back to the original displaced position so that it continues to hover at correct height
        float time = 0.0f;
        while (Vector3.Distance(meshObject.transform.position, meshOriginalRelativePosition) < 0.1f)
        {
            Vector3 newPos = Vector3.Lerp(meshObject.transform.position, meshOriginalRelativePosition, time);
            meshObject.transform.SetLocalPositionAndRotation(newPos, meshObject.transform.rotation);
            //meshObject.transform.position = newPos;
            time += Time.deltaTime;
            yield return null;
        }
    }
}
