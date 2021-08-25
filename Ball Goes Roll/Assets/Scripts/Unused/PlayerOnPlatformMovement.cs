using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnPlatformMovement : MonoBehaviour
{
    GameObject defaultParent;

    private void Awake() {
        defaultParent = transform.parent.gameObject;
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Platform")) {
            transform.SetParent(collision.transform.parent);
            
        }
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Platform")) {
            transform.SetParent(defaultParent.transform);
        }
    }
}
