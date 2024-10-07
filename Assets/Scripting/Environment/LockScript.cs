using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockScript : MonoBehaviour
{
    private Animation doorAnim;

    private void Start()
    {
        doorAnim = GetComponent<Animation>();
        Unlock();
    }
    private void Unlock()
    {
        doorAnim.Play("DoorOpenCombine");
    }
}
