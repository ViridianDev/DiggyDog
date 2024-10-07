using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateDust : MonoBehaviour
{
    [SerializeField] private GameObject dustParticles;
    [SerializeField] private Interactor interact;
    private GameObject clone;

    public void InstantiateDustParticles() //function that instantiates the dust particles that follow a crate when it is being pushed.
    {
        clone = Instantiate(dustParticles, interact.grabbedObject.transform);
        clone.transform.LookAt(GameObject.Find("Diggy (Player)").transform);
    }

    public void DestroyDustParticles()
    {
        Destroy(clone);
    }

    private void Awake()
    {
        interact = GameObject.Find("Interactor").GetComponent<Interactor>();
    }
}
