using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    [SerializeField] private Checkpoint[] adjacentCheckpoints = new Checkpoint[1];          // the checkpoints that are next after this checkpoint. Most of the time there will only be one checkpoint. Should be possible to have multiple

    [SerializeField] int nextCPsNextCPIndex = 0;       // if the next checkpoint(CP) has multiple paths to take(multiple CPs adjacent) spicify the one you wish to use as the next CP (specify the next CP's next CP)   (Alternate Explination: when entering another CP trigger, this CP will now be the previous CP. Use this vasriable as the index of the other CP's nextCP araay to pick as set as the next CP)
    private Vector3 respawnPoint;
    private Quaternion respawnRotation;

    public Vector3 RespawnPoint {
        get { return respawnPoint; }
    }


    public Quaternion RespawnRotation {
        get { return respawnRotation; }
    }

    public Checkpoint[] NextCheckpoints {
        get { return adjacentCheckpoints; }
    }
    public int NextCheckpointIndex {
        get { return nextCPsNextCPIndex; }
    }




    // Start is called before the first frame update
    void Awake()
    {
        Transform[] transformsArray = gameObject.GetComponentsInChildren<Transform>();
        Transform childTransform = transformsArray[1];
        if(transformsArray.Length > 2) {
            Debug.LogError("Transform array in Checkpoints scripts is greater than 2 may have to adjust code to make sure correct Transform is selected");
        }
        respawnPoint = childTransform.position;
        respawnRotation = childTransform.rotation;
        childTransform.gameObject.SetActive(false);
    }

   
}
