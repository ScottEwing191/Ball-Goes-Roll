using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ScottEwing.MovingObjects.MovingPhysicsPlatform3D {
    public class HideMeshOnStart : MonoBehaviour {
        [SerializeField] bool hideMesh = true;
        private void Awake() {
            if (hideMesh) {
                GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
}
