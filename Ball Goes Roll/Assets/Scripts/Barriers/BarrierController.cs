using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    [SerializeField] Transform m_TargetTransform;       // The Transform the barrier will move

    Vector3 m_StartPosition;
    Quaternion m_StartRotation = Quaternion.identity;
    Vector3 m_StartScale = Vector3.zero;

    Vector3 m_TargetPosition = Vector3.zero;
    Quaternion m_TargetRotation = Quaternion.identity;
    Vector3 m_TargetScale = Vector3.zero;

    
    [SerializeField] bool m_ShouldMoveOnce;             // true if barrier should stop moving once it has reached its target position
    [SerializeField] bool m_IsActivatedAtStart;         // If this barrier is attached to a lever then the lever will overite the control of the m_IsActive variable.
    [HideInInspector] public bool m_IsActive = false;   // Set at awake by isActiveAtStart, then changed by lever (if one exists), giving the lever final control of wheter barrier is moving at start

    [SerializeField] float m_MoveTime = 5f;
    [SerializeField] float m_WaitTime = 1f;             // the time the barrier will wait before returning to other position

    bool m_IsCoroutineRunning;                          // Used to keep track of whether the coroutine is running, and also stop multiple instances of it being run at once
    
    bool m_MoveStartToTarget = true;                    // These two variables will be used in the move barrier enumerator. When the lever switches the barrier off the enumerator will...
    float m_ElapsedTime = 0;                            // ... save the elapsed time as well as whether the barrier is moving from the start position to the target or vice versa and ...
                                                        // ... break from the enumerator. When the lever start the barrier again the enumerator can use these variables to start moving ...
                                                        // ... barrier from where is was.

    // Start is called before the first frame update
    void Awake()
    {        
        // Set Start Transform
        m_StartPosition = transform.position;           
        m_StartRotation = transform.rotation;
        m_StartScale = transform.localScale;
        
        // Set Target Transform Positions
        m_TargetPosition = m_TargetTransform.position;
        m_TargetRotation = m_TargetTransform.rotation;
        m_TargetScale = m_TargetTransform.localScale;

        if (m_IsActivatedAtStart)
        {
            m_IsActive = true;
        }
        //OnDrawGizmosSelected();
    }

    private void Start()
    {
        if (m_IsActive)
        {            
            StartCoroutine(MoveBarrier());
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, m_TargetTransform.position);
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            m_IsActive = true;
            if (!m_IsCoroutineRunning)      // this stop the coroutine from being started multiple times.
            {
                StartCoroutine(MoveBarrier());
            }
        }
        else
        {
            m_IsActive = false;
        }        
    }

    /*
     *  Barrier will move from its start location to target location. If shouldMoveOnce is true it will stay at target position.
     *  If Lever is switched off while barrier is moving, Barrier will stop at its current position.
     *  When lever is turned on again barrier will continue moving from where is was.
     *  Elapsed time variable is used to keep track of which position between the start and end point that the barrier should be at.
     *  The MoveStartToTarget boolean variable is used to keep track of whether the barrier should be moving from the start position the the end position or vidce versa.
    */
    IEnumerator MoveBarrier()
    {
        m_IsCoroutineRunning = true;        
        WaitForSeconds waitTime = new WaitForSeconds(m_WaitTime);

        // MOVE ONCE
        if (m_ShouldMoveOnce)
        {            
            while (m_ElapsedTime < 1)                 // will move until barrier is at target Position
            {
                m_ElapsedTime += Time.deltaTime / m_MoveTime;
                transform.position = Vector3.Lerp(m_StartPosition, m_TargetPosition, m_ElapsedTime);
                transform.rotation = Quaternion.Lerp(m_StartRotation, m_TargetRotation, m_ElapsedTime);
                //transform.localScale = Vector3.Lerp(currentScale, m_TargetScale, elapsedTime);
                yield return null;
            }
            m_ElapsedTime = 0;
        }
        // MOVE BACK AND FORTH        
        while (m_IsActive && !m_ShouldMoveOnce)     // will move while barrier is active
        {
            
            // Barrier moving to target
            while (m_ElapsedTime < 1 && m_MoveStartToTarget)                 // will move until barrier is at target Position
            {
                if (!m_IsActive)
                {
                    m_MoveStartToTarget = true;
                    m_IsCoroutineRunning = false;
                    yield break;
                }
                m_ElapsedTime += Time.deltaTime / m_MoveTime;
                transform.position = Vector3.Lerp(m_StartPosition, m_TargetPosition, m_ElapsedTime);          // change position
                transform.rotation = Quaternion.Lerp(m_StartRotation, m_TargetRotation, m_ElapsedTime);       // change rotation
                //transform.localScale = Vector3.Lerp(currentScale, m_TargetScale, elapsedTime);

                yield return new WaitForFixedUpdate();
            }
            // Between Barrier moving

            if (m_MoveStartToTarget)        // this if statement will pass if the barrier is moving normally i.e the lever has not been turned off while the barrier is moving...
            {                               // ... in this case the time should be reset to 0 so that the barrier can lerp between the target and start positions appropriately
                yield return waitTime;      // will wait a set period of time before moving again. Occurs inside if statement so that there is not a wait time after the lever is turned on
                m_ElapsedTime = 0f;                   
            }
            m_MoveStartToTarget = false;

            // Barrier moving back to start position
            while (m_ElapsedTime < 1 && !m_MoveStartToTarget)                 // will move until barrier is back at start position
            {
                if (!m_IsActive)
                {
                    m_MoveStartToTarget = false;
                    m_IsCoroutineRunning = false;
                    yield break;
                }
                m_ElapsedTime += Time.deltaTime / m_MoveTime;
                transform.position = Vector3.Lerp(m_TargetPosition, m_StartPosition, m_ElapsedTime);
                transform.rotation = Quaternion.Lerp(m_TargetRotation, m_StartRotation, m_ElapsedTime);
                //transform.localScale = Vector3.Lerp(currentScale, m_TargetScale, elapsedTime);
                yield return new WaitForFixedUpdate();
            }
            // Between Barrier moving
            yield return waitTime;

            m_ElapsedTime = 0;
            m_MoveStartToTarget = true;
        }
        m_IsCoroutineRunning = false;
    }

    

    void MoveBarrierTransform(Transform initialTransform, Transform endTransform, float time)
    {        
            //if (initialTransform.position != endTransform.position)         // Position
           // {
                transform.position = Vector3.Lerp(initialTransform.position, endTransform.position, time);
            //}
            //if (initialTransform.rotation != endTransform.rotation)         // Rotation
            //{
            //    transform.rotation = Quaternion.Lerp(initialTransform.rotation, endTransform.rotation, time);
           // }
            //if (initialTransform.localScale != endTransform.localScale)     // Scale
           // {
           //     transform.localScale = Vector3.Lerp(initialTransform.localScale, endTransform.localScale, time);
           // }        
    }

    /*void SetCurrentTransform(ref Vector3 position, ref Quaternion rotation, ref Vector3 scale)
    {
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.localScale;
    }*/







