using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallAudio : MonoBehaviour {

    [SerializeField] private AudioSource rollSource;
    [SerializeField] private AudioSource splashSource;
    [SerializeField] private AnimationCurve rollVolumeCurve;
    [SerializeField] private AnimationCurve rollPitchCurve;
    private PlayerController playerController;


    // Start is called before the first frame update
    void Start() {
        playerController = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void Update() {
        if (PlayerSingleton.Instance.PlayerController.IsGrounded) {
            DoRollSound();
        }
        else {
            rollSource.Stop();
        }
    }

    private void DoRollSound() {
        if (PlayerSingleton.Instance.PlayerController.IsGrounded) {
            if (!rollSource.isPlaying) {
                rollSource.Play();

            }
            float speedScalar = CalculateSpeedScalar();
            rollSource.volume = rollVolumeCurve.Evaluate(speedScalar);
            rollSource.pitch = rollPitchCurve.Evaluate(speedScalar);

            //rollSource.volume = speedScalar;
            //rollSource.pitch = Mathf.Lerp(0.5f, 1.5f, speedScalar);

        }
        else {
            rollSource.Stop();
        }
    }
    // Calculate the value which will be used to scale the pitch and volume of the sound based on the speed of the ball
    private float CalculateSpeedScalar() {
        float velocityMagnitude = playerController.PlayerRigidbody.velocity.magnitude;
        float maxVelocity = playerController.MaxVelocity;
        float volume = velocityMagnitude / maxVelocity;
        Mathf.Clamp(volume, 0, 1);
        return volume;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Water")) {
            splashSource.Play();
        }
    }
}
