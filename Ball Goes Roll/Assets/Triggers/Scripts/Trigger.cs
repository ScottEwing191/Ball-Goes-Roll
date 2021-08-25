using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Trigger : MonoBehaviour
{
    [SerializeField] protected bool activateOnce = true;
    [SerializeField] protected bool isActivatable = true;

    [SerializeField] protected UnityEvent onTriggerMethod;
    [SerializeField] protected string triggeredBy = "Player";

    public bool IsActivatable {
        get { return isActivatable; }
        set { isActivatable = value; }
    }



    protected virtual void OnTriggerEnter(Collider other) {
        if (activateOnce) {
            Destroy(this.gameObject);
        }
    }
}
