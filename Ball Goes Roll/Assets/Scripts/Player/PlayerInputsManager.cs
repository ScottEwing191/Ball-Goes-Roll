using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputsManager : MonoBehaviour {
    BallGoesRollActions controls;
    PlayerController playerController;
    private Vector3 movementVector = Vector3.zero;
    private Vector3 leapTargetMovementVector;
    float horizontalCameraMovement, verticalCameraMovement;

    private bool leapFrogModePressed, leapFrogModeHeld, leapFrogModeReleased;
    private bool jump;
    private bool cancelLeap;

    private void Awake() {
        controls = new BallGoesRollActions();
        playerController = GetComponentInChildren<PlayerController>();
        //controls.Player.CancelLeap.canceled += Cancel;
        //controls.Player.CancelLeap.performed += Cancel;

    }

    private void OnEnable() {
        controls.Enable();
    }
    private void OnDisable() {
        controls.Disable();
    }

    #region Properties
    public Vector3 MovementVector {
        get { return movementVector; }
    }
    public Vector3 LeapTargetMovementVector {
        get { return leapTargetMovementVector; }
    }
    public bool LeapFrogModePressed {
        get { return leapFrogModePressed; }
    }
    public bool LeapFrogModeHeld {
        get { return leapFrogModeHeld; }
    }
    public bool LeapFrogModeReleased {
        get { return leapFrogModeReleased; }
    }
    public bool Jump {
        get { return jump; }
    }
    public bool CancelLeap {
        get { return cancelLeap; }
    }
    public float HorizontalCameraMovement {
        get { return horizontalCameraMovement; }
    }
    public float VerticalCameraMovement {
        get { return verticalCameraMovement; }
    }
    #endregion

    private void Update() {
        bool currentLeapInputValue = controls.Player.LeapFrogMode.ReadValue<float>() > 0;
        // Get Button Down
        if (currentLeapInputValue && !leapFrogModePressed && !leapFrogModeHeld && !leapFrogModeReleased) {
            leapFrogModePressed = true;
        }
        //Get Button
        else if (currentLeapInputValue && leapFrogModePressed && !leapFrogModeHeld && !leapFrogModeReleased) {
            leapFrogModePressed = false;
            leapFrogModeHeld = true;
        }
        //Get Button Up
        else if (!currentLeapInputValue && !leapFrogModePressed && leapFrogModeHeld && !leapFrogModeReleased) {
            leapFrogModeReleased = true;
            leapFrogModeHeld = false;
        }
        else if (!currentLeapInputValue && !leapFrogModePressed && !leapFrogModeHeld && leapFrogModeReleased) {
            leapFrogModeReleased = false;

        }
    }

    
    // === On Action Methods Start ===
    private void OnMove(InputValue input) {
        Vector2 v = input.Get<Vector2>();
        movementVector = new Vector3(v.x, 0, v.y);
    }
    private void OnLook(InputValue input) {
        Vector2 v = input.Get<Vector2>();
        horizontalCameraMovement = v.x;
        verticalCameraMovement = -v.y;      // inverted is better
    }
    private void OnMoveLeapTarget(InputValue input) {
        Vector2 v = input.Get<Vector2>();
        leapTargetMovementVector = new Vector3(0, 0, v.y);
    }

    private void OnJump(InputValue input) {
        playerController.DoJump();
    }

    private void OnCancelLeap(InputValue input) {
        cancelLeap = true;
        StartCoroutine(SetCancelLeapFalseAtEndOfFrame());
    }
    // === On Action Methods End ===


    // === On Subscribed Methods Start ===
    // Using this method to handle the cancel button works but it means that the cancel variable will continue to be true until the player releases the key. So player couldnt start a new leap until cancel
    // ... button is released
    /*private void Cancel(InputAction.CallbackContext ctx) {
        if (ctx.performed) {
            print("Performed");
            cancelLeap = true;
        }
        else if (ctx.canceled) {
            print("Cancelled");
            cancelLeap = false;
        }
    }*/
    // === On Subscribed Methods End ===


    // === Enumerators
    IEnumerator SetCancelLeapFalseAtEndOfFrame() {       // Emulates The Get Key Down Functionality
        yield return new WaitForEndOfFrame();
        cancelLeap = false;
    }
}
