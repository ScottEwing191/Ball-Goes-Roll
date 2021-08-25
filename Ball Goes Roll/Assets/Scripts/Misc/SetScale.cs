using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScale : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        if (PlayerSingleton.Instance != null) {
            transform.localScale = PlayerSingleton.Instance.PlayerController.gameObject.transform.localScale;
        }
    }
    void OnEnable() {
        if (PlayerSingleton.Instance != null) {

            transform.localScale = PlayerSingleton.Instance.PlayerController.gameObject.transform.localScale;
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
