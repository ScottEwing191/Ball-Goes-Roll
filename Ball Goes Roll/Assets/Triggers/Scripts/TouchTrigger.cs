using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTrigger : Trigger {
    protected override void OnTriggerEnter(Collider other) {
        if (other.CompareTag(triggeredBy) && isActivatable) {
            onTriggerMethod.Invoke();
            base.OnTriggerEnter(other);
        }
    }
}