/*
    // COPY OF WORKING ENUMERATOR INCASE I MESS IT UP
    // Barrier will move from its start location to target location. If shouldMoveOnce is true it will stay at target position
    IEnumerator MoveBarrier()
    {
        m_IsCoroutineRunning = true;
        float elapsedTime = 0;
        WaitForSeconds waitTime = new WaitForSeconds(m_WaitTime);
        // MOVE ONCE
        if (m_ShouldMoveOnce)
        {
            while (elapsedTime < 1)                 // will move until barrier is at target Position
            {
                elapsedTime += Time.deltaTime / m_MoveTime;
                transform.position = Vector3.Lerp(m_StartPosition, m_TargetPosition, elapsedTime);
                transform.rotation = Quaternion.Lerp(m_StartRotation, m_TargetRotation, elapsedTime);
                //transform.localScale = Vector3.Lerp(currentScale, m_TargetScale, elapsedTime);
                yield return null;
            }
        }
        // MOVE BACK AND FORTH        
        while (m_IsActive && !m_ShouldMoveOnce)     // will move while barrier is active
        {


            elapsedTime = 0;                        // reset time          
            while (elapsedTime < 1)                 // will move until barrier is at target Position
            {
                
                elapsedTime += Time.deltaTime / m_MoveTime;
                transform.position = Vector3.Lerp(m_StartPosition, m_TargetPosition, elapsedTime);          // change position
                transform.rotation = Quaternion.Lerp(m_StartRotation, m_TargetRotation, elapsedTime);       // change rotation
                //transform.localScale = Vector3.Lerp(currentScale, m_TargetScale, elapsedTime);

                yield return null;
            }
            yield return waitTime;                  // will wait a set period of time before moving again

            elapsedTime = 0f;                       // reset time
            while (elapsedTime < 1)                 // will move until barrier is back at start position
            {
                elapsedTime += Time.deltaTime / m_MoveTime;
                transform.position = Vector3.Lerp(m_TargetPosition, m_StartPosition, elapsedTime);
                transform.rotation = Quaternion.Lerp(m_TargetRotation, m_StartRotation, elapsedTime);
                //transform.localScale = Vector3.Lerp(currentScale, m_TargetScale, elapsedTime);
                yield return null;
            }
            yield return waitTime;
        }
        m_IsCoroutineRunning = false;
    }
    */
}
