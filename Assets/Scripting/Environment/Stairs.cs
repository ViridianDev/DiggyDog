using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    [SerializeField] private Raycaster raycaster;
    private float timer;

    private void Awake()
    {
        raycaster = GameObject.Find("Diggy (Player)").transform.Find("Raycaster").GetComponent<Raycaster>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            raycaster.negateFalling = true;
            raycaster.raycastDown = transform.TransformDirection(0f, -1f, 0f);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            raycaster.negateFalling = true;
            raycaster.raycastDown = transform.TransformDirection(0f, -1f, 0f);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            raycaster.negateFalling = false;
            raycaster.raycastDown = transform.TransformDirection(0f, -0.5f, 0f);
        }
    }

    private IEnumerator BeginCountdown()
    {
        float duration = 1f; // Set the duration of the countdown in seconds
        timer = 0f;

        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // When the countdown finishes, allow the player to fall
        Debug.Log("Can fall");
        raycaster.negateFalling = false;
    }
}
