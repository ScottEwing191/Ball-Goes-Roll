using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ScottEwing.MovingObjects.MovingPhysicsPlatform3D {
    public class LeverController : MonoBehaviour {
        public GameObject m_Handle;
        [SerializeField] PhysicsPlatform physicsPlatform;
        [SerializeField] static float m_OnRotation = -30f;          // the angle of the lever on/off position
        static float m_OffRotation = -m_OnRotation;

        [SerializeField] float m_RotateSpeed = 1000;
        [SerializeField] bool m_IsLeverOn = true;
        [SerializeField] bool m_ShouldOnlyActivateOnce = false;     // should the lever only be activatable once
        bool m_HasBeenActivated;                                    // has the lever already been activated

        [HideInInspector] public float leverState;      // 1 when lever is on -1 when lever is off. Can be passed into Move To Final Position Coroutine
        Quaternion m_OnQuaternion;
        Quaternion m_OffQuaternion;

        bool shouldCheckForLeverInput = false;

        public bool ShouldOnlyActivateOnce {
            get { return m_ShouldOnlyActivateOnce; }
        }
        public bool HasBeenActivated {
            get { return m_HasBeenActivated; }
        }

        private void Awake() {
            Vector3 startRotation = transform.rotation.eulerAngles;
            Vector3 onRotationEuler = startRotation + new Vector3(0, 0, m_OnRotation);
            Vector3 offRotationEuler = startRotation + new Vector3(0, 0, -m_OnRotation);
            // add the on/ off rotation to the initial rotation of the lever to get the correct Quaternion
            m_OnQuaternion = Quaternion.Euler(onRotationEuler);
            m_OffQuaternion = Quaternion.Euler(offRotationEuler);

            if (m_IsLeverOn) {
                m_Handle.transform.rotation = m_OnQuaternion;
                leverState = 1f;
            }
            else {
                m_Handle.transform.rotation = m_OffQuaternion;
                leverState = -1f;
            }
        }

        private void Start() {
            if (m_IsLeverOn)
                physicsPlatform.enabled = true;     // Lets the platform move
            else
                physicsPlatform.enabled = false;    // Stops the platform from moving
        }

        

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, physicsPlatform.transform.position);
        }

        // This moves the lever while it is being held by the hand
        public void MoveLever(float xInput) {
            // I WAS DOING THIS
            if (m_ShouldOnlyActivateOnce && m_HasBeenActivated) {
                return;
            }
            float t = xInput / 2 + 0.5f;     // this line converts the xInput which will be between -1 and 1 into the appropriate number between 0 and 1 which can be used in the lerp method.
            Quaternion targetRotation = Quaternion.Lerp(m_OffQuaternion, m_OnQuaternion, t);
            m_Handle.transform.rotation = Quaternion.RotateTowards(m_Handle.transform.rotation, targetRotation, Time.deltaTime * m_RotateSpeed);
        }

        // This moves the lever to its final position after the hand has released the lever
        public IEnumerator MoveLeverToFinalPosition(float xInput) {
            if (m_ShouldOnlyActivateOnce && m_HasBeenActivated) {
                yield break;
            }
            bool keepLooping = true;
            Quaternion targetRotation = m_OnQuaternion;     // on by default
            if (xInput < 0)
                targetRotation = m_OffQuaternion;           //Set to off if input is less than 0        

            while (keepLooping)                             // loops until the lever is at the target position 
            {
                m_Handle.transform.rotation = Quaternion.RotateTowards(m_Handle.transform.rotation, targetRotation, Time.deltaTime * m_RotateSpeed);
                if (m_Handle.transform.rotation == targetRotation)
                    keepLooping = false;
                yield return null;
            }
            m_HasBeenActivated = true;
            ControllBarrier();
            SetLeverState();
        }

        private void SetLeverState() {
            if (m_Handle.transform.rotation == m_OnQuaternion)
                leverState = 1f;
            else if (m_Handle.transform.rotation == m_OffQuaternion)
                leverState = -1f;                                        
        }

        void ControllBarrier() {
            if (m_Handle.transform.rotation == m_OnQuaternion)
                physicsPlatform.enabled = true;
            else if (m_Handle.transform.rotation == m_OffQuaternion)
                physicsPlatform.enabled = false;
        }


        // === Activating the Lever START ===
        private void Update() {
            // If the lever is touching the player and the submit key is pressed then activate the lever
            if (shouldCheckForLeverInput && Input.GetButtonDown("Submit")) {
                StartCoroutine(MoveLeverToFinalPosition(-leverState));      // pass in - lever state to switch it to other position
            }
        }
        private void OnTriggerEnter(Collider other) {
            if (other.CompareTag("Player")) {
                shouldCheckForLeverInput = true;
            }
        }
        private void OnTriggerExit(Collider other) {
            if (other.CompareTag("Player")) {
                shouldCheckForLeverInput = false;
            }
        }
        // === Activating the Lever END ===
    }
}
