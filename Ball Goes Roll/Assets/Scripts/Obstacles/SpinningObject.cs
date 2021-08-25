using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningObject : MonoBehaviour {
    //[SerializeField] GameObject model;  // the model that will spin
    /*[SerializeField] float spinSpeed = 10.0f;
    [SerializeField] float hitForce = 10.0f;*/
    [SerializeField] float torque = 7;
    [SerializeField] float maxAngularVelocity = 5f;
    //[SerializeField] Transform mallotHead;      // the position at the head of the mallot head used to get mallot direction
    [SerializeField] Transform centreOfMass;

    [SerializeField] float startDelay = 0f;
    [SerializeField] bool quickStartOn = false;
    [SerializeField] bool reverseDirection = false;
    [SerializeField] bool rotateAroundTransformUp = true;

    bool isQuickStartRunning = false;

    float reverseMultiplier = 1f;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start() {

        rb = GetComponent<Rigidbody>();
        rb.centerOfMass -= rb.centerOfMass - centreOfMass.localPosition;
        rb.maxAngularVelocity = maxAngularVelocity;
        if (reverseDirection) {
            reverseMultiplier = -1;
        }

        StartCoroutine(QuickStart(startDelay));

    }

    private void FixedUpdate() {
        if (!isQuickStartRunning) {
            rb.centerOfMass -= rb.centerOfMass - centreOfMass.localPosition;            // Just here so mallets can be moves while playing. Also neccesary if being mobed by barrier script
            if (rotateAroundTransformUp) {
                rb.AddTorque(transform.up * torque * reverseMultiplier, ForceMode.Acceleration); 
            }
            else {
                rb.AddTorque(Vector3.up * torque * reverseMultiplier, ForceMode.Acceleration);

            }



        }
        //rb.AddRelativeTorque(transform.up * torque, ForceMode.Acceleration);
        //rb.angularVelocity = transform.up * torque;
    }

    IEnumerator QuickStart(float delay) {
        isQuickStartRunning = true;
        yield return new WaitForSeconds(delay);
        if (quickStartOn) {
            rb.AddTorque(transform.up * maxAngularVelocity * reverseMultiplier, ForceMode.VelocityChange);
        }
        isQuickStartRunning = false;
    }


    private void OnEnable() {
        StartCoroutine(QuickStart(startDelay));
    }


    // Update is called once per frame
    void Update() {

        //model.transform.RotateAround(transform.up, Time.deltaTime * spinSpeed); 
        //mallotHead.RotateAround(transform.up, Time.deltaTime * spinSpeed);


        //Debug.DrawRay(mallotHead.position,  mallotHead.forward * 4, Color.black);
    }

    private void OnCollisionEnter(Collision collision) {
        /*if (collision.gameObject.CompareTag("Player")) {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            playerRb.AddForce(-playerRb.velocity, ForceMode.VelocityChange);
            playerRb.AddTorque(-playerRb.angularVelocity, ForceMode.VelocityChange);

            Vector3 mallotDir = mallotHead.forward;
            Debug.DrawRay(mallotHead.position, mallotHead.position * 4, Color.black);
            //mallotDir = mallotHead.TransformDirection(mallotDir);
            playerRb.AddForce(mallotHead.forward * hitForce * Time.deltaTime, ForceMode.Impulse);
        }*/
        //print("Collision");
    }
}
