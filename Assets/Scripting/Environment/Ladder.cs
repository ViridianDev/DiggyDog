using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Interactor interact;

    private void Awake()
    {
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
        interact = GameObject.Find("Diggy (Player)").transform.Find("Interactor").GetComponent<Interactor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player.canClimbLadder = true;
            interact.gameObject.SetActive(false);

            player.movePoint1 = transform.Find("Move Point 1");
            player.movePoint2 = transform.Find("Move Point 2");
            player.movePoint3 = transform.Find("Move Point 3");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player.canClimbLadder = false;
            interact.gameObject.SetActive(true);

            player.movePoint1 = null;
            player.movePoint2 = null;
            player.movePoint3 = null;
        }
    }
}
