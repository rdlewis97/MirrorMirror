using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class S_EndGameUI : MonoBehaviour {

    [SerializeField] GameObject left, right, timeBoard, playButton;
    [SerializeField] Animator leftAnim, rightAnim;

    [SerializeField] Text congratulationsText, totalTimeText, levelBreakdownTimeText;
    string TimeString, level1Time, level2Time, level3Time, level4Time, level5Time, level6Time, level7Time;

    S_GameManager gameManagerScr;
    AudioSource mySound;

	// Use this for initialization
	void Start () {
        gameManagerScr = GameObject.FindGameObjectWithTag("GameManager").GetComponent<S_GameManager>();
        mySound = GetComponent<AudioSource>();
	}

    //Function called when the game is completed
    public void OnGameOver()
    {
        //trigger animations to slide in
        left.SetActive(true);
        right.SetActive(true);
        leftAnim.SetTrigger("Close");
        rightAnim.SetTrigger("Close");
        Invoke("DisplayUI", 0.75f); //display the scores after a short delay

        //custom event to report to analytics that the game has been completed
        //passes the coins collected, total deaths, shields owned and timewarps owned
        Analytics.CustomEvent("gameCompleted", new Dictionary<string, object>{
            { "coinsCollected", gameManagerScr.coinChange},
            { "deaths", gameManagerScr.deaths},
            { "shields", gameManagerScr.shieldCount},
            { "timeWarps", gameManagerScr.timeWarpCount}
        });

        //check to make sure sound isnt already playing
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound); //play button click sound
        }
    }

    //Function to display UI element showing stats and scores
    void DisplayUI()
    {
        //enable necessary UI elements
        timeBoard.SetActive(true);
        playButton.SetActive(true);

        //create a string to display the total time taken
        TimeString = ((int)gameManagerScr.totalTime / 60)+ ":" + ((int)gameManagerScr.totalTime%60);
        totalTimeText.text = "You did it in a time of: " + TimeString + " with " + gameManagerScr.deaths + " deaths!";

        //create strings for each level to display individual time and deaths per level
        level1Time = ((int)gameManagerScr.level1Time / 60 + ":" + (int)gameManagerScr.level1Time % 60);
        level2Time = ((int)gameManagerScr.level2Time / 60 + ":" + (int)gameManagerScr.level2Time % 60);
        level3Time = ((int)gameManagerScr.level3Time / 60 + ":" + (int)gameManagerScr.level3Time % 60);
        level4Time = ((int)gameManagerScr.level4Time / 60 + ":" + (int)gameManagerScr.level4Time % 60);
        level5Time = ((int)gameManagerScr.level5Time / 60 + ":" + (int)gameManagerScr.level5Time % 60);
        level6Time = ((int)gameManagerScr.level6Time / 60 + ":" + (int)gameManagerScr.level6Time % 60);
        level7Time = ((int)gameManagerScr.level7Time / 60 + ":" + (int)gameManagerScr.level7Time % 60);

        //concatenate all individual level strings in one string
        levelBreakdownTimeText.text = "Level 1: " + level1Time + " with " + gameManagerScr.level1Deaths + " deaths!" + "\nLevel 2: " + level2Time + " with " + gameManagerScr.level2Deaths + " deaths!" + "\nLevel 3: " + level3Time + " with " + gameManagerScr.level3Deaths + " deaths!" + "\nLevel 4: " + level4Time + " with " + gameManagerScr.level4Deaths + " deaths!" + "\nLevel 5: " + level5Time + " with " + gameManagerScr.level5Deaths + " deaths!" + "\nLevel 6: " + level6Time + " with " + gameManagerScr.level6Deaths + " deaths!" + "\nLevel 7: " + level7Time + " with " + gameManagerScr.level7Deaths + " deaths!";
    }

    //Function called when play button is clicked to return to menu
    public void OnMenuClicked()
    {
        //play feedback sound
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound);
        }
        gameManagerScr.currentLevel = 0; //reset gamemanager current level back to 0
        SceneManager.LoadScene(0); //load menu scene
    }
}
