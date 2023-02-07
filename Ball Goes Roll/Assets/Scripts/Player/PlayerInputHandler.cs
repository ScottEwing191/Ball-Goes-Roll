using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour {
    public Vector3 MovementVector { get; private set; }
    public Vector3 LeapTargetMovementVector { get; private set; }
    public float HorizontalCameraMovement { get; private set; }
    public float VerticalCameraMovement { get; private set; }
    public bool RunLeapRoutine { get; private set; }

    private bool leapJustCancelled, nextCheckpointPressed;
    private PlayerInput playerInput;

    public BallGoesRollActionsTwo Controls { get; set; }

    private PlayerController playerController;
    private InputActionMap playerActionMap, leapModeActionMap, UIActionMap;
    private Action swapPlayerAndLeapModeActionMap;


    private void Awake() {
        Controls = new BallGoesRollActionsTwo();
        playerController = GetComponentInChildren<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        playerActionMap = playerInput.actions.FindActionMap("Player");
        leapModeActionMap = playerInput.actions.FindActionMap("LeapMode");
        UIActionMap = playerInput.actions.FindActionMap("UI");
        UIActionMap.Enable();

    }

    private void OnEnable() {
        Controls.Enable();
        swapPlayerAndLeapModeActionMap += SwapPlayerAndLeapModeActionMap;

    }
    private void OnDisable() {
        Controls.Disable();
        swapPlayerAndLeapModeActionMap -= SwapPlayerAndLeapModeActionMap;

    }

    // === On Action Methods Start ===
    private void OnMove(InputValue input) {
        Vector2 v = input.Get<Vector2>();
        MovementVector = new Vector3(v.x, 0, v.y);
    }
    public void OnMove(InputAction.CallbackContext input) {
        Vector2 v = input.action.ReadValue<Vector2>();
        MovementVector = new Vector3(v.x, 0, v.y);
        if (input.started) {
            print("started");
        }
        if (input.performed) {
            print("performed");
        }
        if (input.canceled) {
            print("canceled");
        }
    }
    
    private void OnLook(InputValue input) {
        Vector2 v = input.Get<Vector2>();
        HorizontalCameraMovement = v.x;
        VerticalCameraMovement = -v.y;      // inverted is better
    }
    

    private void OnJump(InputValue input) {
        playerController.DoJump();
    }

    private void OnInteract(InputValue input) {
        playerController.TryInteract();

    }

    private void OnRespawn(InputValue input) {
        print("RESPAWN");
        playerController.Respawn();
    }

    private void OnCloseTip(InputValue input) {
        print("On Close Tip");
        
    }
    private void OnNextCheckpoint(InputValue input) {
        nextCheckpointPressed = !nextCheckpointPressed;
        if (nextCheckpointPressed) {
            PlayerSingleton.Instance.PlayerCheckpointController.TrySkipCheckpoint();
        }
        else {
            PlayerSingleton.Instance.PlayerCheckpointController.CancelSkipCheckpoint();
        }
    }
    public void SwapPlayerAndLeapModeActionMap() {
        playerActionMap.Disable();
        leapModeActionMap.Enable();
    }
    private void OnStartLeapMode(InputValue input) {
        if (leapJustCancelled) {      // this check is needed since if the leap is cancelled the right trigger will still be held down when the Player action map is swittched
            leapJustCancelled = false;  //... back when the trigger move (start to be released by player) it will be registered a a button press and this bethod will be called
            return;                     // ... when it shouldn't be. 
        }
        print("Start Leap");
        RunLeapRoutine = true;
        StartCoroutine(playerController.StartLeapMode(swapPlayerAndLeapModeActionMap));

    }
    private void OnLeap(InputValue input) {
        RunLeapRoutine = false;
        playerController.Leap();
        playerActionMap.Enable();
        leapModeActionMap.Disable();
    }
    private void OnCancelLeap(InputValue input) {
        RunLeapRoutine = false;
        leapJustCancelled = true;
        playerController.CancelLeap();
        playerActionMap.Enable();
        leapModeActionMap.Disable();
    }
    private void OnMoveLeapTarget(InputValue input) {
        Vector2 v = input.Get<Vector2>();
        LeapTargetMovementVector = new Vector3(0, 0, v.y);
    }

    private void OnTogglePauseMenu(InputValue input) {
        GameManager.Instance.DoPauseGame();
        /*if (playerActionMap.enabled)
            playerActionMap.Disable();
        else
            playerActionMap.Enable();*/
    }

    // === On Action Methods End ===

}
