using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingSFX : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject climbSFX;
    private bool hasPlayedSound = false;
    private GameObject climbClone;

    private void Awake()
    {
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
    }
    public void PlayClimbSFX()
    {
        if (!hasPlayedSound)
        {
            climbClone = Instantiate(climbSFX);
            hasPlayedSound = true;
        }
    }

    private void Update()
    {
        if (climbClone != null)
        {
            if (!player.isClimbingLadder)
            {
                Destroy(climbClone);
                hasPlayedSound = false;
            }
        }
    }
}
