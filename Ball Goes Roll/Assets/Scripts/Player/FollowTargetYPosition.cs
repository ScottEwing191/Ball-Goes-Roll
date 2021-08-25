using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetYPosition : MonoBehaviour
{
    [SerializeField] Transform target;
    Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        offset = target.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 temp =  new Vector3(transform.position.x, target.position.y, transform.position.z);
        transform.position = new Vector3(transform.position.x, target.position.y, transform.position.z);


        //transform.SetPositionAndRotation(temp, transform.rotation);
        
        //transform.position = Vector3.Scale(Vec)
    }
}
