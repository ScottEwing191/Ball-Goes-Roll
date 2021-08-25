using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLauncher : MonoBehaviour {
    [SerializeField] private Rigidbody ball;
    [SerializeField] private Transform target;

    [SerializeField] float h = 25;      // Height of ball ??
    [SerializeField] float gravity = -18;

    [SerializeField] bool debugPath = true;



    private void Start() {
        ball.useGravity = false;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Launch();
        }

        if (debugPath) {
            DrawPath();
        }
    }


    void Launch() {
        Physics.gravity = Vector3.up * gravity;         // Undo this later
        ball.useGravity = true;
        ball.velocity = CalculateLaunchData().initialVelocity;
        print(ball.velocity);
    }
    
    // the initial launch velocity (speed and direction)
    LaunchData CalculateLaunchData() {
        float displacementY = target.position.y - ball.position.y;      // Calculate Py from diagram
        Vector3 displacementXZ = new Vector3(target.position.x - ball.position.x, 0, target.position.z - ball.position.z);      // Calculate Px but for XZ axis not just X 
        float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (displacementY - h) / gravity);    // This is  Thorizontal  = Tup + Tdown
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2.0f * gravity * h);       // This is the initial up Velocity

        Vector3 velocityXZ = displacementXZ / time;   // this is  horizontal velocity Uh (the one that uses Tup + Tdown)
        return new LaunchData( velocityXZ + velocityY, time);      // velocityXZ has ) as Y value and velocityY has 0 as XZ values so this just combines them
    }

    void DrawPath() {
        LaunchData launchData = CalculateLaunchData();
        Vector3 previousDrawPoint = ball.position;
        int resolution = 30;        // how many times are we checking the path when drawing the line

        for (int i = 0; i <= resolution; i++) {
            float simulationTime = i / (float)resolution * launchData.timeTotarget;     // gives a variable going from 0 to the timeToTarget over the course of the for loop
            Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;      // using 3rd suvat equation  s = ut + at^2 / 2
            Vector3 drawPoint = ball.position + displacement;
            Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
            previousDrawPoint = drawPoint;
        }
    }

    struct LaunchData {
        public readonly Vector3 initialVelocity;
        public readonly float timeTotarget;

        public LaunchData(Vector3 initialVelocity, float timeTotarget) {
            this.initialVelocity = initialVelocity;
            this.timeTotarget = timeTotarget;
        }
    }
}
