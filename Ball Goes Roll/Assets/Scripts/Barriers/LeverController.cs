using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverController : MonoBehaviour
{
    public GameObject m_Handle;
    [SerializeField] BarrierController barrierController;
    [SerializeField] static float m_OnRotation = -30f;      // the angle of the lever on/off position
    static float m_OffRotation = -m_OnRotation;

    [SerializeField] float m_RotateSpeed = 1;
    [SerializeField] bool m_IsLeverOn = true;
    [SerializeField] bool m_ShouldOnlyActivateOnce;           // should the lever only be activatable once
    bool m_HasBeenActivated;       // has the lever already been activated

    [HideInInspector] public float leverState;                // 1 when lever is on -1 when lever is off. Can be passed into Move To Final Position Coroutine
    Quaternion m_OnQuaternion;
    Quaternion m_OffQuaternion;

    AudioSource leverSource;

    Color m_Color;     // used to set the barrier to the same colour as the lever


    public bool ShouldOnlyActivateOnce {
        get { return m_ShouldOnlyActivateOnce; }
    }
    public bool HasBeenActivated {
        get { return m_HasBeenActivated; }
    }



    private void Awake()
    {
        Vector3 startRotation = transform.rotation.eulerAngles;
        Vector3 onRotationEuler = startRotation + new Vector3(0, 0, m_OnRotation);
        Vector3 offRotationEuler = startRotation + new Vector3(0, 0, -m_OnRotation);
        // add the on/ odd rotation to the initial rotation of the lever to get the correct Quaternion
        m_OnQuaternion = Quaternion.Euler(onRotationEuler);
        m_OffQuaternion = Quaternion.Euler(offRotationEuler);


        if (m_IsLeverOn)
        {
            m_Handle.transform.rotation = m_OnQuaternion;
            leverState = 1f;
        }
        else
        {
            m_Handle.transform.rotation = m_OffQuaternion;
            leverState = -1f;
        }
    }

    private void Start()
    {
        if (m_IsLeverOn)
        {
            barrierController.m_IsActive = true;
        }
        else
        {
            barrierController.m_IsActive = false;
        }
        SetBarrierColour();
        leverSource = GetComponent<AudioSource>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, barrierController.transform.position);
    }

    // This moves the lever while it is being held by the hand
    public void MoveLever(float xInput)
    {
        // I WAS DOING THIS
        if (m_ShouldOnlyActivateOnce && m_HasBeenActivated)
        {
            return;
        }
        float t = xInput / 2 + 0.5f;     // this line converts the xInput which will be between -1 and 1 into the appropriate number between 0 and 1 which can be used in the lerp method.
        //m_Handle.transform.rotation = Quaternion.Lerp(m_OffQuaternion, m_OnQuaternion, t);

        Quaternion targetRotation = Quaternion.Lerp(m_OffQuaternion, m_OnQuaternion, t);
        m_Handle.transform.rotation = Quaternion.RotateTowards(m_Handle.transform.rotation, targetRotation, Time.deltaTime *m_RotateSpeed);
        
    }

    // This moves the lever to its final position after the hand has released the lever
    public IEnumerator MoveLeverToFinalPosition(float xInput)
    {
        if (m_ShouldOnlyActivateOnce && m_HasBeenActivated)
        {
            yield break;
        }
        PlaySound();            // May wat to delete this in future
        bool keepLooping = true;
        Quaternion targetRotation = m_OnQuaternion;     // on by default
        if(xInput < 0)        
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

    private void PlaySound() {
        if (leverSource != null) {
            leverSource.Play(); 
        }
    }

    private void SetLeverState() {
        if (m_Handle.transform.rotation == m_OnQuaternion) {
            leverState = 1f;
        }
        else if (m_Handle.transform.rotation == m_OffQuaternion) {
            leverState = 0f;

        }
    }

    void ControllBarrier()
    {
        if (m_Handle.transform.rotation == m_OnQuaternion)
        {
            barrierController.SetActive(true);
        }
        else if (m_Handle.transform.rotation == m_OffQuaternion)
        {
            barrierController.SetActive(false);
        }
    }

    // Sets the barrier that this lever is attached to the same colour as this lever
    void SetBarrierColour()
    {
        PickObjectColor lever = GetComponent<PickObjectColor>();
        // Unity autimatically knows to use getcomponets from the game object that barriercontroller is on.
        MeshRenderer[] meshRenderers = barrierController.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshRenderers)
        {
            mesh.material.color = lever.m_Color;
        }
    }
}
