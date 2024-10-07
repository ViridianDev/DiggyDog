using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public GameObject grabbedObject;

    private void OnTriggerStay(Collider other)
    {
        grabbedObject = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        grabbedObject = null;
    }
}
