using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoSingleton<PlayerSingleton>
{
    private PlayerController playerController;
    //[HideInInspector] public LeapFrogMechanic leapFrogMechanic;
    private PlayerCheckpointController playerRespawn;

    public PlayerCheckpointController PlayerRespawn {
        get { return playerRespawn; }
    }
    public PlayerController PlayerController {
        get { return playerController; }
    }


    protected override void Awake() {
        base.Awake();
        playerController = FindObjectOfType<PlayerController>();
        playerRespawn = FindObjectOfType<PlayerCheckpointController>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
