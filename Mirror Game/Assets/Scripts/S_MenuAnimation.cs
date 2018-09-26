using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

[RequireComponent(typeof(AudioSource))]

public class S_MenuAnimation : MonoBehaviour {

    S_GameManager gameManagerScr;
    [SerializeField] Animator leftAnim, rightAnim, textAnim, playAnim;
    [SerializeField] GameObject tutorialScroll, playButton, leaderboardsButton, shopButton, shopImage, leaderboardsImage, achievementsImage, blueBG, scrollImg, achievementsButton, settingsButton;
    [SerializeField] bool bAnimationFinished = false;
    [SerializeField] RectTransform coinsButtons, moneyButtons;

    AudioSource mySound;

    void Start()
    {
        gameManagerScr = GameObject.Find("_GameManager").GetComponent<S_GameManager>();
        moneyButtons.gameObject.SetActive(false);
        mySound = GetComponent<AudioSource>();
        InvokeRepeating("PlayButtonAnimation", 2.0f, 4.0f); //start the animation loop for the playbutton animations
        bAnimationFinished = false; //bool used to track when animations are finished
        Time.timeScale = 1.0f; //ensure timescale is at 1 if player returned to menu during timewarp mode

        /////////////////////// Google Play Services Stuff ////////////////////////////////////////////////
        /*PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
        //SignIn();

*/

    }

    //Function for Google Play Services to sign in player to google account
    /*public void SignIn()
    {
        signInText.text = "Signing In...";
        Social.localUser.Authenticate((bool success) => {
            if (success)
            {
                signInText.text = "Authenticated";
                playButton.SetActive(true);
                leaderboardsButton.SetActive(true);
                shopButton.SetActive(true);
                signInButton.SetActive(false);
            }
            else if (!success)
            {
                signInText.text = "Failed to sign in";
            }
        });
    }*/

    //function used to loop play button animations
    void PlayButtonAnimation()
    {
        playAnim.SetTrigger("Use");
    }

    //Function called when play button is clicked on main menu
    public void PlayClicked()
    {
        CancelInvoke(); //cancel any invokes used for animations
        TriggerAnimations(); //trigger the parting animation of UI

        if (gameManagerScr.currentLevel == 0) //if game isn't resuming
        {
            tutorialScroll.SetActive(true); //show tutorial pop-up
        }
        else //if the user has returned to main menu during game
        {
            Invoke("ContinueGame", 1.0f); //continue from where player left off
        }
        //play feedback sound if one isn't already playing
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
    }

    //Function used to resume level if player exited to menu
    void ContinueGame()
    {
        SceneManager.LoadScene(gameManagerScr.currentLevel);
    }

    //Function called when shop button is clicked on main menu
    public void ShopClicked()
    {
        TriggerAnimations(); //trigger the parting animation of UI
        shopImage.SetActive(true); //display shop UI
        gameManagerScr.UpdateScores(); //update UI
        //play feedback sound
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
    }

    //Function called when shop, leaderboards and achievement back buttons are clicked
    public void ShopBackClicked()
    {
        //enable menu UI elements
        leftAnim.gameObject.SetActive(true);
        rightAnim.gameObject.SetActive(true);
        textAnim.gameObject.SetActive(true);
        //start closing animation of menu UI elements
        leftAnim.SetTrigger("Close");
        rightAnim.SetTrigger("Close");
        textAnim.SetTrigger("Close");
        //after 2 second delay, show UI buttons for main menu
        Invoke("ToggleOn", 2.0f);
        //play feedback sound
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
    }

    //Function called when the leaderboards button is clicked
    public void LeaderboardsClicked()
    {
        TriggerAnimations(); //trigger the parting animation of UI
        //Social.ShowLeaderboardUI(); //google play leaderboards here
        leaderboardsImage.SetActive(true); //display the leaderboard UI pop-up
        //play feedback sound if sound isn't already playing
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
    }

    public void AchievementsClicked()
    {
        TriggerAnimations(); //trigger the parting animation of UI
        //google play achievements here
        achievementsImage.SetActive(true);
        //play feedback sound if sound isn't already playing
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
    }
    
    //Function used to trigger the parting animation of UI
    public void TriggerAnimations()
    {
        //start the animation on the two halves and the text
        leftAnim.SetTrigger("Use");
        rightAnim.SetTrigger("Use");
        textAnim.SetTrigger("Use");
        //hide all UI buttons
        playButton.SetActive(false);
        leaderboardsButton.SetActive(false);
        shopButton.SetActive(false);
        achievementsButton.SetActive(false);
        settingsButton.SetActive(false);

        Invoke("ToggleOff", 1.2f); //invoke a function to turn off the off screen UI elements
    }

    //Function called when the tutorial is started
    public void OnAnimationFinish()
    {
        //reports that a new game with tutorial has been started to the analytics dashboard
        Analytics.CustomEvent("gameStarted", new Dictionary<string, object>{
            { "gameStarted", true}
        });

        gameManagerScr.NextLevel(); //load the next level
        //play feedback sound if sound isn't already playing
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
    }

    //Function called when skip tutorial is clicked
    public void SkipTutorial()
    {
        //reports that the tutorial was skipped to analytics dashboard
        Analytics.CustomEvent("tutorialSkipped", new Dictionary<string, object>{
            { "skipped", true}
        });

        gameManagerScr.currentLevel = 1; //sets current level to level 1 to skip tutorial
        gameManagerScr.tutorialCount = 3; //sets variables as if tutorial has been completed
        gameManagerScr.coins += 4; //add extra coins that could have been earnt in tutorial
        gameManagerScr.bTutorialFinished = true; //set flag that tutorial is finished for other scripts
        gameManagerScr.OnLevelCompleted(); //call function to move to next level
        //play feedback sound if sound isn't already playing
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
    }

    //Function used to turn off, off-screen UI elements after parting animation
    void ToggleOff()
    {
        leftAnim.gameObject.SetActive(false);
        rightAnim.gameObject.SetActive(false);
        textAnim.gameObject.SetActive(false);
    }

    //Function used to turn on buttons after closing animation is played
    void ToggleOn()
    {
        //set menu UI buttons visible
        playButton.SetActive(true);
        leaderboardsButton.SetActive(true);
        shopButton.SetActive(true);
        achievementsButton.SetActive(true);
        settingsButton.SetActive(true);
        //ensure shop, leaderboards, achievement and tutorial scroll is off
        shopImage.SetActive(false);
        leaderboardsImage.SetActive(false);
        achievementsImage.SetActive(false);
        tutorialScroll.SetActive(false);
    }

    //Function called when coins tab is clicked in the shop
    public void OnCoinsClicked()
    {
        coinsButtons.gameObject.SetActive(true);
        scrollImg.GetComponent<ScrollRect>().content = coinsButtons; //swap items in scroll list
        moneyButtons.gameObject.SetActive(false);
        //play feedback sound if sound isn't already playing
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
    }

    //Function called when money tab is clicked in the shop
    public void OnMoneyClicked()
    {
        moneyButtons.gameObject.SetActive(true);
        scrollImg.GetComponent<ScrollRect>().content = moneyButtons; //swap items in scroll list
        coinsButtons.gameObject.SetActive(false);
        //play feedback sound if sound isn't already playing
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
    }
}
