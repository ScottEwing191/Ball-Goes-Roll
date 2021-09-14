using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSingleton : MonoSingleton<PlayerSingleton>
{
    private PlayerController playerController;
    //[HideInInspector] public LeapFrogMechanic leapFrogMechanic;
    private PlayerCheckpointController playerCheckpointController;

    public PlayerCheckpointController PlayerCheckpointController {
        get { return playerCheckpointController; }
    }
    public PlayerController PlayerController {
        get { return playerController; }
    }


    protected override void Awake() {
        base.Awake();
        playerController = FindObjectOfType<PlayerController>();
        playerCheckpointController = FindObjectOfType<PlayerCheckpointController>();

    }
}
