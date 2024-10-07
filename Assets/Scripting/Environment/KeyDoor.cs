using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyDoor : MonoBehaviour
{
    //Script references
    [SerializeField] private PlayerController player;
    public bool isOpened = false;

    private void Awake()
    {
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && player.keys > 0) //open the key door if the player enters the trigger in front of the door and has at least 1 key
        {
            if(isOpened == false)
            {
                isOpened = true;
                transform.parent.GetComponent<Animation>().Play(); //open the door
                player.ChangeKeys(-1); //the player loses 1 key
            }
        }
    }
}
