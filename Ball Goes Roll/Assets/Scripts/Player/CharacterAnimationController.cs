using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour {
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private PlayerController playerController;

    private Animator anim;

    private bool leapRoutineRunning = false;

    void Start() {
        //anim = GetComponent<Animator>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update() {
        SetParameters();
        SetRotationBasedOnVelocity();
        //SetRotationBasedOnInput();
    }

    private void SetParameters() {
        // Idle, Walk, Run Parameter
        anim.SetFloat("Speed", playerRb.velocity.magnitude);

        // Jump Parameters
        if (!isJumpRoutineRunning && playerController.IsGrounded && Input.GetButtonDown("Jump")) {
            StartCoroutine(JumpAndLandRoutine());
        }

        // Fall Parameter
        if (!isJumpRoutineRunning && !playerController.IsGrounded) {
            anim.SetBool("IsFalling", true);
            StartCoroutine(JumpAndLandRoutine(true));       // Skip jump animation and go strain to fall anim
        }
        //Leap Parameters
        if (!leapRoutineRunning && playerController.InLeapViewMode == true) {
            StartCoroutine(LeapRoutine());

        }

    }

    bool isJumpRoutineRunning = false;
    IEnumerator JumpAndLandRoutine(bool skipJump = true) {
        isJumpRoutineRunning = true;
        anim.SetBool("InAir", true);        // character in air while jumping
        if (!skipJump) {

            anim.SetBool("Jumping", true);      // Starts jumping animation
            yield return new WaitForSeconds(0.1f);
        }                // Give the ball time to get off ground before doing grounded check
        // Wait until Landed
        while (!playerController.IsGrounded) {
            yield return null;

        }
        anim.SetBool("InAir", false);
        anim.SetBool("Jumping", false);
        anim.SetBool("IsFalling", false);

        isJumpRoutineRunning = false;
    }

    IEnumerator LeapRoutine() {
        leapRoutineRunning = true;
        anim.SetBool("InLeapViewMode", true);

        // Character in Leap view Mode (Crouching)
        while (playerController.InLeapViewMode) {
            yield return null;
            SetCharacterToCameraDirection();       // Set the direction the character is facing
            if (playerController.isLeaping == true) {
                anim.SetBool("InLeapViewMode", false);
                anim.SetBool("IsLeaping", true);

            }
            // Cancel Leap
            if (Input.GetButtonDown("Cancel")) {
                anim.SetBool("InLeapViewMode", false);       // Resetting IsLeaping Parameter
                anim.SetBool("IsLeaping", false);       // Resetting IsLeaping Parameter
                anim.SetBool("LeapLand", false);        // Resetting LeapLand Parameter
                anim.Play("StandingBored");
                leapRoutineRunning = false;
                yield break;
            }
        }
        //yield return new WaitForFixedUpdate();      // should hopefully wait till after ball has left ground
        yield return new WaitForSeconds(0.1f);

        // Player in Air
        while (!playerController.IsGrounded) {
            yield return null;
        }
        // Player Landed
        anim.SetBool("LeapLand", true);         //
        anim.SetBool("IsLeaping", false);       // Resetting IsLeaping Parameter
        yield return null;
        anim.SetBool("LeapLand", false);        // Resetting LeapLand Parameter

        leapRoutineRunning = false;
    }

    // Sets the character to face the same way as the camera
    // Somethings isnt right with this method. It only works if SetRotationBasedOnVelocity() is also running this frame. Dont know why
    private void SetCharacterToCameraDirection() {
        Vector3 cameraAtCharacterY = Camera.main.transform.position;            // the camera position where the y component is set to the same as the character,s
        cameraAtCharacterY.y = transform.position.y;
        Vector3 cameraToCharacterDirection = transform.position - cameraAtCharacterY;

        Vector3 lookDirection = transform.TransformDirection(cameraToCharacterDirection);

        transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
    }


    // The character will always face the way the ball is rotating
    private void SetRotationBasedOnVelocity() {
        /*if (playerRb.velocity.magnitude == 0) {     // If velocity is 0 then face the way the camera is facing.
            SetCharacterToCameraDirection();
            return;
        }*/
        Vector3 direction = playerRb.velocity.normalized;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private void SetRotationBasedOnInput() {
        Vector3 inputXZ = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (inputXZ.magnitude > 0) {
            Vector3 directionToFace = transform.TransformDirection(inputXZ);
            transform.rotation = Quaternion.LookRotation(directionToFace, Vector3.up);
        }

    }
}
