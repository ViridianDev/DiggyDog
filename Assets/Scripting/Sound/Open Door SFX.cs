using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorSFX : MonoBehaviour
{
    public void PlaySFX()
    {
        GetComponent<AudioSource>().Play();
    }

    public void StopSFX()
    {
        GetComponent<AudioSource>().Stop();
    }
}
