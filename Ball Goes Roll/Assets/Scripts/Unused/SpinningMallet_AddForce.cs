using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningMallet_AddForce : MonoBehaviour
{
    [SerializeField] GameObject model;  // the model that will spin
    [SerializeField] float spinSpeed = 10.0f;
    [SerializeField] float hitForce = 10.0f;
    
    [SerializeField] Transform mallotHead;      // the position at the head of the mallot head used to get mallot direction

    Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
       
    }

    // Update is called once per frame
    void Update()
    {

        model.transform.RotateAround(transform.up, Time.deltaTime * spinSpeed); 
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
        }
        print("Collision");*/
    }
}
