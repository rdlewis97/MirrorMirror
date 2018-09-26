using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;

[RequireComponent(typeof(AudioSource))] //ensure there is an audiosource component on the game manager

public class S_GameManager : MonoBehaviour {

    public static S_GameManager control; //create a singleton gamemanager

    //stored values for game volume
    public float volume = 1.0f; 
    public float SFXVolume = 1.0f;

    [Header("Scores")]
    public int coins = 6;
    public int coinsTemp = 0;
    public int coinChange = 0;
    public int timeWarpCount = 0;
    public int shieldCount = 0;
    public int currentLevel = 0;
    public int tutorialCount = 0;
    public int deaths = 0;
    public int level1Deaths = 0;
    public int level2Deaths = 0;
    public int level3Deaths = 0;
    public int level4Deaths = 0;
    public int level5Deaths = 0;
    public int level6Deaths = 0;
    public int level7Deaths = 0;
    public int currentDeaths = 0;


    [Header("Level Times")]
    public float currentTime;
    public float totalTime = 0.0f;
    public float level1Time = 0.0f;
    public float level2Time = 0.0f;
    public float level3Time = 0.0f;
    public float level4Time = 0.0f;
    public float level5Time = 0.0f;
    public float level6Time = 0.0f;
    public float level7Time = 0.0f;

    private int menu = 0, tutorial = 1, level1 = 2, level2 = 3, level3 = 4, level4 = 5, level5 = 6, level6 = 7, level7 = 8, levelCount = 8, totalTutorialStages = 3, deathCounter = 0, coinStart = 0;

    Text coinsScoreText, shieldCountText, timeWarpCountText;
    public GameObject playerShield, mirrorShield;

    [Header("Bools")]
    public bool bGameInProgress = false;
    public bool bShieldActive = false;
    public bool bTutorialFinished = false;
    public bool bInvincible = false;
    public bool bOnLevelFailedLock = false;
    public bool bIsAdPlaying = false;

    [Header("Sounds")]
    public AudioClip music;
    public AudioClip completeLevelSound;
    public AudioClip coinCollectSound;
    public AudioClip failedLevelSound;
    public AudioClip buttonClickSound;
    public AudioClip buySound;
    private AudioSource mySound;

    [SerializeField] ParticleSystem confettiSystem; //used for end of level celebration

    void Awake()
    {
        //singleton checks
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
        //continuous music
        if (!mySound)
        {
            mySound = GetComponent<AudioSource>();
        }
        //check to perform code on every scene loading except the menu
        if (SceneManager.GetActiveScene().buildIndex != menu)
        {
            //initialise UI elements
            coinsScoreText = GameObject.Find("Coins Score Text").GetComponent<Text>();
            shieldCountText = GameObject.Find("Shield Score Text").GetComponent<Text>();
            timeWarpCountText = GameObject.Find("Bomb Score Text").GetComponent<Text>();
            //find a reference to the player and mirror shields
            playerShield = GameObject.FindGameObjectWithTag("PlayerShield");
            mirrorShield = GameObject.FindGameObjectWithTag("MirrorShield");
        }
        else
        {
            //used to track how many coins are collected in this session. Was used for Google Play achievement tracking...
            coinStart = coins;
        }
    }
    private void Update()
    {
        // timer to track time in each level
        if (bGameInProgress)
        {
            currentTime += Time.deltaTime;
        }
    }
    
