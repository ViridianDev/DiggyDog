using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinHole : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            collision.transform.GetComponent<PlayerController>().hasWon = true;
        }
    }
}
