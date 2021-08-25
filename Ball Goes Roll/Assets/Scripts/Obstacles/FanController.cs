using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanController : MonoBehaviour
{
    [SerializeField] float fanSpeed = 10.0f;
    [SerializeField] float fanHeight = 8f;
    [SerializeField] Transform fanOrigin;       // the Transform at which the fan will be most powerful
    [SerializeField] CapsuleCollider fanCollider;

    private float gradient;
    //[SerializeField] float upwardsModifier = 0;
    
    

    //private Rigidbody playerRigidbody;
    // Start is called before the first frame update
    void Start()
    {
        fanCollider.height = fanHeight;
        fanCollider.center = new Vector3(fanCollider.center.x, fanCollider.height / 2f - fanCollider.radius, fanCollider.center.z);
        gradient = -fanSpeed / ((fanHeight - fanCollider.radius) * (fanHeight - fanCollider.radius));
    }

    // Update is called once per frame
    void Update()
    {
        fanCollider.height = fanHeight;
        fanCollider.center =  new Vector3(fanCollider.center.x, fanCollider.height / 2f - fanCollider.radius, fanCollider.center.z);
        //print(fanCollider.bounds.extents.magnitude);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            //playerRigidbody = other.attachedRigidbody;
        }
    }
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            // fan will add les force to the ball depending on ball distance from centre of fan using this equation:
            // y = mx^2 + c
            // fanPower = -0.1 * distance^2 + maxFanPower


            float dstFromPowerOrigin = Vector3.Distance(other.transform.position, fanOrigin.position);
            //float adjustedFanSpeed =  -0.1f * dstFromPowerOrigin * dstFromPowerOrigin + fanSpeed;
            float adjustedFanSpeed = gradient * dstFromPowerOrigin * dstFromPowerOrigin + fanSpeed;
            adjustedFanSpeed = Mathf.Clamp(adjustedFanSpeed, 0, fanSpeed);

            if (adjustedFanSpeed < 0) {
                Debug.LogError("Fan Controller: Adjusted Fan Speed is less than 0");

            }
            //print("Fan Speed: " + adjustedFanSpeed);
            other.attachedRigidbody.AddForce(transform.up * adjustedFanSpeed, ForceMode.Acceleration);
            
            //float explosionRadius = fanCollider.height - fanCollider.radius;
            //other.attachedRigidbody.AddExplosionForce(fanSpeed, fanOrigin.position, explosionRadius, upwardsModifier, ForceMode.Acceleration);
            //other.attachedRigidbody.AddTorque(-other.attachedRigidbody.angularVelocity);

        }
    }

    /*float CalculateFanSpeed(float dist) {
        float adjustedFanSpeed = -0.1f * dist * dist + fanSpeed;
    }*/
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Player")) {
            //playerRigidbody = null;
        }
    }
    

}
