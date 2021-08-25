using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScale : MonoBehaviour
{
    [SerializeField] float scaleSpeed = 1;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.O)) {
            transform.localScale -= Vector3.one * (scaleSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.P)) {
            transform.localScale += Vector3.one * (scaleSpeed * Time.deltaTime);
        }
    }
}
