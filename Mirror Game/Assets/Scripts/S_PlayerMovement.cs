using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class S_PlayerMovement : MonoBehaviour
{
    S_GameManager gameManagerScr;
    public float playerSpeed = 0.5f;
    private Rigidbody playerRigidbody, mirrorRigidbody;
    GameObject player, mirror, timeImage;
    Animator timeAnim;
    S_PlayerCollisions playerCollScr, mirrorCollScr;

    private Vector3 moveInput, moveVelocity, startTilt;

    public bool isFlat = false;
    private bool bReadyToClick = true;

    private Camera mainCamera;

    Ray cameraRay;

    void Start()
    {
        //create references to required objects
        playerRigidbody = GetComponent<Rigidbody>();
        mainCamera = FindObjectOfType<Camera>();
        player = gameObject;
        playerCollScr = player.GetComponent<S_PlayerCollisions>();
        mirror = GameObject.Find("Mirror");
        mirrorCollScr = mirror.GetComponent<S_PlayerCollisions>();
        mirrorRigidbody = mirror.GetComponent<Rigidbody>();

        startTilt = Input.acceleration; //this is used to centre the movement based on the position the user started in rather than true flat

        gameManagerScr = GameObject.Find("_GameManager").GetComponent<S_GameManager>();
        gameManagerScr.bGameInProgress = false; //set game to not in progress as the pop up menu will be onscreen

        timeImage = GameObject.FindGameObjectWithTag("TimeImage");
        timeAnim = timeImage.GetComponent<Animator>();
        timeImage.SetActive(false);
    }

    void Update()
    {
        if (gameManagerScr.bGameInProgress) //only performs checks if the game is in progress
        {
            if (!Application.isMobilePlatform) //platform specific input
            {
                cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition); //create a raycast to the mouse position
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
                float rayLength;

                if (groundPlane.Raycast(cameraRay, out rayLength)) //if the mouse is over the plane
                {
                    Vector3 pointToMove = (cameraRay.GetPoint(rayLength) - transform.position); // set a point to move towards

                    moveInput = new Vector3(pointToMove.x, transform.position.y, pointToMove.z); //create a vector with the correct y value to move towards

                    if (moveInput.sqrMagnitude <= 0.6f) //dead zone to stop jittery movement
                    {
                        moveInput = new Vector3(0.0f, transform.position.y, 0.0f); //set movement to 0
                    }
                    moveInput = Vector3.Normalize(moveInput); //normalise the move vector to ensure its always the same speed
                }

                moveVelocity = moveInput * playerSpeed * 70; //multiply the vector by a constant speed value (*70 just makes the playerSpeed a nicer number)
                
                //collision checks to limit movement of other object, mirror/player, based on collisions
                if (!playerCollScr.bInCollision) //if player isn't in a collision then the mirror can move
                {
                    mirrorRigidbody.velocity = new Vector3(-1.0f*moveVelocity.x, moveVelocity.y, -1.0f*moveVelocity.z);
                }
                if (!mirrorCollScr.bInCollision) //if mirror isn't in a collision then the player can move
                {
                    playerRigidbody.velocity = moveVelocity;
                }
                if (mirrorCollScr.bInCollision && playerCollScr.bInCollision) //if both objects are in a collision, both can move to avoid getting stuck
                {
                    playerRigidbody.velocity = moveVelocity;
                    mirrorRigidbody.velocity = new Vector3(-1.0f * moveVelocity.x, moveVelocity.y, -1.0f * moveVelocity.z);
                }
                //check to fix displacement errors if mirror object moves out of sync with the player position
                if (!mirrorCollScr.bInCollision && !playerCollScr.bInCollision && mirror.transform.position.x != (player.transform.position.x*-1) && mirror.transform.position.z != player.transform.position.z * -1)
                {
                    Vector3 oppositePos = player.transform.position*-1;
                    oppositePos.y = player.transform.position.y;
                    mirror.transform.position = oppositePos;
                }

                //used to activate timewarp
                if (gameManagerScr.timeWarpCount > 0) //if the player has bought a timewarp
                {
                    //if the player clicks the screen and not a UI element
                    if (bReadyToClick && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        gameManagerScr.timeWarpCount--; //reduce number of timewarps available
                        gameManagerScr.UpdateScores(); //update UI
                        bReadyToClick = false; //lock to stop them using two timewarps by accident
                        timeImage.SetActive(true); //give visual feedback that the timewarp is active
                        Time.timeScale = 0.25f; //slow down time to a quarter speed
                        Invoke("ResetSpeed", 3f); //in 3 seconds (12 seconds realtime) reset the speed
                        Invoke("StartFlash", 1f); //start the flashing effect after 1 third of the time
                    }
                }
            }
            else if (Application.isMobilePlatform) //mobile specific movement input
            {
                /////////////// Start of adapted code from N3K EN, 2017 //////////////////////

                Vector3 tilt = Input.acceleration - startTilt; //work out the tilt displacement from the start position

                if (isFlat)
                {
                    tilt = Quaternion.Euler(90, 0, 0) * tilt * playerSpeed * 100; //set the vector to be the correct rotation (device face up)
                }
                //code to limit movement on mobile based on collisions
                if (!playerCollScr.bInCollision) //if player isn't in collision, allow movement
                {
                    mirrorRigidbody.velocity = new Vector3(-1.0f * tilt.x, tilt.y, -1.0f * tilt.z);
                }
                if (!mirrorCollScr.bInCollision) //if mirror isn't in collision, allow movement
                {
                    playerRigidbody.velocity = tilt;
                }
                if (mirrorCollScr.bInCollision && playerCollScr.bInCollision) //if both are in collision, allow movement to stop getting stuck
                {
                    playerRigidbody.velocity = tilt;
                    mirrorRigidbody.velocity = new Vector3(-1.0f * tilt.x, tilt.y, -1.0f * tilt.z);
                }


                /////////////// End of adapted code from N3K EN, 2017 //////////////////////

                //used to activate timewarp on mobile platform
                if (gameManagerScr.timeWarpCount > 0) //if player has bought a timewarp
                {
                    if (bReadyToClick && Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) //if they tap the screen
                    {
                        if (!EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId)) //check tap isn't on UI element
                        {
                            //perform same actions as pc platform
                            gameManagerScr.timeWarpCount--;
                            gameManagerScr.UpdateScores();
                            bReadyToClick = false;
                            timeImage.SetActive(true);
                            Image timeImageImg = timeImage.GetComponent<Image>();
                            timeImageImg.color = new Color(timeImageImg.color.r, timeImageImg.color.g, timeImageImg.color.b, 255);
                            Time.timeScale = 0.25f; //slow time down in the game
                            //start timers for resetting speed and displaying flash
                            Invoke("ResetSpeed", 3f);
                            Invoke("StartFlash", 1f);
                        }                       
                    }
                }
            }

        }
        else
        {
            //fail safe to stop any movement whilst the game is not in progress
            playerRigidbody.velocity = Vector3.zero;
            mirrorRigidbody.velocity = Vector3.zero;
        }
    }

    //Function to find starting position of device
    public void ZeroTilt()
    {
        startTilt = Input.acceleration;
        isFlat = true;
    }

    //function used to reset the game speed back to 1 after entering timewarp mode
    void ResetSpeed()
    {
        Time.timeScale = 1;
        timeAnim.SetBool("bFlashing", false); //bool used to control flashing
        timeImage.SetActive(false);
        bReadyToClick = true; //unlock the time warp ready for another use
    }

    //Function used to start the flash animation on the timewarp image
    void StartFlash()
    {
        timeAnim.SetBool("bFlashing", true);
    }
}