    // Function called when player reaches the end zone of a level
    public void OnLevelCompleted()
    {
        //plays level complete sound effect
        mySound.PlayOneShot(completeLevelSound, SFXVolume);
        //plays the confetti particle system
        confettiSystem.Play();
        //used for analytic custom event to report level name for more readable data in dashboard
        string levelString;
        if (currentLevel > 1)
        {
            levelString = "Level " + (currentLevel - 1);
        }
        else if (currentLevel == 1)
        {
            levelString = "Tutorial";
        }
        else
        {
            levelString = "Menu";
        }
        //analytics custom event to report the name of each level completed
        Analytics.CustomEvent("levelCompleted", new Dictionary<string, object>{
            { "level", levelString},
            { "coinsCollected", coinsTemp},
            { "deaths", deaths},
            { "time", currentTime}
        });

        bGameInProgress = false; //let other scripts know that the gameplay has stopped
        //assign the time recorded for the level and the number of deaths to the correct variable for each level
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0: break;
            case 1: break;
            case 2: level1Time = currentTime; level1Deaths = currentDeaths;  break;
            case 3: level2Time = currentTime; level2Deaths = currentDeaths; break;
            case 4: level3Time = currentTime; level3Deaths = currentDeaths; break;
            case 5: level4Time = currentTime; level4Deaths = currentDeaths; break;
            case 6: level5Time = currentTime; level5Deaths = currentDeaths; break;
            case 7: level6Time = currentTime; level6Deaths = currentDeaths; break;
            case 8: level7Time = currentTime; level7Deaths = currentDeaths; break;
            default: break;
        }
        //calculate a total time for the current session
        totalTime = level1Time + level2Time + level3Time + level4Time + level5Time + level6Time + level7Time;
        //reset level based variables that track time, deaths and coins collected per level
        currentTime = 0.0f;
        currentDeaths = 0;
        coinsTemp = 0;
        bInvincible = false; //reset invincibility

        NextLevel(); //function called to perform the scene transition
    }

    //function that handles scene transitions
    public void NextLevel()
    {
        currentLevel++; //increment level variable to next level
        if (currentLevel <= levelCount) //if it hasn't reached the end of the game
        {
            SceneManager.LoadScene(currentLevel); //load the next level
        }
        else
        {
            coinChange = coins - coinStart; //used to track coin change during session for Google Play Achievements
            /*if (coinChange > 23)
            {
                //unlock achievement "The Banker" on Google Play
                // - collect every coin in every level in one play through
            }*/
            GameObject.FindGameObjectWithTag("EndGameUI").GetComponent<S_EndGameUI>().OnGameOver(); //display end of game UI with scores and buttons
        }
    }

    //Function to increment the tutorial to the next level
    public void NextTutorialStage()
    {
        mySound.PlayOneShot(completeLevelSound, SFXVolume); //play completed level sound
        confettiSystem.Play(); //play confetti particle system to celebrate level completion
        tutorialCount++; //increment tutorial stage
        bGameInProgress = false;
        coinsTemp = 0;
        bInvincible = false; //reset invincibility
        //check if tutorial is completed
        if (tutorialCount > totalTutorialStages)
        {
            //if it is, then increment the level and load the next level
            currentLevel++;
            SceneManager.LoadScene(currentLevel);
        }
        else
        {
            //if it isn't then load the tutorial scene again to reset it for the next stage
            SceneManager.LoadScene(tutorial);
        }
    }

    //Function to play the failed level sound. In a seperate function so that it could be played on level failed AND when shield damage occurs
    public void FailedLevelSound()
    {
        mySound.PlayOneShot(failedLevelSound, SFXVolume);
    }

    //Function for when the player dies
    public void OnLevelFailed()
    {
        //uses a lock to ensure the function doesn't run twice when both player and mirror hit barrier on symmetrical level
        if (!bOnLevelFailedLock)
        {
            FailedLevelSound();
            bOnLevelFailedLock = true;
            Time.timeScale = 1.0f; //ensure slow motion mode gets reset on death
            coins -= coinsTemp; //take away any coins collected in current level
            coinsTemp = 0;
            bInvincible = false; //reset invincibility
            //currentTime = 0.0f; if this is active, timer only shows time of completed attempt on level

            //increment death counters
            currentDeaths++; //deaths on current level
            deaths++; //deaths for current session

            //increment the AD death counter variable and check if Advert should play
            deathCounter++;
            if (deathCounter >= 3)
            {
                //show advert
                ShowAd();
                deathCounter = 0; //reset counter
                coins++; // reward player with a single coin for watching ad
            }

            UpdateScores(); //update in-game UI stats
            SceneManager.LoadScene(currentLevel); //reload the current level
        }
    }

    ///////////////// Start of functions adapted from Unity, c2018 /////////////////////////
    //Function to display a regular advert
    void ShowAd()
    {
        if (Advertisement.IsReady()) //checks if there is an advert ready to play
        {
            //Use result callback to ensure the player can't get stuck waiting for an ad to show if it fails to load for some reason
            ShowOptions options = new ShowOptions();
            options.resultCallback = HandleFreeADShowResult;

            Advertisement.Show(options);
            bIsAdPlaying = true; //boolean keeps track of when adverts are playing
        }
    }
    
    //Function performs actions based on result of displaying the ad
    void HandleFreeADShowResult(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            bIsAdPlaying = false; //sets bool to say the advert has finished playing
        }
        else if (result == ShowResult.Skipped)
        {
            bIsAdPlaying = false;
        }
        else if (result == ShowResult.Failed)
        {
            Debug.LogError("Video failed to show");
            bIsAdPlaying = false;
        }
    }

    //Function to display a rewarded advert. Used in the shop in exchange for free coins
    public void ShowRewardedVideo()
    {
        ShowOptions options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        Advertisement.Show("rewardedVideo", options);
    }

    void HandleShowResult(ShowResult result)
    {
        if (result == ShowResult.Finished)
        {
            Debug.Log("Video completed - Offer a reward to the player");
            // Reward player with 2 free coins
            coins += 2;
            bIsAdPlaying = false;
            UpdateScores(); //update UI stats
        }
        else if (result == ShowResult.Skipped)
        {
            Debug.LogWarning("Video was skipped - Do NOT reward the player");
            bIsAdPlaying = false;
        }
        else if (result == ShowResult.Failed)
        {
            Debug.LogError("Video failed to show");
            bIsAdPlaying = false;
        }
    }
