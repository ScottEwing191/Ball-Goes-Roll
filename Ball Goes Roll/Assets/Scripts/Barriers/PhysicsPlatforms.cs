using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsPlatforms : MonoBehaviour {
    enum PlatformState { AT_START, TOWARDS_START, AT_TARGET, TOWARDS_TARGET }

    [SerializeField] float maxVelocity = 1f;
    float acceleration = 1f;       // not currently working with acceleration
    [SerializeField] float moveTime = 5f;
    [SerializeField] private float startDelay = 0;
    [SerializeField] float waitTime = 1f;
    [SerializeField] bool calculateVelocityWithMoveTime = true;

    bool useAcceleration = false;      // Platforms do not currently work if this is true

    [SerializeField] Transform targetTransform;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 startToTargetDir;
    private Vector3 targetToStartDir;
    private Vector3 currentDestination;

    private float threshholdDistance = 0.1f;                // The distance at which the platform will be considered at the target

    private bool isWaitTimeUp = false;

    PlatformState platformState = PlatformState.TOWARDS_TARGET;

    private Rigidbody rb;


    private void Awake() {
        targetPosition = targetTransform.position;
        startPosition = transform.position;
        startToTargetDir = (targetPosition - startPosition).normalized;
        targetToStartDir = -startToTargetDir;
        rb = GetComponent<Rigidbody>();
        currentDestination = targetPosition;
        platformState = PlatformState.AT_START;
        StartCoroutine(WaitTime(startDelay));
        if (calculateVelocityWithMoveTime) {
            CalculateMaxVelocity();
        }
    }

    // Calculate the max velocity based on how long it takes to move from start to target position
    private void CalculateMaxVelocity() {
        float dst = Vector3.Distance(startPosition, targetPosition);
        maxVelocity = dst / moveTime;

    }

    // Start is called before the first frame update
    void Start() {

    }
    private void Update() {

    }
    // Update is called once per frame
    void FixedUpdate() {
        CheckPlatformState();
        CheckPlatformVelocity();
    }

    // Will cause error if platform is moving fast enough that it moves too big a distance from frame to frame and the distance is never below the threshold amount
    private bool HasReachedDestination() {
        float dstToDest = Vector3.Distance(transform.position, currentDestination);
        if (dstToDest < threshholdDistance) {
            return true;
        }
        return false;

    }

    private void CheckPlatformState() {
        switch (platformState) {
            case PlatformState.AT_START:
                if (isWaitTimeUp) {
                    platformState = PlatformState.TOWARDS_TARGET;
                    currentDestination = targetPosition;
                    MovePlatform(startToTargetDir, maxVelocity);
                }
                break;
            case PlatformState.TOWARDS_START:
                if (HasReachedDestination()) {
                    StartCoroutine(WaitTime(waitTime));
                    platformState = PlatformState.AT_START;
                    StopPlatform();
                }
                break;
            case PlatformState.AT_TARGET:
                if (isWaitTimeUp) {
                    platformState = PlatformState.TOWARDS_START;
                    currentDestination = startPosition;
                    MovePlatform(targetToStartDir, maxVelocity);
                }
                break;
            case PlatformState.TOWARDS_TARGET:
                if (HasReachedDestination()) {
                    StartCoroutine(WaitTime(waitTime));
                    platformState = PlatformState.AT_TARGET;
                    StopPlatform();
                }
                break;
            default:
                break;
        }
    }

    void MovePlatform(Vector3 direction, float force) {
        if (useAcceleration) {
            rb.AddForce(direction * acceleration);      // this is currently not working for some reason

        }
        else {
            rb.AddForce(direction * force, ForceMode.VelocityChange);

        }

    }

    void StopPlatform() {
        rb.AddForce(-rb.velocity, ForceMode.VelocityChange);
    }
    void CheckPlatformVelocity() {
        if (rb.velocity.magnitude > maxVelocity) {
            float extraVelocity = rb.velocity.magnitude - maxVelocity;
            MovePlatform(-rb.velocity.normalized, extraVelocity);
        }
    }

    IEnumerator WaitTime(float timeToWait) {
        isWaitTimeUp = false;
        yield return new WaitForSeconds(timeToWait);
        isWaitTimeUp = true;
    }

}
