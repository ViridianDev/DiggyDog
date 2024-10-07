using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    //Script references
    [SerializeField] private PlayerController player;
    [SerializeField] private Interactor interact;
    [SerializeField] private Hole hole;
    public bool negateFalling = false;

    private Transform forwardRayOrigin; //the point at which the forward raycast originates
    private Transform backwardRayOrigin; //the point at which the backward raycast originates
    private Vector3 raycastForward; //the raycast that shoots out in front of the player
    private Vector3 raycastBehind; //the raycast that shoots out from behind the player
    [System.NonSerialized] public Vector3 raycastDown; //the raycast that shoots from the player into the ground
    private RaycastHit objectHit;

    //booleans that determine if the push directions are valid
    public bool canPushForward = true;
    public bool canPushBackward = true;

    public bool isFilling = false;

    public void CheckMoveDirections() //function that determines which direction the player can push or pull the block in
    {
        if (Physics.Raycast(forwardRayOrigin.position, raycastForward, out objectHit, 1.5f, ~2)) //if the object (not the player) exists and is on the "stopMovement" layer
        {
            if (objectHit.transform != null && objectHit.transform.tag != "Player" && (objectHit.transform.gameObject.layer == 3 || objectHit.transform.gameObject.layer == 8))
            {
                canPushForward = false;
            }
        }
        else
        {
            canPushForward = true;
        }

        if (Physics.Raycast(backwardRayOrigin.position, raycastBehind, out objectHit, 1.5f, ~2)) //if the raycasts detects something NOT on the IgnoreRaycast layermask
        {
            if (objectHit.transform != null && objectHit.transform.tag != "Player" && (objectHit.transform.gameObject.layer == 3 || objectHit.transform.gameObject.layer == 8 || objectHit.transform.name.Contains("Hole"))) //if the object (not the player) exists and is on the "stopMovement" layer, and is
            {
                canPushBackward = false;
            }
        }
        else
        {
            canPushBackward = true;
        }
    }

    private void Start()
    {
        raycastDown = transform.TransformDirection(new Vector3(0f, -0.5f, 0f));
        Debug.Log(raycastDown.y);
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, raycastDown, Color.white);

        if (!Physics.Raycast(transform.position, raycastDown, out objectHit, -raycastDown.y) && !player.isClimbingLadder) //if there is not something below the player to stand on and they are not climbing a ladder
        {
            if (!negateFalling)
            {
                StartCoroutine(BeginFall());
            }
        }
        else if (Physics.Raycast(transform.position, raycastDown, out objectHit, -raycastDown.y))
        {
            if(objectHit.transform.tag == "Trigger")
            {
                player.isFalling = true;
            }
            else
            {
                player.isFalling = false;
            }
        }

        if (hole != null)
        {
            //do stuff
        }

        if (player.isGrabbingRock && !player.isMovingRock)
        {
            if (isFilling)
            {
                return;
            }
            else
            {
                interact.grabbedObject.transform.Find("Raycast Point").transform.rotation = player.transform.rotation;

                forwardRayOrigin = interact.grabbedObject.transform.Find("Raycast Point").transform; //sets the forward ray origin as grabbed object's raycast point child object
                backwardRayOrigin = transform; //sets the backward ray origin as the raycaster transform

                //creates the raycasts
                raycastForward = forwardRayOrigin.TransformDirection(new Vector3(0f, 0f, 1f));
                raycastBehind = backwardRayOrigin.TransformDirection(new Vector3(0f, 0f, -1f));

                //displays the raycasts in the scene for testing
                Debug.DrawRay(forwardRayOrigin.position, raycastForward, Color.white);
                Debug.DrawRay(backwardRayOrigin.position, raycastBehind, Color.white);

                CheckMoveDirections();
            }  
        }
        else
        {
            //reset the booleans to their default values
            canPushForward = true;
            canPushBackward = true;
        }

        if(forwardRayOrigin != null)
        {
            if (Physics.Raycast(forwardRayOrigin.position, raycastForward, out objectHit, 1f))
            {
                if (objectHit.transform != null)
                {
                    if (objectHit.transform.name == "Hole")
                    {
                        hole = objectHit.transform.GetComponentInChildren<Hole>();
                    }
                }
            }
            else
            {
                hole = null;
            }
        }
    }

    private IEnumerator BeginFall()
    {
        yield return new WaitForSeconds(0.1f);
        player.isFalling = true;
    }
}
