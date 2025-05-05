using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//Inspired by: https://youtu.be/vBWcb_0HF1c?si=evpS0rS6HtbmqZ07

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControl;

    [Header("Action Map Name Reference")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string movement = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string swing = "Swing";
    [SerializeField] private string grab = "Grab";
    [SerializeField] private string pause = "Pause";

    private Dictionary<string, InputAction> actions = new Dictionary<string, InputAction>();

    public Vector2 MovementInput {  get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool SprintTriggered { get; private set; }
    public bool SwingTriggered { get; private set; }
    public bool GrabTriggered { get; private set; }
    public bool SwingPressed { get; private set; }
    public bool GrabPressed { get; private set; }
    public bool PausePressed { get; private set; }

    public void Awake()
    {
        InputActionMap mapReference = playerControl.FindActionMap(actionMapName);

        actions[movement] = mapReference.FindAction(movement);
        actions[look] = mapReference.FindAction(look);
        actions[jump] = mapReference.FindAction(jump);
        actions[sprint] = mapReference.FindAction(sprint);
        actions[swing] = mapReference.FindAction(swing);
        actions[grab] = mapReference.FindAction(grab);
        actions[pause] = mapReference.FindAction(pause);

        SubscribeActionValuesToInputEvents();

        PausePressed = false;
    }

    private void SubscribeActionValuesToInputEvents()
    {
        actions[movement].performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        actions[movement].canceled += inputInfo => MovementInput = Vector2.zero;

        actions[look].performed += inputInfo => LookInput = inputInfo.ReadValue<Vector2>();
        actions[look].canceled += inputInfo => LookInput = Vector2.zero;

        actions[jump].performed += inputInfo => JumpTriggered = true;
        actions[jump].canceled += inputInfo => JumpTriggered = false;

        actions[sprint].performed += inputInfo => SprintTriggered = true;
        actions[sprint].canceled += inputInfo => SprintTriggered = false;

        actions[swing].started += inputInfo => SwingTriggered = true;
        actions[swing].performed += inputInfo => SwingTriggered = true;
        actions[swing].canceled += inputInfo => SwingTriggered = false;

        actions[grab].started += inputInfo => GrabTriggered = true;
        actions[grab].performed += inputInfo => GrabTriggered = true;
        actions[grab].canceled += inputInfo => GrabTriggered = false;

        actions[pause].started += inputInfo => PausePressed = true;
        actions[pause].performed += inputInfo => PausePressed = true;
        actions[pause].canceled += inputInfo => PausePressed = false;
    }
    private void Update()
    {
        //Terrible way to only detect a pressed event but cannot seem to do it with events
        SwingPressed = actions[swing].WasPressedThisFrame();
        GrabPressed = actions[grab].WasPressedThisFrame();
        PausePressed = actions[pause].WasPressedThisFrame();
    }
    private void OnEnable()
    {
        playerControl.FindActionMap(actionMapName).Enable();
    }

    private void OnDisable()
    {
        playerControl.FindActionMap(actionMapName).Disable();
    }

    public void SetPausePressed(bool pressed)
    {
        PausePressed = pressed;
    }

    public void DisableGameControls()
    {
        actions[movement].Disable();
        actions[look].Disable();
        actions[swing].Disable();
        actions[grab].Disable();
        actions[sprint].Disable();
        actions[jump].Disable();
    }
    public void EnableGameControls()
    {
        actions[movement].Enable();
        actions[look].Enable();
        actions[swing].Enable();
        actions[grab].Enable();
        actions[sprint].Enable();
        actions[jump].Enable();
    }

    public void DisablePause()
    {
        actions[pause].Disable();
        PausePressed = false;
    }
    public void EnablePause()
    {
        actions[pause].Enable();
    }
    public void SetControlReadsToZero()
    {
        MovementInput.Set(0, 0);
        LookInput.Set(0, 0);
        SwingPressed = false;
        SwingTriggered = false;
        GrabPressed = false;
        GrabTriggered = false;
        SprintTriggered = false;
        JumpTriggered = false;
    }
}
