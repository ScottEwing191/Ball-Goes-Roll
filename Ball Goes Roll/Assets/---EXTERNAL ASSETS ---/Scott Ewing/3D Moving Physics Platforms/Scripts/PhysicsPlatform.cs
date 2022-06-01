using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ScottEwing.MovingObjects.MovingPhysicsPlatform3D {
    public class PhysicsPlatform : MonoBehaviour {
        enum PlatformState { AT_TARGET, TOWARDS_TARGET }

        [Tooltip("Calculate the velocity using the move time between the start position and first target position")]
        [SerializeField] bool calculateVelocityWithMoveTime = false;
        [ConditionalHide("calculateVelocityWithMoveTime", false, true)]
        [SerializeField] protected float maxVelocity = 3f;

        [ConditionalHide("calculateVelocityWithMoveTime", true)]
        [Tooltip("The time the platform takes to move between the start position and first target position")]
        [SerializeField] float moveTime = 5f;
        [Space]

        [SerializeField] protected bool useAcceleration = false;       // Platforms do not currently work if this is true
        [ConditionalHide("useAcceleration", true)]
        [SerializeField] float acceleration = 10f;           // not currently working with acceleration
        [Space]

        [Tooltip("An initial delay before the platform starts moving")]
        [SerializeField] private float startDelay = 0;

        [Tooltip("The time the platform waits at each target transform before moving again")]
        [SerializeField] float waitTime = 1f;
        [Space]

        [Tooltip("If true platform will stop once it reaches the last Transform in the array")]
        [SerializeField] bool singleUse = false;
        [ConditionalHide("singleUse", false, true)]
        [SerializeField] bool loopDestinations = true;              // should the platform go around the destination in a loop or should it go out and back

        [Tooltip("Should the platform snap to the exact target position once close enough")]
        [SerializeField] bool snapToDestination = true;

        [SerializeField] Transform[] targetTransformsArray;

        Vector3[] destinations;
        private Vector3 startPosition;
        private Vector3 currentDestination;

        private int destinationsPosition = 0;                       // the current destination target in the destinations array

        private float threshholdDistance = 0.1f;                    // The distance at which the platform will be considered at the target
        private float lastDistanceToDestination = float.MaxValue;   // the distance to the destination last frame used to check if the platform has 
                                                                    // ..gone past the destination.  
        private bool isWaitTimeUp = false;
        private bool isGoingForwards = true;                        // is the platform moving forwards through the destinations Array

        private PlatformState platformState;

        protected Rigidbody rb;
        protected Coroutine accelerateMoveRoutine;


        public void Awake() {
            startPosition = transform.position;
            rb = GetComponent<Rigidbody>();
            StartCoroutine(WaitTime(startDelay));

            SetDestinationsArray();
            if (calculateVelocityWithMoveTime) {
                CalculateMaxVelocity();
            }

            platformState = PlatformState.AT_TARGET;
            currentDestination = destinations[0];
        }


        private void SetDestinationsArray() {
            destinations = new Vector3[targetTransformsArray.Length + 1];
            destinations[0] = startPosition;
            for (int i = 0; i < targetTransformsArray.Length; i++) {
                if (targetTransformsArray[i] == null) {
                    Debug.LogError("PhysicsPlatforms: A Transform in the Target Transform Array is Null. Make sure each element of the array is assign a Transform");
                    return;
                }
                destinations[i + 1] = targetTransformsArray[i].position;
            }
        }

        // Calculate the max velocity based on how long it takes to move from start to first target position
        private void CalculateMaxVelocity() {
            float dst = Vector3.Distance(startPosition, destinations[1]);
            maxVelocity = dst / moveTime;

        }
        void FixedUpdate() {
            ControlPlatform();
            CheckPlatformVelocity();
        }

        private bool HasReachedDestination() {
            float dstToDest = Vector3.Distance(transform.position, currentDestination);
            if (dstToDest < threshholdDistance) {               // Check if platform is close enough to destination to be considered at the destination
                lastDistanceToDestination = float.MaxValue;
                return true;
            }
            if (dstToDest > lastDistanceToDestination) {        // if the platform is moving fast it may be possible for the platform to skip over the 
                lastDistanceToDestination = float.MaxValue;     // .. threshold distance from one frame to the next. This will check if the distance to the
                return true;                                    // .. destination has increased since the last frame
            }
            lastDistanceToDestination = dstToDest;
            return false;
        }

        private void ControlPlatform() {
            switch (platformState) {
                case PlatformState.AT_TARGET:
                    if (singleUse && destinationsPosition == destinations.Length - 1) {
                        return;
                    }
                    if (isWaitTimeUp) {
                        SetCurrentDestination();
                        platformState = PlatformState.TOWARDS_TARGET;

                        Vector3 moveDirection = (currentDestination - transform.position).normalized;
                        if (useAcceleration) {
                            accelerateMoveRoutine = StartCoroutine(MoveWithAcceleration(moveDirection, maxVelocity));
                        }
                        else {
                            MovePlatform(moveDirection, maxVelocity);
                        }
                    }
                    break;
                case PlatformState.TOWARDS_TARGET:
                    if (HasReachedDestination()) {
                        StartCoroutine(WaitTime(waitTime));
                        platformState = PlatformState.AT_TARGET;
                        StartCoroutine(StopPlatform(currentDestination));
                    }
                    break;
            }
        }

        // Sets the new current destination taking into consideration whether the platform should be going around in a loop, or going out to the last ..
        // .. destination and then back to the start the same what it came (i.e is loopDestinations true or false)
        private void SetCurrentDestination() {
            // Loop Destinations
            if (loopDestinations) {
                destinationsPosition++;
                if (destinationsPosition >= destinations.Length) {
                    destinationsPosition = 0;
                }
            }
            // Out and Back Destinations
            else {
                if (isGoingForwards) {
                    destinationsPosition++;
                    if (destinationsPosition == destinations.Length - 1) {
                        isGoingForwards = false;
                    }
                }
                else {
                    destinationsPosition--;
                    if (destinationsPosition == 0) {
                        isGoingForwards = true;
                    }
                }
            }
            currentDestination = destinations[destinationsPosition];        // set currentDestination to correct array position
        }

        // Sets the platform velocity immedietly to the max velocity
        void MovePlatform(Vector3 direction, float force) {
            rb.AddForce(direction * force, ForceMode.VelocityChange);
        }

        // Gradually increases the platform velocity to the max velocity
        protected IEnumerator MoveWithAcceleration(Vector3 direction, float velocityMax) {
            while (rb.velocity.magnitude < velocityMax) {
                rb.AddForce(direction * acceleration, ForceMode.Acceleration);
                yield return new WaitForFixedUpdate();
            }
        }

        protected IEnumerator StopPlatform(Vector3 destination) {
            // If acceleration is being used then in the eventuality in which the platform reaches the destination before reaching the max velocity, the ...
            // MoveWithAcelleration Coroutine must be stopped
            if (useAcceleration) {
                if (accelerateMoveRoutine != null)
                    StopCoroutine(accelerateMoveRoutine);
                yield return new WaitForFixedUpdate();      // Wait a frame between telling the routine to stop and actiually setting the velocity, this ...
                                                            // avoid a problem where the platform would continue to move slowly after reaching destination
            }
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, 0);       // Set platform velocity to zero
            if (snapToDestination)                                      // Snap the platform to the exact destination point
                transform.position = destination;
        }

        // Stops the platform from exeeding the maxVelocity
        void CheckPlatformVelocity() {
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }

        // Controls the start delay and the wait time at each destination for the platform
        IEnumerator WaitTime(float timeToWait) {
            isWaitTimeUp = false;
            yield return new WaitForSeconds(timeToWait);
            yield return new WaitForFixedUpdate();      // fixes a problem. Dont worry about it
            isWaitTimeUp = true;
        }

        // === LEVER PHYSICS PLATFORM CODE START ===
        Vector3 velocityWhenDisabled;
        private void OnEnable() {
            rb.AddForce(velocityWhenDisabled, ForceMode.VelocityChange);
            if (useAcceleration) {
                accelerateMoveRoutine = StartCoroutine(MoveWithAcceleration(velocityWhenDisabled.normalized, maxVelocity));
            }
        }
        private void OnDisable() {
            velocityWhenDisabled = rb.velocity;
            if (gameObject.activeInHierarchy) {
                    StartCoroutine(StopPlatform());
            }
        }

        protected IEnumerator StopPlatform() {
            // If acceleration is being used then in the eventuality in which the platform reaches the destination before reaching the max velocity, the ...
            // MoveWithAcelleration Coroutine must be stopped
            if (useAcceleration) {
                if (accelerateMoveRoutine != null)
                    StopCoroutine(accelerateMoveRoutine);
                yield return new WaitForFixedUpdate();      // Wait a frame between telling the routine to stop and actiually setting the velocity, this ...
                                                            // avoid a problem where the platform would continue to move slowly after reaching destination
            }
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, 0);       // Set platform velocity to zero
        }
        // === LEVER PHYSICS PLATFORM CODE END ===


        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            if (targetTransformsArray.Length > 0) {

                // Draw line between start position and last target transform
                if (rb != null) {       // checks if the game has been started yet
                    Gizmos.DrawLine(startPosition, targetTransformsArray[0].position);
                }
                else {
                    Gizmos.DrawLine(transform.position, targetTransformsArray[0].position);
                }

                // Draw line between each of the target transforms
                for (int i = 0; i < targetTransformsArray.Length - 1; i++) {
                    Gizmos.DrawLine(targetTransformsArray[i].position, targetTransformsArray[i + 1].position);
                }

                // Draw line between last target transform and start position
                if (!singleUse && loopDestinations) {   // only link the last and first position if the platform will actually move along this path
                    if (rb != null) {
                        Gizmos.DrawLine(targetTransformsArray[targetTransformsArray.Length - 1].position, startPosition);
                    }
                    else {
                        Gizmos.DrawLine(targetTransformsArray[targetTransformsArray.Length - 1].position, transform.position);
                    }
                }
            }
        }
    }
}
