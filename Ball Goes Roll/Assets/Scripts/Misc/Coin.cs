using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    bool isPickedUp = false;
    Animator anim;
    AudioSource audioSource;
    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !isPickedUp) {
            anim.SetBool("CoinPickedUp", true);
            GameManager.Instance.CollectCoin();
            isPickedUp = true;
            audioSource.pitch = Random.Range(0.6f, 1);
            audioSource.Play();
            
        }
    }
}
