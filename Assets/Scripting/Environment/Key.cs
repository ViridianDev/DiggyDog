using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Key : MonoBehaviour
{
    [SerializeField] UnityEvent key;
    private KeySFX keySFX;
    [SerializeField] private bool destroySelf = true;

    private void Awake()
    {
        keySFX = GameObject.Find("Sound Manager").transform.Find("Key Sound Spawner").GetComponent<KeySFX>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            key.Invoke();
            keySFX.PlayKeySFX();

            if (destroySelf)
            {
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
