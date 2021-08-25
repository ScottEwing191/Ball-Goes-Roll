using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToRestRotation : MonoBehaviour {
    Rigidbody rb;
    [SerializeField] SpinningObject spinningObject;
    [SerializeField] float rotateBackTime = 7.5f;           // the amout of time the platform will take to rotate back if it is perpidicular to the start rotation
    [SerializeField] float rotateResetDelay = 0.25f;
    Quaternion startRotation;
    Coroutine restRotationRoutine;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        startRotation = transform.rotation;
        //restRotationRoutine = GoToRestRotation();
    }

    // Update is called once per frame
    void Update() {
        /*if (Input.GetKeyDown(KeyCode.L)) {
            test = !test;
            restRotationRoutine = StartCoroutine(GoToRestRotation());

        }*/
    }

    //bool test = true;

    IEnumerator GoToRestRotation() {
        yield return new WaitForSeconds(rotateResetDelay);
        spinningObject.enabled = false;
        Vector3 targetRotationVector3 = startRotation.eulerAngles;
        targetRotationVector3.y = transform.rotation.eulerAngles.y;                     // change the initial rotation euler Y component to the current Y rotation
        Quaternion targetRotationQuaternion = Quaternion.Euler(targetRotationVector3);  // Convert the new Euler target rotation back to a quaternion
        Quaternion oldRotation = transform.rotation;

        float angleDifference = Quaternion.Angle(targetRotationQuaternion, oldRotation) % 360;
        float maxTime = angleDifference / 180 * rotateBackTime;
        float time = 0;

        rb.AddTorque(-rb.angularVelocity, ForceMode.VelocityChange);
        rb.inertiaTensorRotation = Quaternion.identity;                                 // not sure what difference this makes if any

        //while (time <= rotateBackTime) {
        while (time <= maxTime) {

            //transform.rotation = Quaternion.Slerp(oldRotation, targetRotationQuaternion, time / rotateBackTime);
            transform.rotation = Quaternion.Slerp(oldRotation, targetRotationQuaternion, time / (maxTime));
            time += Time.deltaTime;
            yield return null;
        }
        spinningObject.enabled = true;
        //print("Routine Ended");
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            StopAllCoroutines();
            if (!spinningObject.enabled) {                  // platform should always be spinning if the player is on it
                spinningObject.enabled = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            restRotationRoutine = StartCoroutine(GoToRestRotation());
        }
    }
}
