using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyTunnelCamera : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject gringusCamera;
    [SerializeField] private GameObject keyTunnelCamera;
    [SerializeField] private GameObject dropCamera;

    [SerializeField] PlayerController player;

    private void Awake()
    {
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            player.isInKeyTunnelTrigger = true;

            keyTunnelCamera.SetActive(true);
            gringusCamera.SetActive(false);
            dropCamera.SetActive(false);
            mainCamera.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            player.isInKeyTunnelTrigger = false;

            mainCamera.SetActive(true);
            gringusCamera.SetActive(false);
            dropCamera.SetActive(false);
            keyTunnelCamera.SetActive(false);
        }
    }
}
