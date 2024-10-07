using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gringus : MonoBehaviour
{
    //Script references
    [SerializeField] private PlayerController player;
    [SerializeField] private EndLevel ender;
    private KeySFX keySFX;

    private void Awake()
    {
        keySFX = GameObject.Find("Sound Manager").transform.Find("Key Sound Spawner").GetComponent<KeySFX>();
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
        ender = GameObject.Find("Canvas").transform.Find("Iris Transition").GetComponent<EndLevel>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //player collects Gringus Idol and wins entire game
            keySFX.PlayKeySFX();
            gameObject.SetActive(false);
            player.allowInput = false;
            ender.GetComponent<Animation>().clip = ender.GetComponent<Animation>().GetClip("IrisIn");
            ender.GetComponent<Animation>().Play();
        }
    }
}
