using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSFX : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject fallSFX;
    private bool hasPlayedSound = false;
    private GameObject fallClone;

    private void Awake()
    {
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
    }
    public void PlayFallSFX()
    {
        if (!hasPlayedSound)
        {
            fallClone = Instantiate(fallSFX);
            hasPlayedSound = true;
        }
    }

    private void Update()
    {
        if (fallClone != null)
        {
            if (!player.isFalling)
            {
                Destroy(fallClone);
                hasPlayedSound = false;
            }
        }
    }
}
