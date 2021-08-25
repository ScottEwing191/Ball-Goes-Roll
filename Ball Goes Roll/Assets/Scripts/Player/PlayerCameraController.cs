using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float rotateSpeed = 50.0f;
    [SerializeField] private float verticalRotateSpeed = 25.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Horizontal Movement
        transform.position = player.transform.position;
        float mouseHorizontalMovement = Input.GetAxis("Mouse X");
        
        float horizontalMovement = Input.GetAxis("CameraHorizontal");
        if (horizontalMovement == 0) {
            horizontalMovement = mouseHorizontalMovement;
        }
        gameObject.transform.rotation *= Quaternion.AngleAxis(rotateSpeed * horizontalMovement * Time.deltaTime, Vector3.up);

        //Vertical Movement
        float mouseVerticalMovement = Input.GetAxis("Mouse Y");
        float verticalMovement = Input.GetAxis("CameraVertical");
        if (verticalMovement == 0) {
            verticalMovement = -mouseVerticalMovement;
        }
        gameObject.transform.rotation *= Quaternion.AngleAxis(verticalRotateSpeed * verticalMovement * Time.deltaTime, Vector3.right);

        Vector3 angles = transform.localEulerAngles;
        angles.z = 0;

        float angle = transform.localEulerAngles.x;

        if (angle > 180 && angle < 320) {
            angles.x = 320;
        }
        else if (angle < 180 && angle > 40) {
            angles.x = 40;
        }

        transform.localEulerAngles = angles;
    }
}
