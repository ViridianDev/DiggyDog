using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GringusCam : MonoBehaviour
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            player.isInGringusTrigger = true;

            gringusCamera.SetActive(true);
            dropCamera.SetActive(false);
            keyTunnelCamera.SetActive(false);
            mainCamera.SetActive(false);
        }
    }
}
