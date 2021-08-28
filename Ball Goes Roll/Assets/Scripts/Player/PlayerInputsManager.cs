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
    private bool leapFrogModePressed, leapFrogModeHeld, leapFrogModeReleased;
    private bool jump;





    private void Awake() {
        controls = new BallGoesRollActions();
        playerController = GetComponent<PlayerController>();
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

    private void OnMoveLeapTarget(InputValue input) {
        Vector2 v = input.Get<Vector2>();
        leapTargetMovementVector = new Vector3(0, 0, v.y);
    }

    private void OnJump(InputValue input) {
        playerController.DoJump();
    }

    // === On Action Methods End ===


    // === On Subscribed Methods Start ===
    // === On Subscribed Methods End ===

}
