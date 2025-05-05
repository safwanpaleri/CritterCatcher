using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterFlyAI : CreatureAI
{

    public float escapeTime = 3f;
    public override void Update()
    {
        if (isRunning)
        {
            var val = Vector3.Distance(transform.position, selectedPos);
            if (val < 0.5f)
            {
                isRunning = false;
                animator.SetTrigger("Idle");

                if (!isHiding)
                    SwitchBehaviour();
                else
                    StartCoroutine(HideDelay());

            }
            isRoaming = false;
        }

        if (isAttacking)
        {
            var val = Vector3.Distance(transform.position, playerBody.transform.position);
            if (val < 2.0f)
            {
                AttackPlayer();
                isAttacking = false;
                isRoaming = false;
            }
        }

        if (isRoaming)
        {
            var val = Vector3.Distance(transform.position, selectedPos);
            if (val < 0.5f)
            {
                animator.SetTrigger("Idle");

                StartCoroutine(Roam_Coroutine());
                isRoaming = false;
            }
        }

        if(isCaught)
        {
            StartCoroutine(EscapeMechanism_Coroutine());
            isCaught = false;
        }
    }

    private IEnumerator EscapeMechanism_Coroutine()
    {
        //add escape animation
        GetComponent<Animator>().SetBool("Slip", true);
        GetComponent<AudioPlayer>().PlayAudio("Slip");

        yield return new WaitForSeconds(escapeTime);
        playerBody.GetComponentInParent<ActionsController>().DetachCapturedObject();
        Thrown(Vector3.zero);
        SwitchBehaviour();
        GetComponent<Animator>().SetBool("Slip", false);
        meshObject.transform.SetLocalPositionAndRotation(meshOriginalRelativePosition, meshOriginalRelativeOrientation);
        transform.position += Vector3.up * 2.0f;
        GetComponent<AudioPlayer>().PlayAudio("EndSlip");
    }

    public override void Thrown(Vector3 direction)
    {
        // Sets the meshobject back to the original displacement so it looks like it hovers correctly
        meshObject.transform.SetLocalPositionAndRotation(meshOriginalRelativePosition, meshOriginalRelativeOrientation);

        TurnOnThrowCollision();
        StopCoroutine(EscapeMechanism_Coroutine());
        StartCoroutine(CanCaptureDelay());
        GetComponent<Animator>().SetBool("Slip", false);
    }


    public override void Captured()
    {
        // Stops any hover, or move to coroutines and snaps mesh object to the hand
        StopAllCoroutines();
        meshObject.transform.SetPositionAndRotation(new Vector3(0,0,0), meshObject.transform.rotation);
        canCapture = false;
    }

}
