using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource winStart;
    [SerializeField] private AudioSource winTheme;

    private void Start()
    {
        winStart.loop = false;
        winStart.Stop();
        StartCoroutine(CheckAudio());
    }

    private IEnumerator CheckAudio()
    {
        while (winStart.isPlaying)
        {
            yield return null;
        }

        PlayWinTheme();
    }

    public void PlayWinTheme()
    {
        Debug.Log("Playing");
        winTheme.Play();
    }
}