///////////////// End of adapted functions from Unity, c2018 ////////////////////////////////

    //Function called when a coin is collected
    public void OnCoinCollect()
    {
        coins++; //increment coin count
        coinsTemp++; //increment coins collected on current level
        UpdateScores();
    }

    //Function to play coin collected sound on collision with player rather then when the coin reaches the UI
    public void CoinCollectSound()
    {
        mySound.PlayOneShot(coinCollectSound, SFXVolume);
    }

    //Function to update the stats on the UI. Done manually for performance optimisation so only done when necessary and not every frame
    public void UpdateScores()
    {
        if (coinsScoreText == null || timeWarpCountText == null || shieldCountText == null)
        {
            coinsScoreText = GameObject.Find("Coins Score Text").GetComponent<Text>();
            shieldCountText = GameObject.Find("Shield Score Text").GetComponent<Text>();
            timeWarpCountText = GameObject.Find("Bomb Score Text").GetComponent<Text>();
        }
        //convert int variables to strings
        coinsScoreText.text = coins.ToString();
        timeWarpCountText.text = timeWarpCount.ToString();
        shieldCountText.text = shieldCount.ToString();

        //only performed on levels, not on menu
        if (SceneManager.GetActiveScene().buildIndex > 0)
        {
            if (playerShield == null || mirrorShield == null)
            {
                playerShield = GameObject.FindGameObjectWithTag("PlayerShield");
                mirrorShield = GameObject.FindGameObjectWithTag("MirrorShield");
            }
            //manage shield visibility
            if (shieldCount > 0)
            {
                playerShield.GetComponent<Renderer>().enabled = true;
                playerShield.GetComponent<S_Shield>().enabled = true;
                mirrorShield.GetComponent<Renderer>().enabled = true;
                mirrorShield.GetComponent<S_Shield>().enabled = true;
            }
            else
            {
                playerShield.GetComponent<Renderer>().enabled = false;
                playerShield.GetComponent<S_Shield>().enabled = false;
                mirrorShield.GetComponent<Renderer>().enabled = false;
                mirrorShield.GetComponent<S_Shield>().enabled = false;
            }
        }
        
    }
}
