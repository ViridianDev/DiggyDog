using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region Variables and References
    //Script references
    [Header("Script References")]
    [SerializeField] private Interactor interact;
    [SerializeField] private Raycaster raycaster;
    [SerializeField] private CrateDust duster;
    [SerializeField] private BlockFalling BF;
    [SerializeField] private FallingSFX fall;
    [SerializeField] private ClimbingSFX climb;
    [SerializeField] private EndLevel ender;
    [SerializeField] private PhysicMaterial fallingMaterial;

    //Camera-related variables
    [Header("Camera")]
    [SerializeField] private GameObject virtualCamera; //the virtual camera that the player can turn using the mouse buttons
    private CinemachineVirtualCamera camera;
    private Quaternion camRot; //quaternion value that represents the rotation of the camera
    [SerializeField] private float camMoveDuration = 1f; //value in seconds that controls how fast the camera turns
    [SerializeField] private char camOrientation; //variable representing the orientation of the camera. 'F' = Front, 'R' = Right, 'L' = Left, 'B' = Back
    private bool isRotating = false; //variable that controls camera rotation
    [SerializeField] private float zoomSpeed = 0.1f; //variable that controls how fast the player can zoom the camera in and out
    [SerializeField] private float zoomSpeedController = 1f; //variable that controls how fast the player can zoom the camera in and out with the controller stick
    private float currentZoomInput = 0f;
    [SerializeField] private float minZoom; //variable that represents how far the camera can zoom out
    [SerializeField] private float maxZoom; //variable that represents how far the camera can zoom in

    //Player movement-related variables
    [Header("Player Movement")]
    public Rigidbody rb; //rigidbody of the player
    [SerializeField] private Vector2 movementInput; //vector2 value based on the input from the input system (WASD or arrow keys)
    public float angle; // //the angle between the player's forward direction and the four cardinal directions
    public Vector3 movementDirection; //vector3 value that converts 2D input into 3D inputs
    [SerializeField] private float moveSpeed = 2f; //float value that represents the player's movement
    [SerializeField] private float rotationSpeed = 1000f; //float value that represents the player's angular movement
    public char playerDirection; //variable representing the orientation of the player. 'N' = North, 'S' = South, 'E' = East, 'W' = West

    //Transforms that the player moves to during ladder climbing
    [System.NonSerialized] public Transform movePoint1;
    [System.NonSerialized] public Transform movePoint2;
    [System.NonSerialized] public Transform movePoint3;

    //Booleans that control when certain movements are allowed
    [Header("Player Conditionals")]
    public bool allowInput = true; //variable that can restrict movement
    public bool negateNextInput = false; //variable that will negate the next input, used for correct pause screen functionality
    public bool isMoving; //variable that checks to see if the player is moving
    public bool isFalling = false; //variable that displays when the player is falling
    public bool canClimbLadder = false; //variable that checks to see if the player is able to climb a ladder
    public bool isClimbingLadder = false; //variable that checks to see if the player is climbing a ladder
    public bool isGrabbingRock = false; //variable that checks to see if the player is grabbing something
    public bool isMovingRock = false; //variable that checks to see if the player is moving a rock
    public bool isRotatingRock = false; //variable that checks to see if the player is rotating a rock
    public bool isAdjustingLocation = false; //variable that checks to see if the player's position is being adjusted before grabbing a rock
    public bool canTakeDamage = false; //variable that checks to see if the player can take damage or not
    public bool isDead = false; //variable that specifies when the player is dead
    public bool hasWon = false; //variable that handles level winning
    public bool hasPlayedWinningSequence = false; //variable that handles level winning
    private bool hasToggled = false;
    public bool isInKeyTunnelTrigger = false;
    public bool isInGringusTrigger = false;

    [Header("Player Stats")]
    public GameObject[] heartIcons;
    public int health = 3;
    public int keys = 0;

    //Time variables that control how long certain movements are
    [Header("Movement Durations")]
    [SerializeField] private float rockDuration = 1f; //variable that determines how long a push/pull/rotation is
    [SerializeField] private float rockRotationDuration = 1f; //variable that determines how long a push/pull/rotation is
    [SerializeField] private float climbLadderDuration = 1f; //variable that determines how long a push/pull/rotation is

    [Header("Player Animator")]
    public Animator anim;

    //UI-related variables
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI camText;
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private TextMeshProUGUI keysNum;
    #endregion

    #region Player Action Functions
    //functions that handle how the player moves and interacts with the environment

    private void UpdateMovementAndRotation() //function that handles player movement
    {
        Vector3 forward = transform.forward; //gets the forward vector of the player
        angle = Vector3.SignedAngle(forward, Vector3.forward, Vector3.up);

        switch (camOrientation) //determines player direction based on angle and camera orientation
        {
            case 'F': //camera is in "front" position
                if (angle >= -45f && angle < 45f)
                {
                    playerDirection = 'N';
                    playerText.text = "Player Facing: North";
                }
                else if (angle >= 45f && angle < 135f)
                {
                    playerDirection = 'W';
                    playerText.text = "Player Facing: West";
                }
                else if (angle >= -135f && angle < -45f)
                {
                    playerDirection = 'E';
                    playerText.text = "Player Facing: East";
                }
                else
                {
                    playerDirection = 'S';
                    playerText.text = "Player Facing: South";
                }
                break;
            case 'R': //camera is in "right" position
                if (angle >= -45f && angle < 45f)
                {
                    playerDirection = 'E';
                    playerText.text = "Player Facing: East";
                }
                else if (angle >= 45f && angle < 135f)
                {
                    playerDirection = 'N';
                    playerText.text = "Player Facing: North";
                }
                else if (angle >= -135f && angle < -45f)
                {
                    playerDirection = 'S';
                    playerText.text = "Player Facing: South";
                }
                else
                {
                    playerDirection = 'W';
                    playerText.text = "Player Facing: West";
                }
                break;
            case 'B': //camera is in "back" position
                if (angle >= -45f && angle < 45f)
                {
                    playerDirection = 'S';
                    playerText.text = "Player Facing: South";
                }
                else if (angle >= 45f && angle < 135f)
                {
                    playerDirection = 'E';
                    playerText.text = "Player Facing: East";
                }
                else if (angle >= -135f && angle < -45f)
                {
                    playerDirection = 'W';
                    playerText.text = "Player Facing: West";
                }
                else
                {
                    playerDirection = 'N';
                    playerText.text = "Player Facing: North";
                }
                break;
            case 'L': //camera is in "left" position
                if (angle >= -45f && angle < 45f)
                {
                    playerDirection = 'W';
                    playerText.text = "Player Facing: West";
                }
                else if (angle >= 45f && angle < 135f)
                {
                    playerDirection = 'S';
                    playerText.text = "Player Facing: South";
                }
                else if (angle >= -135f && angle < -45f)
                {
                    playerDirection = 'N';
                    playerText.text = "Player Facing: North";
                }
                else
                {
                    playerDirection = 'E';
                    playerText.text = "Player Facing: East";
                }
                break;    
        }

        if (!isGrabbingRock) //if the player is not grabbing anything
        {
            Vector3 move = movementDirection * moveSpeed * Time.fixedDeltaTime; //creates a move vector that considers move speed and time
            rb.MovePosition(rb.position + move); //updates the player's rigidbody
            anim.SetBool("isRunning", true);
            transform.Find("Walking SFX").GetComponent<AudioSource>().Play();

            float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg; //calculates the angle in degrees between the player's forward direction and movement direction
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f); //creates a rotation value based on the targetAngle on the y-axis
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime); //smootly rotate the player towards the target direction
        }
        else if (!isMovingRock && !isRotatingRock && (Mathf.Abs(movementDirection.x) != 0 ^ Mathf.Abs(movementDirection.z) != 0)) //if the player is currently not moving a rock and they are not moving diagonally
        {
            Vector3 targetPosition = transform.position + movementDirection; //calculates target move position based on what direction the player inputs

            if(!raycaster.canPushForward && !raycaster.canPushBackward)
            {
                return;
            }
            else
            {
                switch (camOrientation)
                {
                    case 'F': //player direction is relative. If the player is facing east and the camera is oriented forward, then the player's forward is positive direction on the x-axis. This is extremely confusing, I know.
                        {
                            switch (playerDirection)
                            {
                                case 'N':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                anim.SetBool("isPushing", true);
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                anim.SetBool("isPulling", true);
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                                case 'S':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                                case 'E':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                                case 'W':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case 'B':
                        {
                            switch (playerDirection)
                            {
                                case 'N':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                                case 'S':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                                case 'E':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                                case 'W':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case 'R':
                        {
                            switch (playerDirection)
                            {
                                case 'N':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                                case 'S':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                                case 'E':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                                case 'W':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case 'L':
                        {
                            switch (playerDirection)
                            {
                                case 'N':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                                case 'S':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.x) > Mathf.Abs(movementDirection.z) && movementDirection.x < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                                case 'E':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                                case 'W':
                                    {
                                        if (raycaster.canPushForward) //if the player can push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                        else //if the player cannot push the block forward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }

                                        if (raycaster.canPushBackward) //if the player can pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z < 0) //if the player moves backward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 2));
                                            }
                                        }
                                        else //if the player cannot pull the block backward
                                        {
                                            if (Mathf.Abs(movementDirection.z) > Mathf.Abs(movementDirection.x) && movementDirection.z > 0) //if the player moves forward
                                            {
                                                isMovingRock = true;
                                                StartCoroutine(MoveRock(targetPosition, 1));
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
        }
    }

    public void Move(InputAction.CallbackContext context) //movement function for the input system
    {
        movementInput = context.ReadValue<Vector2>();

        //round out the movement to accommodate controller inputs
        float roundedXInput = Mathf.Round(movementInput.x);
        float roundedYInput = Mathf.Round(movementInput.y);

        if(!isInKeyTunnelTrigger && !isInGringusTrigger)
        {
            switch (camOrientation) //change movementDirection value to be congruent across all camera orientations
            {
                case 'F': //camera is in "front" position
                    movementDirection = new Vector3(roundedXInput, 0, roundedYInput);
                    break;

                case 'R': //camera is in "right" position
                    movementDirection = new Vector3(-roundedYInput, 0, roundedXInput);
                    break;

                case 'B': //camera is in "back" position
                    movementDirection = new Vector3(-roundedXInput, 0, -roundedYInput);
                    break;

                case 'L': //camera is in "left" position
                    movementDirection = new Vector3(roundedYInput, 0, -roundedXInput);
                    break;
            }
        }
        else if (isInKeyTunnelTrigger)
        {
            movementDirection = new Vector3(-roundedYInput, 0, roundedXInput);
        }
        else if (isInGringusTrigger)
        {
            movementDirection = new Vector3(roundedYInput, 0, -roundedXInput);
        }
    }

    public void Primary(InputAction.CallbackContext context) //primary interaction input. E key
    {
        if (negateNextInput)
        {
            negateNextInput = false;
            return;
        }
        else if (!isMoving && context.started) //if the player is not moving
        {
            if (!isGrabbingRock && interact.grabbedObject != null) //if the player is not already grabbing a rock and the player is not already grabbing something
            {
                if (interact.grabbedObject.tag == "Push Block") //if the interacted object is a rock
                {
                    StartCoroutine(AdjustPlayerAndGrab(interact.grabbedObject));
                }
                else //if the grabbed object is not a rock
                {
                    //other grabbing logic
                }
            }
            else if (!isMovingRock && !isRotatingRock) //if the player is currently grabbing a rock and they press the primary input again
            {
                isRotatingRock = true;
                anim.SetTrigger("RotateBlock");
                StartCoroutine(RotateRock()); //rotate the rock 90 degrees
            }
        }
    }

    public void Secondary(InputAction.CallbackContext context) //secondary interaction input. R key
    {
        if (negateNextInput)
        {
            negateNextInput = false;
            return;
        }
        else if (context.started && !isMoving && !isMovingRock && !isRotatingRock && !isClimbingLadder) //if the player themselves is not moving and they are also not moving a rock
        {
            if (isGrabbingRock) //if they are currently grabbing a rock
            {
                interact.grabbedObject.transform.parent = null; //let go of the rock
                interact.grabbedObject.transform.localScale = new Vector3(1f, 1f, 1f);
                interact.grabbedObject.layer = 3; //put the rock back on the default layer

                //create rounded positions for the grabbed object
                float roundedXPos = Mathf.Round(interact.grabbedObject.transform.position.x * 10) / 10;
                float roundedYPos = Mathf.Round(interact.grabbedObject.transform.position.y * 10) / 10;
                float roundedZPos = Mathf.Round(interact.grabbedObject.transform.position.z * 10) / 10;

                //create rounded rotations for the grabbed object
                float roundedXRot = Mathf.Round(interact.grabbedObject.transform.localEulerAngles.x);
                float roundedYRot = Mathf.Round(interact.grabbedObject.transform.localEulerAngles.y);
                float roundedZRot = Mathf.Round(interact.grabbedObject.transform.localEulerAngles.z);

                //Set the grabbed object's position and rotation to the rounded numbers so that they remain on the grid
                Vector3 roundedPos = new Vector3(roundedXPos, roundedYPos, roundedZPos);
                Quaternion roundedRot = Quaternion.Euler(roundedXRot, roundedYRot, roundedZRot);

                interact.grabbedObject.transform.position = roundedPos;
                interact.grabbedObject.transform.rotation = roundedRot;

                isGrabbingRock = false;
                StopAllCoroutines();
            }
            else if (interact.grabbedObject != null)
            {
                if (canClimbLadder && !isClimbingLadder) //if the object they are interacting with has a ladder component
                {
                    isClimbingLadder = true;
                    anim.SetTrigger("Climb");
                    climb.PlayClimbSFX();
                    StartCoroutine(ClimbLadder(movePoint1, movePoint2, movePoint3));
                }
            }
        }
    }

    private IEnumerator AdjustPlayerAndGrab(GameObject heldObject) //function that moves the player to be parallel to the rock's position when grabbing. This is so that the player won't collide with walls when pushing blocks through a tight passage
    {
        Vector3 newPos; //the position the player will move to before moving the rock
        isAdjustingLocation = true;
        interact.grabbedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll; //freeze the rock's rigidbody

        switch (camOrientation) //player's position will be moved based on camera orientation and the direction they're facing
        {
            case 'F': //camera is in "front" position
            case 'B': //camera is in "back" position
                if (playerDirection == 'N' || playerDirection == 'S')
                {
                    //match the player's x coordinate with the rock's x coordinate and move them to that position
                    newPos = new Vector3(interact.grabbedObject.transform.position.x, transform.position.y, transform.position.z);

                    while (transform.position != newPos)
                    {
                        anim.SetBool("isRunning", true);
                        transform.position = Vector3.MoveTowards(transform.position, newPos, (moveSpeed / 5) * Time.fixedDeltaTime);
                        yield return null;
                    }
                }
                else if (playerDirection == 'E' || playerDirection == 'W')
                {
                    //match the player's z coordinate with the rock's z coordinate and move them to that position
                    newPos = new Vector3(transform.position.x, transform.position.y, interact.grabbedObject.transform.position.z);

                    while (transform.position != newPos)
                    {
                        anim.SetBool("isRunning", true);
                        transform.position = Vector3.MoveTowards(transform.position, newPos, (moveSpeed / 5) * Time.fixedDeltaTime);
                        yield return null;
                    }
                }
                break;

            case 'R': //camera is in "right" position
            case 'L': //camera is in "left" position
                if (playerDirection == 'N' || playerDirection == 'S')
                {
                    //match the player's z coordinate with the rock's z coordinate and move them to that position
                    newPos = new Vector3(transform.position.x, transform.position.y, interact.grabbedObject.transform.position.z);

                    while (transform.position != newPos)
                    {
                        anim.SetBool("isRunning", true);
                        transform.position = Vector3.MoveTowards(transform.position, newPos, (moveSpeed / 5) * Time.fixedDeltaTime);
                        yield return null;
                    }
                }
                else if (playerDirection == 'E' || playerDirection == 'W')
                {
                    //match the player's x coordinate with the rock's x coordinate and move them to that position
                    newPos = new Vector3(interact.grabbedObject.transform.position.x, transform.position.y, transform.position.z);

                    while (transform.position != newPos)
                    {
                        anim.SetBool("isRunning", true);
                        transform.position = Vector3.MoveTowards(transform.position, newPos, (moveSpeed / 5) * Time.fixedDeltaTime);
                        yield return null;
                    }
                }
                break;
        }
        anim.SetBool("isRunning", false);
        isAdjustingLocation = false;
        interact.grabbedObject.transform.parent = this.transform; //grab the rock
        interact.grabbedObject.layer = 2; //put the grabbed rock on the IgnoreRaycast layer
        interact.grabbedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        BF = interact.grabbedObject.GetComponent<BlockFalling>();
        isGrabbingRock = true;
        interact.grabbedObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private IEnumerator MoveRock(Vector3 targetPos, int direction) //function that controls how the player pushes/pulls rocks
    {
        BF.okayToFall = false;
        float elapsedTime = 0f; //creates a timer variable

        if (direction == 1)
        {
            anim.SetBool("isPushing", true);
        }
        else if (direction == 2)
        {
            anim.SetBool("isPulling", true);
        }

        while (elapsedTime < 1f)
        {
            float t = elapsedTime / rockDuration; //creates a time variables that controls the speed of the motion
            transform.position = Vector3.Lerp(transform.position, targetPos, t); //creates an end position for the player to lerp to
            elapsedTime += Time.deltaTime; //increments the timer
            yield return new WaitForEndOfFrame();
        }

        transform.position = targetPos; // ensures the object reaches the exact target position

        isMovingRock = false;

        if (direction == 1)
        {
            anim.SetBool("isPushing", false);
        }
        else if (direction == 2)
        {
            anim.SetBool("isPulling", false);
        }

        BF.okayToFall = true;
    }

    private IEnumerator RotateRock() //function that controls how the player rotates rocks
    {
        float elapsedTime = 0f; //creates a timer variable
        Quaternion targetRot = Quaternion.Euler(0f, interact.grabbedObject.transform.eulerAngles.y + 90f, 0f);
        while (elapsedTime < 0.5f)
        {
            float t = elapsedTime / rockRotationDuration;
            interact.grabbedObject.transform.rotation = Quaternion.Slerp(interact.grabbedObject.transform.rotation, targetRot, t);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        interact.grabbedObject.transform.rotation = targetRot; //ensures the object reaches the exact target rotation
        interact.grabbedObject.transform.localScale = new Vector3(1f, 1f, 1f);

        isRotatingRock = false;
    }

    private IEnumerator RoundBlockPos() //function that rounds grabbed block pos once in FixedUpdate when the player's grabbing is interrupted
    {
        if (isFalling)
        {
            interact.grabbedObject.transform.parent = null; //let go of the rock
            interact.grabbedObject.layer = 3; //put the rock back on the default layer
            isGrabbingRock = false;
        }

        //create rounded positions for the grabbed object
        float roundedXPos = Mathf.Round(interact.grabbedObject.transform.position.x * 10) / 10;
        float roundedYPos = Mathf.Round(interact.grabbedObject.transform.position.y * 10) / 10;
        float roundedZPos = Mathf.Round(interact.grabbedObject.transform.position.z * 10) / 10;

        //create rounded rotations for the grabbed object
        float roundedXRot = Mathf.Round(interact.grabbedObject.transform.localEulerAngles.x);
        float roundedYRot = Mathf.Round(interact.grabbedObject.transform.localEulerAngles.y);
        float roundedZRot = Mathf.Round(interact.grabbedObject.transform.localEulerAngles.z);

        //Set the grabbed object's position and rotation to the rounded numbers so that they remain on the grid
        Vector3 roundedPos = new Vector3(roundedXPos, roundedYPos, roundedZPos);
        Quaternion roundedRot = Quaternion.Euler(roundedXRot, roundedYRot, roundedZRot);

        interact.grabbedObject.transform.position = roundedPos;
        interact.grabbedObject.transform.rotation = roundedRot;
        interact.grabbedObject.transform.localScale = new Vector3(1f, 1f, 1f);
        yield return null;
    }

    private IEnumerator ClimbLadder(Transform movePoint1, Transform movePoint2, Transform movePoint3) //function that controls how the player climbs ladders
    {
        transform.position = movePoint1.position; //moves the player to the first move point (ground)

        //move the player to the second move point (air)
        float elapsedTime = 0f;
        while (elapsedTime < 0.5f)
        {
            float t = elapsedTime / climbLadderDuration; //creates a time variables that controls the speed of the motion
            transform.position = Vector3.Lerp(transform.position, movePoint2.position, t); //creates an end position for the player to lerp to
            elapsedTime += Time.deltaTime; //increments the timer
            yield return new WaitForEndOfFrame();
        }

        transform.position = movePoint2.position; // ensures the player reaches the exact target position

        //move the player to the final move point (top of rock)
        elapsedTime = 0f; //creates a timer variable
        while (elapsedTime < 0.5f)
        {
            float t = elapsedTime / climbLadderDuration; //creates a time variables that controls the speed of the motion
            transform.position = Vector3.Lerp(transform.position, movePoint3.position, t); //creates an end position for the player to lerp to
            elapsedTime += Time.deltaTime; //increments the timer
            yield return new WaitForEndOfFrame();
        }

        transform.position = movePoint3.position; //ensures the player reaches the exact target position
        isClimbingLadder = false;

        rb.velocity = Vector3.zero;
    }
    #endregion

    #region Camera Functions
    //functions that dictate how the camera is controlled by the player
    public void Zoom(InputAction.CallbackContext context)
    {
        float zoom = context.ReadValue<float>() * zoomSpeed * Time.deltaTime;

        if (zoom != 0)
        {
            camera.m_Lens.FieldOfView += zoom;
        }
    }

    public void ZoomController(InputAction.CallbackContext context)
    {
        currentZoomInput = context.ReadValue<float>();
    }

    public void ShiftCameraClockwise(InputAction.CallbackContext context) //shifting camera function for the input system
    {
        if (negateNextInput)
        {
            negateNextInput = false;
            return;
        }
        else if(isInKeyTunnelTrigger || isInGringusTrigger)
        {
            return;
        }
        else if (context.started)
        {
            Vector3 currentRot = virtualCamera.transform.eulerAngles;
            currentRot.y += 90f;

            if (!isRotating) //if the camera is not currently rotating
            {
                switch (camOrientation)
                {
                    case 'F': //shifting from front to left
                        camOrientation = 'L';
                        camText.text = "Side: Left";
                        break;

                    case 'L': //shifting from left to back
                        camOrientation = 'B';
                        camText.text = "Side: Back";
                        break;

                    case 'B': //shifting from back to right
                        camOrientation = 'R';
                        camText.text = "Side: Right";
                        break;

                    case 'R': //shifting from right to front
                        camOrientation = 'F';
                        camText.text = "Side: Front";
                        break;
                }

                StartCoroutine(UpdateCameraRotation(currentRot));
            }
        }
    }

    public void ShiftCameraCounterClockwise(InputAction.CallbackContext context) //shifting camera function for the input system
    {
        if (negateNextInput)
        {
            negateNextInput = false;
            return;
        }
        else if (isInKeyTunnelTrigger || isInGringusTrigger)
        {
            return;
        }
        else if (context.started)
        {
            Vector3 currentRot = virtualCamera.transform.eulerAngles;
            currentRot.y -= 90f;

            if (!isRotating) //if the camera is not currently rotating
            {
                switch (camOrientation)
                {
                    case 'F': //shifting from front to right
                        camOrientation = 'R';
                        camText.text = "Side: Right";
                        break;

                    case 'R': //shifting from right to back
                        camOrientation = 'B';
                        camText.text = "Side: Back";
                        break;

                    case 'B': //shifting from back to left
                        camOrientation = 'L';
                        camText.text = "Side: Left";
                        break;

                    case 'L': //shifting from left to front
                        camOrientation = 'F';
                        camText.text = "Side: Front";
                        break;
                }

                StartCoroutine(UpdateCameraRotation(currentRot));
            }
        }
    }

    private IEnumerator UpdateCameraRotation(Vector3 rotVector) //function that handles camera shifting
    {
        isRotating = true;
        Quaternion newRot = Quaternion.Euler(rotVector);

        float elapsedTime = 0f;
        while (elapsedTime < camMoveDuration)
        {
            float t = elapsedTime / camMoveDuration; //interpolation value based on time 
            virtualCamera.transform.rotation = Quaternion.Lerp(camRot, newRot, t); //linearly interpolate the camera's position to the new one
            elapsedTime += Time.deltaTime; //update the amount of elapsed time
            yield return null; //wait for the next frame
        }

        virtualCamera.transform.rotation = newRot; //ensure the camera gets to the final position
        camRot = newRot; //update the camera's position to be the new one
        isRotating = false;
    }
    #endregion

    #region Player Reactionary Functions
    //functions that handle how the player reacts to the environment or enemies
    public IEnumerator TakeDamage(Transform enemy) //function that changes health amount
    {
        if (canTakeDamage) //if the player is not intangible (if they can take damage)
        {
            canTakeDamage = false; //activate temporary invincibility
            anim.SetTrigger("Take Damage"); //play damage animation
            transform.Find("Hurt SFX").GetComponent<AudioSource>().Play();

            //update health variable and associated UI element
            heartIcons[health - 1].SetActive(false);
            health--;

            Vector3 hitDirection = (transform.position - enemy.position).normalized; //create a vector representing where the player will get launched by the damage-giver
            Debug.Log(hitDirection);

            if (!isGrabbingRock && transform.position.y < enemy.position.y) //the player will not get launched if they are grabbing a rock or falling
            {
                if (health != 0) //if the player is dead, do not launch them 
                {
                    if (!isMoving) //if they are not moving, the launch power increases to match the power of the launch if they were moving
                    {
                        rb.AddForce(hitDirection * 70f, ForceMode.Impulse);

                    }
                    else
                    {
                        rb.AddForce(hitDirection * 20f, ForceMode.Impulse);
                    }
                }
            }

            StartCoroutine(enemy.gameObject.GetComponent<EnemyMovement>().BecomeIntangible());

            //turn off and on the player's model for a short while to show they are temporarily invincible
            float elapsedTime = 0f;

            if (!isGrabbingRock)
            {
                while (elapsedTime < 3f)
                {
                    transform.Find("guy").gameObject.SetActive(false);
                    yield return new WaitForSeconds(0.2f);
                    transform.Find("guy").gameObject.SetActive(true);
                    yield return new WaitForSeconds(0.2f);
                    elapsedTime += 0.4f;
                }
            }
            else
            {
                yield return new WaitForSeconds(3f);
            }

            canTakeDamage = true;
        }
    }

    private IEnumerator WinLevel() //function that fires when the player wins the level
    {
        anim.SetTrigger("Win"); //play win animation
        transform.Find("Bark SFX").GetComponent<AudioSource>().Play();

        //have the player look at the camera
        Vector3 lookPos = new Vector3(virtualCamera.transform.position.x, virtualCamera.transform.position.y, virtualCamera.transform.position.z);
        transform.LookAt(lookPos);

        hasPlayedWinningSequence = true;

        //play the level transition and load the next level
        yield return new WaitForSeconds(1f);
        ender.GetComponent<Animation>().clip = ender.GetComponent<Animation>().GetClip("IrisIn");
        ender.GetComponent<Animation>().Play();
    }

    private IEnumerator LoseLevel() //function that fires when the player loses the level
    {
        anim.SetBool("isDead", true); //play the dead animation

        //have the player look at the camera
        Vector3 lookPos = new Vector3(virtualCamera.transform.position.x, virtualCamera.transform.position.y, virtualCamera.transform.position.z);
        transform.LookAt(lookPos);
        yield return new WaitForSeconds(3f);
        //play the level transition and reload the current level
        ender.isRestarting = true;
        ender.GetComponent<Animation>().clip = ender.GetComponent<Animation>().GetClip("IrisIn");
        ender.GetComponent<Animation>().Play();
        yield return null;
    }

    private IEnumerator ToggleCollider()
    {
        hasToggled = true;
        /*
        transform.GetComponent<CapsuleCollider>().enabled = false;
        yield return new WaitForSeconds(0.1f);
        transform.GetComponent<CapsuleCollider>().enabled = true;
        */

        float radius = transform.GetComponent<CapsuleCollider>().radius;
        transform.GetComponent<CapsuleCollider>().radius = (radius / 2);
        yield return new WaitForSeconds(0.5f);
        transform.GetComponent<CapsuleCollider>().radius = radius;

        //transform.GetComponent<CapsuleCollider>().material = fallingMaterial;
    }
    #endregion

    #region Miscellaneous Functions
    public void ChangeKeys(int keyChange) //function that changes key amount
    {
        keys = keys + keyChange;
        keysNum.text = keys.ToString();
    }

    private void OnCollisionStay(Collision collision)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    #endregion

    #region Unity-Specific Functions
    private void Awake()
    {
        camera = GameObject.Find("Camera Parent").transform.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        fall = GameObject.Find("Sound Manager").transform.Find("Falling Spawner").GetComponent<FallingSFX>();
        climb = GameObject.Find("Sound Manager").transform.Find("Climbing Spawner").GetComponent<ClimbingSFX>();
        ender = GameObject.Find("Canvas").transform.Find("Iris Transition").GetComponent<EndLevel>();
    }

    // Start is called before the first frame update
    void Start()
    {
        isInKeyTunnelTrigger = false;
        isInGringusTrigger = false;

        camera.m_Lens.FieldOfView = 38.5f;
        rb = GetComponent<Rigidbody>();
        BF = null;
        camOrientation = 'F'; //sets the default orientation of the camera to front
        camRot = virtualCamera.transform.rotation; //gets the default position of the camera
        canTakeDamage = true;
    }

    private void Update()
    {
        // Calculate the zoom amount based on the input
        float zoomAmount = currentZoomInput * zoomSpeedController * Time.deltaTime;

        // Adjust the camera's field of view based on the direction of the input
        if (currentZoomInput != 0)
        {
            camera.m_Lens.FieldOfView += zoomAmount;
        }
    }

    private void FixedUpdate()
    {
        if (canTakeDamage)
        {
            transform.Find("guy").gameObject.SetActive(true);
        }

        if (camera.m_Lens.FieldOfView < maxZoom)
        {
            camera.m_Lens.FieldOfView = maxZoom;
        }

        if (camera.m_Lens.FieldOfView > minZoom)
        {
            camera.m_Lens.FieldOfView = minZoom;
        }

        rb.angularVelocity = Vector3.zero; //static angular velocity to prevent strange collision velocity issues

        //rewrite !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if(interact.grabbedObject != null)
        {
            if (isMovingRock && isGrabbingRock)
            {
                interact.grabbedObject.transform.Find("PushEffects").gameObject.transform.LookAt(transform);
                interact.grabbedObject.transform.Find("PushEffects").gameObject.SetActive(true);
            }
            else if(!isMovingRock && isGrabbingRock)
            {
               interact.grabbedObject.transform.Find("PushEffects").gameObject.SetActive(false);
            }
        }

        if (hasWon && !hasPlayedWinningSequence) //if the player has won the level, initiate the winning sequence
        {
            StartCoroutine(WinLevel());
        }

        if (health == 0) //if the player has 0 health, then they are dead
        {
            //disable all collisions for the player and make them untouchable while they are dead
            isDead = true;
            canTakeDamage = false;
            GetComponent<CapsuleCollider>().enabled = false;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            StartCoroutine(LoseLevel()); //initiate the winning sequence
        }

        if (isDead || isClimbingLadder || hasWon) //if the player character is in any of these states, they cannot move
        {
            allowInput = false;
            anim.SetBool("isRunning", false);
            transform.Find("Walking SFX").GetComponent<AudioSource>().Stop();
        }
        else if (isFalling)
        {
            if (!hasToggled)
            {
                StartCoroutine(ToggleCollider());
            }
            rb.mass = 500;
            allowInput = false;
            anim.SetBool("isRunning", false);
            anim.SetBool("isFalling", true);
            fall.PlayFallSFX();

            if (isMovingRock)
            {
                StartCoroutine(RoundBlockPos());
            }
        }
        else //if the player is not in any of the above states, they can move
        {
            if (ender.canPlay)
            {
                transform.GetComponent<CapsuleCollider>().material = null;
                rb.mass = 1;
                allowInput = true;
                anim.SetBool("isFalling", false);
                hasToggled = false;
            }
        }

        if (allowInput) //if the player is allowed to move
        {
            if ((movementInput != Vector2.zero)) //if the player is moving
            {
                UpdateMovementAndRotation();
                isMoving = true;
            }
            else if (isAdjustingLocation)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
                anim.SetBool("isRunning", false);
                transform.Find("Walking SFX").GetComponent<AudioSource>().Stop();
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
    #endregion
}
