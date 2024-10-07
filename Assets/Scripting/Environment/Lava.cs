using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : MonoBehaviour
{
    private PlayerController player;

    private void Awake()
    {
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            foreach (GameObject hearts in player.heartIcons)
            {
                hearts.SetActive(false);
            }
            player.anim.SetBool("isFalling", false);
            player.anim.SetBool("isDead", true);
            player.transform.Find("Hurt SFX").GetComponent<AudioSource>().Play();
            player.health = 0;
        }
    }
}
