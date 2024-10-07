using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySFX : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject keySFX;

    private void Awake()
    {
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
    }
    public void PlayKeySFX()
    {
        GameObject keyClone = Instantiate(keySFX);
        Destroy(keyClone, 1f);
    }
}
