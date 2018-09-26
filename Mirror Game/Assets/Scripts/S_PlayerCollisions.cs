using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_PlayerCollisions : MonoBehaviour {

    GameObject endCube;
    S_GameManager gameManagerScr;
    Material playerMat, otherMat;
    [SerializeField] Material shieldMat;
    public bool bInCollision = false;

    // Use this for initialization
    void Start () {
        //find and get references to end cube and gamemanager
        endCube = GameObject.Find("End Cube");
        gameManagerScr = GameObject.Find("_GameManager").GetComponent<S_GameManager>();
        gameManagerScr.UpdateScores(); //update UI on game start
        playerMat = gameObject.GetComponent<Renderer>().material; //get the material of the player object (mirror or player)
    }

    //Function called when the player or mirror object enters a collision with another object
    private void OnTriggerEnter(Collider other)
    {
        //if the object is the end cube and its in the tutorial, move to the next tutorial stage
        if (other.gameObject == endCube && gameObject.name == "Player" && gameManagerScr.tutorialCount < 3)
        {
            gameManagerScr.NextTutorialStage();
        }
        else if (other.gameObject == endCube && gameObject.name == "Player" && gameManagerScr.tutorialCount == 3)
        {
            //if the object is the end cube and the tutorial is finished then call next level
            gameManagerScr.bTutorialFinished = true; //bool to let other scripts know tutorial is finished
            gameManagerScr.OnLevelCompleted();
        }
        //if the object is a barrier
        if (other.gameObject.tag == "Barrier" && gameObject.tag == "Player")
        {
            bInCollision = true; //lock used to only run code once per object
            if (gameManagerScr.shieldCount > 0 && !gameManagerScr.bInvincible) //if player has a shield active and isnt in invincible state
            {
                //play negative feedback sound
                gameManagerScr.FailedLevelSound();
                gameManagerScr.bInvincible = true; //set player and mirror to invincible state
                Invoke("ResetInvincibility", 2.0f); //start a timer to reset the invincibility after 2 seconds
                gameManagerScr.shieldCount--; //reduce players shield count by one
                gameManagerScr.UpdateScores(); //update UI
                gameObject.GetComponent<Renderer>().material = shieldMat; //set objects material to invincible texture
                //apply material to opposite object also
                if (gameObject.name == "Player")
                {
                    otherMat = GameObject.Find("Mirror").GetComponent<Renderer>().material;
                    GameObject.Find("Mirror").GetComponent<Renderer>().material = shieldMat;
                }
                else
                {
                    otherMat = GameObject.Find("Player").GetComponent<Renderer>().material;
                    GameObject.Find("Player").GetComponent<Renderer>().material = shieldMat;
                }
                
            }
            else if (gameManagerScr.shieldCount <= 0 && !gameManagerScr.bInvincible) //if player doesn't have a shield available
            {
                gameManagerScr.OnLevelFailed(); //call level failed function to reset the level
            }

        }
        if (other.gameObject.tag == "Coin") //if the collision is with a coin object
        {
            gameManagerScr.CoinCollectSound(); //play coin collect sound
            other.GetComponent<Collider>().enabled = false; //turn the coins collider off to stop it triggering if it hits the other object on way to top UI
            StartCoroutine(DeleteCoin(other.gameObject)); //use co-routine to move, expand and then delete coin object
        }
    }

    //function used to determine if the player object is still in a collision with the barriers
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Barrier" && gameObject.tag == "Player")
        {
            bInCollision = false; //set a flag to say collision is over
        }
    }

    //Function used to reset the invincible flag and material of player object and opposite object
    void ResetInvincibility()
    {
        gameManagerScr.bInvincible = false;

        gameObject.GetComponent<Renderer>().material = playerMat;
        if (gameObject.name == "Player")
        {
            GameObject.Find("Mirror").GetComponent<Renderer>().material = otherMat;
        }
        else
        {
            GameObject.Find("Player").GetComponent<Renderer>().material = otherMat;
        }
    }

    //co-routine used to move, scale and destroy coin object
    IEnumerator DeleteCoin(GameObject coin) //reference to coin object is passed into function
    {
        //reset values each time a coin is collected
        float elapsedTime = 0;
        float totalTime;
        //fix for coins collected when in timewarp mode
        if (Time.timeScale < 1)
        {
            totalTime = 0.5f; //reduce time the coroutine runs for if in slow motion
        }
        else
        {
            totalTime = 0.75f;
        }

        Vector3 startingPos = coin.transform.position; //store a reference to coins start position
        Vector3 newPos = new Vector3(-27, coin.transform.position.y + 3, 45); //position UI element is in, but in world space, in top corner
        while (elapsedTime < totalTime) //while loop to perform movement within total time
        {
            if (coin) //if reference to coin exists
            {
                coin.transform.position = Vector3.Lerp(startingPos, newPos, (elapsedTime / totalTime)); //lerp towards end position
                if (coin.transform.localScale.x < 3.5f || coin.transform.localScale.z < 3.5f) //if the coins scale is less than the desired scale
                {
                    coin.transform.localScale += new Vector3(0.02f, 0.0f, 0.02f); //increase the size of the coin
                }
                elapsedTime += Time.deltaTime; //increment elapsed time per frame
            }
            yield return null;
        }
        if (coin) //if reference to coin exists and while loop has finished
        {
            Destroy(coin); //destroy the coin object
            gameManagerScr.OnCoinCollect(); //call the coin collect function on gamemanager
            yield return null; //end coroutine
        }
    }
}
