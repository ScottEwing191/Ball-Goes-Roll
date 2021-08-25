using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCentreOfMass : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] Transform centreOfMass;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centreOfMass.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        rb.inertiaTensorRotation = Quaternion.identity;
    }
}
