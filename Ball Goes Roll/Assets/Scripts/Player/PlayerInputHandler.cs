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

    private bool leapJustCancelled = false;
    private PlayerInput playerInput;
    private BallGoesRollActions controls;
    private PlayerController playerController;
    private InputActionMap playerActionMap, leapModeActionMap;

    private void Awake() {
        controls = new BallGoesRollActions();
        playerController = GetComponentInChildren<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        playerActionMap = playerInput.actions.FindActionMap("Player");
        leapModeActionMap = playerInput.actions.FindActionMap("LeapMode");
    }

    private void OnEnable() {
        controls.Enable();
    }
    private void OnDisable() {
        controls.Disable();
    }

    // === On Action Methods Start ===
    private void OnMove(InputValue input) {
        Vector2 v = input.Get<Vector2>();
        MovementVector = new Vector3(v.x, 0, v.y);
    }
    private void OnLook(InputValue input) {
        Vector2 v = input.Get<Vector2>();
        HorizontalCameraMovement = v.x;
        VerticalCameraMovement = -v.y;      // inverted is better
    }

    private void OnJump(InputValue input) {
        playerController.DoJump();
    }

    private void OnRespawn(InputValue input) {
        playerController.Respawn();
    }

    private void OnStartLeapMode(InputValue input) {
        if (leapJustCancelled) {      // this check is needed if cancel leap input is on press
            leapJustCancelled = false;
            return;
        }
        print("Start Leap");
        RunLeapRoutine = true;
        StartCoroutine(playerController.StartLeapMode());

        playerActionMap.Disable();
        leapModeActionMap.Enable();
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
    
    // === On Action Methods End ===

}
