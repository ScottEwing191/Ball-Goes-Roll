using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    [Header("Player Controller")]
    [SerializeField] private float speed = 100;
    [SerializeField] private float maxVelocity = 10.0f;
    [SerializeField] private float jumpHeight = 10;
    [SerializeField] private float inAirDrag = 1f;
    [SerializeField] private float inAirSpeed = 75f;
    private float groundCheckOffset = 0.4f;                                 // If the the collision point with the ground is bellow this distance relative to the centre of the ball then the ball                                                                          with be grounded
    [SerializeField] private float groundCheckOffsetPercentage = 0.457f;    // the percentage of the ball radius bellow the centre of the ball to use as the groundCheckOffset
    [SerializeField] private bool hasAirControl = true;
    // this allow the scale of the ball to change and the ground check still work the same.
    private bool onJumpSurface = true;                                      // is the ball touching a surface that it can jump from
    [Header("Leap Mechanic")]
    [SerializeField] private float minLeapTargetDistance = 2f;
    [SerializeField] private float maxLeapTargetDistance = 30f;
    [SerializeField] private bool isLeapMechanicEnabled = true;             // Is the leap mechanic currently enabled
    [SerializeField] private bool inLeapViewMode = false;                   // is th eplayer currently viewing where they can jump in llepfrog view mode.
    [SerializeField] private Vector3 leapVelocity = Vector3.zero;           // the velocity the ball will leap with. Calculated over multiple frames then applied to ball
    [SerializeField] private Transform leapTarget;
    [SerializeField] GameObject leapLandObject;
    private bool onLeapSurface = true;                                      // is the surface the ball is touch one that the ball can leap from


    [SerializeField] private bool isDEBUG = false;

    [SerializeField] private Transform parallelToGroundTransform;           // A transform whose forward vector is always parallel to the ground



    public bool isLeaping = false;
    public float maxAngularVelocity = 7;                                    // affects how fast player can roll down slopes i think. 

    private LineRenderer leapLine;                                          // The line that display the trajectory of the ball
    private float maxXZAirVelocity = 0;
    private float defaultDrag = 0.1f;
    //private float groundCheckRadius = 0.7f;                                 // How far out from the centre of the ball should the rays be cast from when doing ground check
    private bool isStillInTheAir = false;                                   // True if the ball is in the air and was also in the air in the previous frame. (Except first frame in air)
    [SerializeField] private bool isGrounded = true;                                         // is the ball currently grounded
    private bool hasGroundCheckBeenDoneThisFrame = false;                   // keeps track of whether ground check has already been done
    private bool shouldCheckLeverInteractInput = false;

    private LeapFrogMechanic leapFrogMechanic;
    private Rigidbody rb;
    private Vector3 startPos;                                               // used to reset the ball to start position in debug mode
    private Vector3 jumpStartVelocity;                                      // When the ball jumps or is in the air the velocity will be clamped to this amoult. Exept when leeping
    public float defaultAirVelocityMagnitude = 5f;                          // this is the velocity the ball will be able to get up to if jumping from a stand still
    private Collider playerCollider;                                        // the collider attached to the player used to check if player is grounded
    private Collider leverCollider;                                         // will be set to "other" collider OnTriggerEnter when the player is touching a lever trigger

    private PlayerInputHandler inputsManager;
    #region Properties
    public bool IsGrounded {
        get { return isGrounded; }
    }
    public bool IsLeaping {
        get { return isLeaping; }
    }
    public bool InLeapViewMode {
        get { return inLeapViewMode; }
    }
    public Rigidbody PlayerRigidbody {
        get { return rb; }
    }
    public float MaxVelocity {
        get { return maxVelocity; }
    }

    #endregion


    void Start() {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        leapFrogMechanic = GetComponent<LeapFrogMechanic>();
        defaultDrag = rb.drag;
        startPos = transform.position;
        leapLine = GetComponentInChildren<LineRenderer>();
        leapLine.gameObject.SetActive(false);
        inputsManager = GetComponentInParent<PlayerInputHandler>();
    }
    void FixedUpdate() {
        if (isGrounded && !isLeaping) {      // Regular movement
            rb.drag = defaultDrag;
            Movement();

        }
        else if (!isLeaping) {                  // in-air Movement
            Vector3 XZVelocity = Vector3.Scale(rb.velocity, (Vector3.right + Vector3.forward));

            maxXZAirVelocity = XZVelocity.magnitude;
            rb.drag = inAirDrag;
            if (!isStillInTheAir) {             // if this is the first frame off the ground set the velocity of the ball at this point
                isStillInTheAir = true;
                jumpStartVelocity = rb.velocity;
                //if (Vector3.Scale(jumpStartVelocity, Vector3.forward +Vector3.left).magnitude < defaultAirVelocityMagnitude) {                        // setting the minimum max velocity
                //    jumpStartVelocity =  jumpStartVelocity.normalized * defaultAirVelocityMagnitude;
                //}
            }
            InAirMovement(jumpStartVelocity);
        }


    }

    private void Update() {
        hasGroundCheckBeenDoneThisFrame = false;
        if (shouldCheckLeverInteractInput) {
            CheckLeverInteractInput();
        }
        SetLeapTargetYPosition();       // Can probably be removed when i dont want the leap target to be visible all the time
    }
    public void Respawn() {
        if (!inLeapViewMode) {     // Respawn the player if they are not in leap view mode
            PlayerSingleton.Instance.PlayerRespawn.Respawn();
            GameManager.Instance.ResetBall();
        }
    }
    private void CheckSkipToNextCheckpoint() {
        if (!inLeapViewMode && Input.GetButtonDown("NextCheckpoint")) {     // Respawn the player if they are not in leap view mode
            PlayerSingleton.Instance.PlayerRespawn.GoToNextCheckpoint();
        }
    }

    public void DoJump() {
        Vector3 jumpDirection = parallelToGroundTransform.TransformDirection(Vector3.up);
        //if (Input.GetButtonDown("Jump") && isGrounded && onJumpSurface) {
        if (isGrounded && onJumpSurface) {

            //rb.AddForce(new Vector3(0, jumpHeight, 0), ForceMode.Impulse);
            rb.AddForce(jumpDirection * jumpHeight, ForceMode.Impulse);
        }
    }


    private void SetLeapTargetYPosition() {

        leapTarget.position = new Vector3(leapTarget.position.x, transform.position.y, leapTarget.position.z);

    }

    // Input Triggered Version
    public IEnumerator StartLeapMode() {
        if (!isLeapMechanicEnabled) { yield break; }     // if the leap mechanic is not enabled then dont bother doing anything else
        if (!isGrounded) { yield break; }
        
        while (inputsManager.RunLeapRoutine) {

            // ENTER LEAP VIEW MODE
            if (!inLeapViewMode && isGrounded && onLeapSurface) {
                SetLeapModeVariableActiveState(true);

                rb.AddForce(-rb.velocity, ForceMode.VelocityChange);                // stop the ball when entering leap mode
                rb.AddTorque(-rb.angularVelocity, ForceMode.VelocityChange);
                leapVelocity = Vector3.zero;                                            // Resetting leap Velocity for next leap

            }
            // STOP LEAP SINCE BALL IS NOT ON GROUND
            else if (inLeapViewMode && !isGrounded) {
                SetLeapModeVariableActiveState(false);
            }
            // UPDATE 
            else if (inLeapViewMode && isGrounded) {           // passes every frame leep view mode while is active
                SetLeapModeVariableActiveState(true);

                //Target Position Method
                MoveLeapTarget();
                Vector3[] linepoints = leapFrogMechanic.GetPathPointsNoLimit();

                leapFrogMechanic.RenderPath(linepoints);        // render the ball tragectory line
                leapLandObject.transform.position = linepoints[linepoints.Length - 1];
            }
            yield return null;
        }
    }

    private void SetLeapModeVariableActiveState(bool active) {
        inLeapViewMode = active;
        leapTarget.gameObject.SetActive(active);
        leapLine.gameObject.SetActive(active);
        leapLandObject.SetActive(active);
    }
    public void Leap() {
        if (!isLeapMechanicEnabled) { return; }     // if the leap mechanic is not enabled then dont bother doing anything else

        if (InLeapViewMode) {
            inLeapViewMode = false;                   // Probably set this later
            leapTarget.gameObject.SetActive(false);

            //Target Position Method
            leapVelocity = leapFrogMechanic.CalculateLaunchData().initialVelocity;
            rb.AddForce(leapVelocity, ForceMode.VelocityChange);
            isLeaping = true;
            rb.drag = 0.0f;

            StartCoroutine("Leaping");
        }
    }

    public void CancelLeap() {
        if (!isLeapMechanicEnabled) { return; }     // if the leap mechanic is not enabled then dont bother doing anything else

        if (inLeapViewMode) {
            SetLeapModeVariableActiveState(false);
        }

    }
    // reset the is leaping bool once the ball has landed. also reset the drag
    IEnumerator Leaping() {
        yield return new WaitForSeconds(0.1f);          // wait a fraction of a second to make suure ball is off the ground. otherwise in air control and drag will be on while ball is in air

        while (!isGrounded) {
            yield return null;
        }
        isLeaping = false;
        leapTarget.gameObject.SetActive(false);
        leapLine.gameObject.SetActive(false);
        leapLandObject.SetActive(false);

        rb.drag = defaultDrag;
    }

    // Moves the target position that the ball will aim for
    void MoveLeapTarget() {
        float movementHorizontal = Input.GetAxis("Horizontal");
        float movementVertical = Input.GetAxis("Vertical");


        //Vector3 movementVector = new Vector3(0, 0.0f, movementVertical);
        Vector3 movementVector = inputsManager.LeapTargetMovementVector;


        movementVector = Camera.main.transform.TransformDirection(movementVector);

        movementVector.y = 0;
        Vector3 tempLeapTargetPosition = leapTarget.position;           // store the position of the leapTarget incase the new position is too close or far away from the player

        leapTarget.position += movementVector * Time.deltaTime * 20;

        if (!isDEBUG) {
            float distance = Vector3.Distance(transform.position, leapTarget.position);
            // if the new position of the leap target is too close or far away from the player then set it back to position it was before it was moved
            if (distance < minLeapTargetDistance || distance > maxLeapTargetDistance) {
                leapTarget.position = tempLeapTargetPosition;
            }
        }
    }


    void Movement() {

        rb.maxAngularVelocity = maxAngularVelocity;
        //float movementHorizontal = Input.GetAxis("Horizontal");
        //float movementVertical = Input.GetAxis("Vertical");

        //Vector3 movementVector = new Vector3(movementHorizontal, 0.0f, movementVertical);
        Vector3 movementVector = inputsManager.MovementVector;



        if (isGrounded || hasAirControl) {    // if not in the air

            movementVector = parallelToGroundTransform.TransformDirection(movementVector);


            if (!inLeapViewMode) {                                                      // only move ball if not in leap mode
                rb.AddForce(movementVector * speed * Time.deltaTime);
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);

            }
        }
    }

    Vector3 MakeVectorParallelToGround(Vector3 vector) {
        RaycastHit hit;
        if (true) {
            Physics.Raycast(transform.position, Vector3.down, out hit, (playerCollider.bounds.extents.y + 1f), 1 << LayerMask.NameToLayer("Ground"));
            print("Raycast Hit");
        }
        Vector3 perpendicularToGround = Vector3.Cross(Camera.main.transform.right, hit.normal);

        Debug.DrawLine(transform.position, transform.position + (perpendicularToGround * 10), Color.black);
        Vector3 forceDirection = ((transform.position + perpendicularToGround) - transform.position).normalized;

        return forceDirection;
    }

    void InAirMovement() {      // This Works
        Vector3 movementVector = inputsManager.MovementVector;
        if (hasAirControl) {    // if not in the air
            movementVector = Camera.main.transform.TransformDirection(movementVector);
            movementVector.Scale(Vector3.right + Vector3.forward);                      // add sforce forwards indepentand of camera pitch
            // only move ball if not in leap mode
            rb.AddForce(movementVector * inAirSpeed * Time.deltaTime);
        }
    }

    public void StopBall() {
        rb.AddForce(-rb.velocity, ForceMode.VelocityChange);
        rb.AddTorque(-rb.angularVelocity, ForceMode.VelocityChange);
    }

    void InAirMovement(Vector3 startVelocity) {

        Vector2 startVelocityXZ = new Vector2(startVelocity.x, startVelocity.z);    // dont let the player XZ magnitude increase beyon this
        Vector2 currentVelocityXZ = Vector2.zero;

        float movementHorizontal = Input.GetAxis("Horizontal");
        float movementVertical = Input.GetAxis("Vertical");

        Vector3 movementVector = new Vector3(movementHorizontal, 0.0f, movementVertical);

        if (hasAirControl) {    // if not in the air
            movementVector = Camera.main.transform.TransformDirection(movementVector);
            movementVector.Scale(Vector3.right + Vector3.forward);                      // add force forwards indepentant of camera pitch (sets y compnent to 0)

            // only move ball if not in leap mode
            rb.AddForce(movementVector * inAirSpeed * Time.deltaTime);

            // Check if velocity has increased and undo it if it has
            currentVelocityXZ = new Vector2(rb.velocity.x, rb.velocity.z);
            // Ball can maintain speed it had when it went into air and speed up to a minimum speed
            if (currentVelocityXZ.sqrMagnitude > startVelocityXZ.sqrMagnitude && currentVelocityXZ.magnitude > defaultAirVelocityMagnitude) {
                //print("Velocity Clamped");
                rb.AddForce(-movementVector * inAirSpeed * Time.deltaTime);
            }
        }
    }

    private void CheckLeverInteractInput() {
        if (Input.GetButtonDown("Interact")) {
            LeverController lever = leverCollider.GetComponent<LeverController>();
            StartCoroutine(lever.MoveLeverToFinalPosition(-lever.leverState));      // pass in - lever state to switch it to other position
            if (lever.ShouldOnlyActivateOnce) {
                //Hide UI Messgae
                UILevelManager.Instance.InteractPanel.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Lever")) {
            shouldCheckLeverInteractInput = true;
            leverCollider = other;
            LeverController leverController = other.GetComponent<LeverController>();
            // Display UI Message
            UILevelManager.Instance.InteractPanel.SetActive(true);
            if (leverController.ShouldOnlyActivateOnce && leverController.HasBeenActivated) {
                UILevelManager.Instance.InteractPanel.SetActive(false);
            }
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Lever")) {
            shouldCheckLeverInteractInput = false;
            leverCollider = null;
            // Hide UI Message

            UILevelManager.Instance.InteractPanel.SetActive(false);


        }
    }

    private void OnCollisionEnter(Collision collision) {
        CheckIfGrounded(collision);                         // Check if the ball is grounded and set is grounded variable
        CheckIfOnJumpSurface(collision);                    // Check if the surface the ball is touch is one that jump from
        CheckIfOnLeapSurface(collision);                    //// Check if the surface the ball is touch is one that leap from
    }


    private void OnCollisionStay(Collision collision) {
        CheckIfGrounded(collision);
        CheckIfOnLeapSurface(collision);
        CheckIfOnJumpSurface(collision);


    }

    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            isGrounded = false;
        }
    }

    void CheckIfGrounded(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {                // Is ball colliding with ground
            if (hasGroundCheckBeenDoneThisFrame && isGrounded) {                            // has the ball already be found to be touching the ground this frame
                return;
            }
            ContactPoint[] contactPoints = new ContactPoint[10];
            int numberOfPoints = collision.GetContacts(contactPoints);
            groundCheckOffset = playerCollider.bounds.extents.x * groundCheckOffsetPercentage;
            for (int i = 0; i < numberOfPoints; i++) {
                if (contactPoints[i].point.y <= transform.position.y - groundCheckOffset) {  // Check if collision point is far enougn down on the ball for the ball to be grounded            
                    isStillInTheAir = false;                                                 // Ball will no longer have been in the air on the previous frame (after this frame)
                    hasGroundCheckBeenDoneThisFrame = true;
                    isGrounded = true;
                    return;
                }
            }
            hasGroundCheckBeenDoneThisFrame = true;
            isGrounded = false;
        }
    }
    private void CheckIfOnJumpSurface(Collision collision) {
        if (!isGrounded) {
            onJumpSurface = false;
        }
        else if (collision.gameObject.CompareTag("NoJumpSurface")) {
            onJumpSurface = false;
        }
        else {
            onJumpSurface = true;
        }
    }

    private void CheckIfOnLeapSurface(Collision collision) {
        if (!isGrounded) {
            onLeapSurface = false;
        }
        else if (collision.gameObject.CompareTag("NoLeapSurface")) {
            onLeapSurface = false;
        }
        else {
            onLeapSurface = true;
        }
    }
}
