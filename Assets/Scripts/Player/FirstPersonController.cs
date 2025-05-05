using System.Collections;
using UnityEngine;
//Adapted from: https://youtu.be/f473C43s8nE?si=sP3vS8Cw1WLd-tr0 && https://youtu.be/vBWcb_0HF1c?si=evpS0rS6HtbmqZ07

//TODO: Handle sticky walls
//TODO: Cleanup
public class FirstPersonController : MonoBehaviour
{
    [Header("Air Control")]
    [SerializeField] private float airMoveMultiplier = 2.0f;
    [SerializeField] [Range(0f,1f)] float airDrag = 0.9f;
    bool bGroundedFrameOne = false;

    [Header("References")]
    [SerializeField] private JumpController jumpController;
    [SerializeField] private MovementController moveController;
    [SerializeField] private ActionsController actionsController;
    [SerializeField] private FirstPersonCameraController cameraController;
    [SerializeField] private StairBehaviour stairBehaviour;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private PlayerInputHandler playerInput;
    [SerializeField] private Transform cameraBodyOrientator;
    [SerializeField] private ParticleSystem StunPA;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject heldAnimation;
    private PauseMenu pauseMenu;
    public ICapturable grabbedObject;

    bool predictThrow = false;
    [HideInInspector] public bool isStunned = false;
    private float invert = 1.0f;
    private void Awake()
    {
        try
        {
            pauseMenu = FindFirstObjectByType<PauseMenu>();
            pauseMenu.paused.AddListener(TurnPlayerControlsOff);
            pauseMenu.resumed.AddListener(TurnPlayerControlsOn);
        }
        catch (System.Exception)
        {
            Debug.LogWarning("No Pause Menu has been found, player cannot pause");
        }

    }
    private void Start()
    {
        ParticleSystem p = Instantiate(StunPA);
        StunPA = p;
        p.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (playerInput.PausePressed && pauseMenu != null)
        {
            FindAnyObjectByType<PauseMenu>().OnPauseButtonPressed();
        }
        if (actionsController.IsObjectHeld())
        {
            if ((playerInput.SwingPressed || predictThrow))
            {
                animator.gameObject.SetActive(true);
                heldAnimation.SetActive(false);
            }
            else
            {
                animator.gameObject.SetActive(false);
                heldAnimation.SetActive(true);
            }
 
        }
        else
        {
            animator.gameObject.SetActive(true);
            heldAnimation.SetActive(false);
        }
        SendInput();
    }

    private void FixedUpdate()
    {
        if (!jumpController.isGrounded)
        {
            moveController.AddNamedMovementMultipler("Air", airMoveMultiplier);
            moveController.DampenMovement(airDrag);
            bGroundedFrameOne = true;
        }
        if (jumpController.isGrounded && bGroundedFrameOne)
        {
            bGroundedFrameOne = false;
            moveController.RemoveNamedMovementMultipler("Air");
        }
        stairBehaviour.TryClimbStairs(moveController.CalculateWorldDirection(playerInput.MovementInput), jumpController.isGrounded);
    }
    public void InvertMovement(float duration)
    {
        invert = -1.0f;
        Invoke("ResetInvert", duration);
    }
    private void ResetInvert()
    {
        invert = 1.0f;
    }
    public Transform GetPlayerBody()
    {
        return rb.transform;
    }
    private void SendInput()
    {
 
        // Jump Controller
        if (playerInput.JumpTriggered)
        {
            jumpController.JumpPressed();
        }

        // Movement Controller
        //TODO: Tweak behaviour of jump sprints
        if (jumpController.isGrounded)
        {
            moveController.isSprinting = playerInput.SprintTriggered;
            // Cannot sprint backwards
            if (playerInput.MovementInput.y < 0)
            {
                moveController.isSprinting = false;
            }
        }
        moveController.MovePressed(playerInput.MovementInput * invert);


        // Actions, throw has priority, then swing, then grab
        if (playerInput.SwingPressed || predictThrow)
        {
            if (actionsController.IsObjectHeld())
            {
                animator.gameObject.SetActive(true);
                heldAnimation.SetActive(false);
                if (playerInput.SwingTriggered)
                {
                    animator.SetBool("ThrowReady", true);
                    actionsController.PredictThrow();
                    predictThrow = true;
                }
                else
                {
                    animator.SetBool("ThrowReady", false);
                    animator.SetTrigger("Throw");
                    predictThrow = false;
                    actionsController.ThrowObjectIfHeld();
                }

            }
            else
            {
                animator.SetBool("ThrowReady", false);
                predictThrow = false;
            }
        }

        if (!actionsController.IsObjectHeld())
        {
            {
                if (playerInput.SwingPressed || playerInput.GrabPressed)
                {

                    if (playerInput.SwingTriggered)
                    {
                        actionsController.SwingPressed();
                        animator.SetTrigger("Swing");
                    }
                    else if (playerInput.GrabTriggered)
                    {
                        actionsController.GrabPressed();
                        animator.SetTrigger("Grab");
                    }
                }
            }
        }
    }

    private void ChangePlayerControlState(bool on)
    {
        Debug.Log("Player Controls: " + on);
        moveController.enabled = on;
        jumpController.enabled = on;
        actionsController.enabled = on;
        cameraController.enabled = on;
    }

    public void TurnPlayerControlsOn()
    {
        playerInput.EnableGameControls();
    }

    public void TurnPlayerControlsOff()
    {
        playerInput.DisableGameControls();
        playerInput.SetControlReadsToZero();
    }
    public void TurnOffPauseControl()
    {
        playerInput.DisablePause();
    }
    public void TurnOnPauseControl()
    {
        playerInput.EnablePause();
    }
    public void StunPlayer(float stunDelay)
    {
        StartCoroutine(StunPlayer_Coroutine(stunDelay));
    }

    private IEnumerator StunPlayer_Coroutine(float stunDelay)
    {
        if(StunPA != null)
        {
            StunPA.Play();
            StunPA.gameObject.transform.position = this.GetPlayerBody().transform.position;
            StunPA.gameObject.SetActive(true);
        }
        isStunned = true;
        TurnPlayerControlsOff();
        yield return new WaitForSeconds(stunDelay);
        if(StunPA != null)
        {
            StunPA.Stop();
            StunPA.gameObject.SetActive(false);
        }
        isStunned = false;
        TurnPlayerControlsOn();
    }

}
