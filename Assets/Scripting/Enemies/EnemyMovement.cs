using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    //Script references
    [SerializeField] private PlayerController player;

    //Variables that control how and where the enemy moves.
    [SerializeField] private List<Vector3> movePoints; //List that contains all of the locations the enemy will move to
    private Vector3 startPoint; //The start position of the enemy
    private Vector3 endPoint; //The end position that the enemy will move to
    private Vector3 previousPoint; //The position from which the enemy came from
    [SerializeField] private float moveSpeed = 3f; //The speed at which the enemy moves

    //Variables that control the enemy's raycast
    [SerializeField] private Transform rayOrigin; //the point at which the raycast originates
    private Vector3 raycastForward; //the raycast that shoots out in front of the player
    private RaycastHit objectHit;

    [SerializeField] private Animator anim;

    private void Awake()
    {
        player = GameObject.Find("Diggy (Player)").GetComponent<PlayerController>();
        raycastForward = rayOrigin.TransformDirection(transform.forward);
        previousPoint = startPoint; //sets a default previous point
    }
    private IEnumerator Start() //Start function serves as the patrol sequence of the enemy. The enemy will move to the endpoint, turn around, move to the start point, turn around, and repeat
    {
        while (this != null) //while the enemy still exists in the scene
        {
            for (int i = 0; i < movePoints.Count; i++)
            {
                yield return StartCoroutine(RotateToTarget(movePoints[i])); //rotate the enemy to its next move point
                yield return StartCoroutine(MoveToTarget(movePoints[i])); //move the enemy to that point
                yield return new WaitForSeconds(1f);

                if (i == movePoints.Count - 1) //if this is the last move point
                {
                    //reverse the list of move points
                    movePoints.Reverse();
                    //switch the start point with the end point
                    startPoint = movePoints[0];
                    endPoint = movePoints[movePoints.Count - 1];
                }
            }
            //reset the previous point for the next iteration
            previousPoint = movePoints[movePoints.Count - 1];
        }
    }

    private IEnumerator MoveToTarget(Vector3 target) //function that moves the enemy to the point it is given
    {
        Vector3 targetPosition = target;
        transform.LookAt(target); //have the enemy look at the point they are moving to

        while (transform.position != targetPosition) //while the enemy hasn't reached the target point, keep moving it
        {
            anim.SetBool("isWalking", true);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime); //move the enemy to the point
            yield return null;
        }
        previousPoint = targetPosition;
        anim.SetBool("isWalking", false);
    }

    private IEnumerator RotateToTarget(Vector3 target)
    {
        //get the initial rotation and target rotation of the enemy
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(target - transform.position);

        //interpolate the enemy's rotation over time
        float timeElapsed = 0.0f;
        while (timeElapsed < 1f)
        {
            float t = timeElapsed;

            //rotate the enemy over 1 second and increment the time
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        //ensure the object exactly faces the target at the end of the rotation
        transform.rotation = targetRotation;
    }

    private IEnumerator HandleBlockedPath()
    {
        anim.SetBool("isWalking", false);
        yield return StartCoroutine(RotateToTarget(previousPoint));
        yield return StartCoroutine(MoveToTarget(previousPoint));
        StartCoroutine(Start());

    }
    private void OnCollisionEnter(Collision collision) //if the enemy comes in contact with the player, the player takes 1 health of damage
    {
        if (collision.transform.tag == "Player")
        {
            StartCoroutine(player.TakeDamage(transform));
        }
    }
    public IEnumerator BecomeIntangible()
    {
        transform.GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(5f);
        transform.GetComponent<BoxCollider>().enabled = true;
    }
    private void FixedUpdate()
    {
        raycastForward = transform.forward;
        Debug.DrawRay(rayOrigin.position, raycastForward, Color.white);

        if (Physics.Raycast(transform.position, raycastForward, out objectHit, 0.5f, ~2))
        {
            if (objectHit.transform != null && objectHit.transform.tag == "Push Block")
            {
                StopAllCoroutines();
                StartCoroutine(HandleBlockedPath());
            }
        }
    }
}
