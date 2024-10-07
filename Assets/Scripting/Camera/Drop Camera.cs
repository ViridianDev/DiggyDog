using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropCamera : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject gringusCamera;
    [SerializeField] private GameObject keyTunnelCamera;
    [SerializeField] private GameObject dropCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {            
            dropCamera.SetActive(true);
            gringusCamera.SetActive(false);
            keyTunnelCamera.SetActive(false);
            mainCamera.SetActive(false);
        }
    }
}
