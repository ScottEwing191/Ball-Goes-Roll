using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMeshOnStart : MonoBehaviour
{
    [SerializeField] bool hideMesh = true;
    private void Awake() {
        if (hideMesh) {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }   
}
