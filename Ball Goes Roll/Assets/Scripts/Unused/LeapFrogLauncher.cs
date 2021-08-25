using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapFrogLauncher : MonoBehaviour
{

    [SerializeField] private Rigidbody leepFrog;

    float maxHeight;
    [SerializeField] private Vector3 launchVelocity = new Vector3(22, 30, 0);

    [SerializeField] private float gravity = -18f;

    // Start is called before the first frame update
    void Start()
    {
        leepFrog.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)) {
            leepFrog.useGravity = true;
            leepFrog.velocity = launchVelocity;
        }
        if (leepFrog.transform.position.y > maxHeight) {
            maxHeight = leepFrog.transform.position.y;
        }
        DrawTrajectory();
    }


    public void DrawTrajectory() {
        float timeTillLand = 5.0f;
        Vector3 previousDisplacement = leepFrog.transform.position;
        int resolution = 30;

        for (int i = 0; i <= resolution; i++) {
            float simulatedTime = i / (float)resolution * timeTillLand;
            Vector3 displacement = launchVelocity * simulatedTime + Vector3.up * gravity * simulatedTime * simulatedTime / 2;        // suvat 3   s = ut + at^2 / 2
            Vector3 drawPoint = leepFrog.position + displacement;
            Debug.DrawLine(previousDisplacement, drawPoint, Color.blue);
            previousDisplacement = drawPoint;
        }
    }
   
}
