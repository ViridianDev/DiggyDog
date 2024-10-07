using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFalling : MonoBehaviour
{
    //Script references
    [SerializeField] private PlayerController player;
    [SerializeField] private Interactor interact;

    //Raycast variables
    private Vector3 raycastDown;
    private RaycastHit objectHit;

    public bool okayToFall = false; //variable that controls when the block can fall

    private void Awake()
    {
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
        interact = GameObject.Find("Interactor").GetComponent<Interactor>();
    }

    private void Start()
    {
        raycastDown = transform.TransformDirection(new Vector3(0f, -0.5f, 0f)); //sets a default value for the down raycast
    }

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, raycastDown, Color.white); //displays the down raycast for testing purposes

        if (!Physics.Raycast(transform.position, raycastDown, out objectHit, 0.55f) && okayToFall) //if there is nothing below the block and it is okay for the block to fall
        {
            Debug.Log("falling rock");
            transform.GetComponent<BoxCollider>().size = new Vector3(0.95f, 0.95f, 0.95f);
            player.allowInput = false;
            GetComponent<Rigidbody>().constraints &= ~RigidbodyConstraints.FreezePositionY;

            //create rounded positions for the grabbed object
            float roundedXPos = Mathf.Round(transform.position.x * 10) / 10;
            float roundedYPos = Mathf.Round(transform.position.y * 10) / 10;
            float roundedZPos = Mathf.Round(transform.position.z * 10) / 10;

            //Set the grabbed object's position and rotation to the rounded numbers so that they remain on the grid
            Vector3 roundedPos = new Vector3(roundedXPos, roundedYPos, roundedZPos);

            transform.position = roundedPos;
            transform.localScale = new Vector3(1f, 1f, 1f);

            player.isGrabbingRock = false;
            gameObject.layer = 3;
            transform.parent = null;
            transform.GetComponent<BoxCollider>().size = new Vector3(1f, 1f, 1f);
            //StartCoroutine(BlockFall()); //make the block fall
        }
        else if(Physics.Raycast(transform.position, raycastDown, out objectHit, 0.55f))
        {
            okayToFall = false;
        }
    }
    private IEnumerator BlockFall() //function that handles how blocks fall
    {
        yield return new WaitForEndOfFrame();
        player.isGrabbingRock = false;
        player.anim.SetBool("isPushing", false);
        Vector3 fallPos = new Vector3(transform.position.x, 0.5f, transform.position.z);

        float elapsedTime = 0f; //creates a timer variable
        while (elapsedTime < 0.5f)
        {
            float t = elapsedTime / 3; //creates a time variables that controls the speed of the motion
            transform.position = Vector3.Lerp(transform.position, fallPos, t); //creates an end position for the player to lerp to
            elapsedTime += Time.deltaTime; //increments the timer
            yield return new WaitForEndOfFrame();
        }

        transform.position = fallPos; //ensures the player reaches the exact target position
        interact.grabbedObject.layer = 3;
        interact.grabbedObject.transform.parent = null;
        player.allowInput = true;
        okayToFall = false;  
    }
}
