using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlledPlatform : MonoBehaviour {
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float maxVelocity = 0.5f;


    Rigidbody rb;


    void Start() {
        rb = GetComponent<Rigidbody>();
    }
    private void OnDrawGizmos() {
        //Gizmos.DrawLine(transform.position, transform.position + (new Vector3(transform.up.x, 0, transform.up.z) * 10));
    }

    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Vector3 moveDirection = new Vector3(transform.up.x, 0, transform.up.z).normalized;      // the platforms up vectot with no Y component
            rb.AddForce(-moveDirection * moveSpeed, ForceMode.Acceleration);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
        }

    }
    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            rb.AddForce(-rb.velocity, ForceMode.VelocityChange);

            //StartCoroutine(GoToRestRotation());
            //Vector3 angularVelocityXZ = Vector3.Scale(rb.angularVelocity, (Vector3.left + Vector3.right));
            //rb.angularVelocity = angularVelocityXZ;
        }
    }

    private IEnumerator GoToRestRotation() {
        print("Routine Started");
        Debug.LogError("Routine doesnt work YET");

        Vector3 targetRotation = new Vector3(0, transform.rotation.y, 0);
        float time = 0;
        bool exit = false;
        while ((transform.rotation.x < -1 || transform.rotation.x > 1 && transform.rotation.z < -1 || transform.rotation.z > 1) || exit) {
            transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, targetRotation, time));
            time += Time.deltaTime;
            if (time>5) {
                exit = true;
            }
            yield return null;
        }
        print("Routine Finished");

    }
}
