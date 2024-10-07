using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : MonoBehaviour
{
    public void HideDoor()
    {
        transform.Find("Door").Find("DoorObject").gameObject.SetActive(false);
    }
}
