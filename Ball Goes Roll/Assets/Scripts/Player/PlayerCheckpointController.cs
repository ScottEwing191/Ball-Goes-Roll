using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpointController : MonoBehaviour {
    [SerializeField] private Transform cameraTarget;            // when camera target is rotated camera will also rotate. So rotation is set by changing camera target rotation

    [SerializeField] private float skipCheckpointInputHoldTime = 1;
    [SerializeField] private int resetsBeforeCheckpointSkipAvailable = 3;       // the number of resets before the player gets the option to skip tothe next checkpoint
    public int resetsOnCurrentCheckpoint;                           // how many times has the player reset on the current checkpoint
    private bool canSkipCheckpoint;
    private bool isSkipCheckpointHoldRoutineRunning = false;        // Keeps track of whether the Skip checkpoint hold routine is running

    private Coroutine skipCheckpointHoldRoutine = null;             // create the variable so that it can be used to stop the routine later
    private Checkpoint currentCheckpoint;
    private Checkpoint nextCheckpoint;

    // Method called from PlayerInputHandler when input is pressed
    public void TrySkipCheckpoint() {
        if (canSkipCheckpoint) {
            skipCheckpointHoldRoutine = StartCoroutine(SkipCheckpointHold());
            UILevelManager.Instance.DisableSkipCheckpointText();
            UILevelManager.Instance.EnableInputImage();
        }
    }
    // Method called from PlayerInputHandler when input is released
    public void CancelSkipCheckpoint() {
        if (canSkipCheckpoint && isSkipCheckpointHoldRoutineRunning) {
            StopCoroutine(skipCheckpointHoldRoutine);
            isSkipCheckpointHoldRoutineRunning = false;
            ResetUIAfterHoldInputEnumerator();
        }
    }

    private IEnumerator SkipCheckpointHold() {
        float time = 0;
        isSkipCheckpointHoldRoutineRunning = true;
        UILevelManager.Instance.SetSkipCheckpointFillImage(0);
        while (time <= skipCheckpointInputHoldTime) {

            // get a representation of the current time as value between 0 and 1 where 0 is 0 seconds and 1 is the number of secs required to skip
            float valueForSlider = Mathf.Clamp(time / skipCheckpointInputHoldTime, 0, 1);
            UILevelManager.Instance.SetSkipCheckpointFillImage(valueForSlider);
            time += Time.deltaTime;
            yield return null;
        }
        ResetUIAfterHoldInputEnumerator();
        GoToNextCheckpoint();
        isSkipCheckpointHoldRoutineRunning = false;
    }

    // if the the player enters a checkpoint while holding the skip checkpoint button they will skip to the next CP of the CP they have just entered,
    // skipping the minimum number of fails thay must have before being able to skip a checkpoint.
    // To fix this the co routine must be stopped when the player enters a new checkpoint. Which aslo means that any code in the Enumerator which must run 
    // regardless of whether the player succesfully skips the checkpoint or not i.e resetting the UI must be moved to another method which is called from
    // the Enumerator and the method that stops the enumerator
    private void ResetUIAfterHoldInputEnumerator() {
        UILevelManager.Instance.DisableInputImage();
        UILevelManager.Instance.SetSkipCheckpointFillImage(0);
    }

    public void Respawn() {
        //Invoke("MoveBallToCheckpoint", UIManager.Instance.BallResetFadeTime);

        StartCoroutine(MoveBallToCheckpoint(currentCheckpoint));
        resetsOnCurrentCheckpoint++;
        if (resetsOnCurrentCheckpoint >= resetsBeforeCheckpointSkipAvailable) {
            canSkipCheckpoint = true;
            // Tell UI manager to enable skip checkpoint text and image
            //UIManager.Instance.EnableInputImage();
            //UIManager.Instance.EnableSkipCheckpointText();
            UILevelManager.Instance.Invoke("EnableInputImage", UILevelManager.Instance.ScreenFadeTime);
            UILevelManager.Instance.Invoke("EnableSkipCheckpointText", UILevelManager.Instance.ScreenFadeTime);

        }

    }

    public void GoToNextCheckpoint() {
        StartCoroutine(MoveBallToCheckpoint(nextCheckpoint));
    }

    private IEnumerator MoveBallToCheckpoint(Checkpoint checkpoint) {
        if (checkpoint == null) {
            Debug.LogError("Player Respawn Script: MoveBallToCheckpoint(): checkpoint is null");
            //return;
            yield break;
        }
        //UILevelManager.Instance.FadeOutFadeIn();     // Tell the UI manager to fade out and in
        UILevelManager.Instance.ScreenFadeOutIn();     // Tell the UI manager to fade out and in
        // wait a second before moving the ball this give the UI time to fade to black so that the player doesnt see the ball snap to another position
        yield return new WaitForSeconds(UILevelManager.Instance.ScreenFadeTime);
        transform.position = checkpoint.RespawnPoint;
        cameraTarget.rotation = checkpoint.RespawnRotation;
        PlayerSingleton.Instance.PlayerController.StopBall();
    }

    private void OnTriggerEnter(Collider other) {
        SetCheckpoints(other);
    }

    #region No Next Checkpoint ID Version
    // For this version the checkpoint in the array which is set as the next checkpoint depends on the previous checkpoint.
    // If there is no previous CP then the 1st element in the array will be the next CP
    // if the the previous CP is not in the array then the fst element in the array will be the next CP
    // if the previous CP is in the array then the element in the array after this (the previous) CP will be set as the next CP
    // if there is no CP's in the array then next CP will be Null
    private void SetCheckpoints(Collider other) {
        if (other.CompareTag("Checkpoint")) {
            //print("Checkpoint Reached");

            Checkpoint otherCheckpoint = other.GetComponent<Checkpoint>();
            // Player Re entering current checkpoint
            if (currentCheckpoint == otherCheckpoint) {
                return;                                     // dont need to do anything if player has entered current checkpoint
            }
            Checkpoint previousCheckpoint = currentCheckpoint;
            currentCheckpoint = otherCheckpoint;
            resetsOnCurrentCheckpoint = 0;                  // reset variable which track how many time the player has reset on each checkpoint
            canSkipCheckpoint = false;
            // Tell UI Manager to disable skip Checkpoint text and image upon entering a new checkpoint
            UILevelManager.Instance.DisableInputImage();
            UILevelManager.Instance.DisableSkipCheckpointText();
            // coroutine must be stopped when entering checkpoint so that the player cant skip ahead to the new CP's next CP
            //StopAllCoroutines();
            if (skipCheckpointHoldRoutine != null) {
                StopCoroutine(skipCheckpointHoldRoutine);
            }
            ResetUIAfterHoldInputEnumerator();
            // Code to do with Next Checkpoint
            if (otherCheckpoint.NextCheckpoints[0] != null) {       // if the checkpoint the player has just reached contains a rerference to the next checkpoint

                if (previousCheckpoint != null) {
                    //nextCheckpoint = currentCheckpoint.NextCheckpoints[previousCheckpoint.NextCheckpointIndex];

                    for (int i = 0; i < currentCheckpoint.NextCheckpoints.Length; i++) {       // dont need to check if previous CP is == to last CP in array since this would only happen if current CP had already been reached
                        if (i == currentCheckpoint.NextCheckpoints.Length - 1) {     //check if on last itteration of loop
                            nextCheckpoint = currentCheckpoint.NextCheckpoints[0]; // if there is no match before the end "2nd last index" of array player has not yet been to CP so use first index
                            continue;
                        }
                        if (previousCheckpoint == currentCheckpoint.NextCheckpoints[i]) {
                            nextCheckpoint = currentCheckpoint.NextCheckpoints[i + 1];
                            return;
                        }
                    }
                }
                else {
                    nextCheckpoint = currentCheckpoint.NextCheckpoints[0];
                }
            }
            else {
                nextCheckpoint = null;
            }
            //other.gameObject.SetActive(false);      // deactivate checkpoints player has been too maybe dont do this
        }
    }
    #endregion

    /*#region Next Checkpoint ID Version

    private void SetCheckpoints(Collider other) {
        if (other.CompareTag("Checkpoint")) {
            print("Checkpoint Reached");

            Checkpoint otherCheckpoint = other.GetComponent<Checkpoint>();
            if (currentCheckpoint == otherCheckpoint) {
                return;                                     // dont need to do anything if player has entered current checkpoint
            }
            Checkpoint previousCheckpoint = currentCheckpoint;
            currentCheckpoint = otherCheckpoint;

            if (otherCheckpoint.NextCheckpoints[0] != null) {       // if the checkpoint the player has just reached contains a rerference to the next checkpoint

                if (previousCheckpoint != null) {
                    nextCheckpoint = currentCheckpoint.NextCheckpoints[previousCheckpoint.NextCheckpointIndex];
                }
                else {
                    nextCheckpoint = currentCheckpoint.NextCheckpoints[0];
                }

            }
            else {
                nextCheckpoint = null;
            }

            //other.gameObject.SetActive(false);      // deactivate checkpoints player has been too maybe dont do this
        }
    }
    #endregion*/



}
