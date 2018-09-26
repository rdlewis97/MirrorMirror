using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(AudioSource))]

public class S_UIManager : MonoBehaviour {

    S_GameManager gameManagerScr;
    [SerializeField] Text descriptionText, pauseCountdownText;
    AudioSource mySound;
    [SerializeField] GameObject paymentScreen, pauseMenu, settingsMenu;
    [SerializeField] Slider volumeSlider, SFXVolumeSlider;

    AudioSource musicController;

    void Start () {
        gameManagerScr = GameObject.Find("_GameManager").GetComponent<S_GameManager>();
        //sets the text on the start of game UI pop-up if necessary
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0: break;
            case 1: break;
            case 2: descriptionText.text = ""; break;
            case 3: descriptionText.text = ""; break;
            case 4: descriptionText.text = ""; break;
            case 5: descriptionText.text = "Watch out for moving objects!"; break;
            case 6: descriptionText.text = ""; break;
            case 7: descriptionText.text = ""; break;
            case 8: descriptionText.text = ""; break;
            default: break;
        }
        mySound = GetComponent<AudioSource>();
        musicController = GameObject.FindGameObjectWithTag("MusicController").GetComponent<AudioSource>();
        
        //set the sliders in the settings menu to represent current settings
        volumeSlider.value = gameManagerScr.volume; 
        SFXVolumeSlider.value = gameManagerScr.SFXVolume;
    }

    //Function called when a single shield is purchased with coins
    public void OnShopShieldClick()
    {
        if (gameManagerScr.coins >= 5) //if the player has enough coins for item
        {
            //buy shield
            gameManagerScr.coins -= 5; //reduce coin count by cost of item
            gameManagerScr.shieldCount++; //increase shield count
            gameManagerScr.UpdateScores(); //update UI stats

            //play positive feedback sound of purchase
            mySound.PlayOneShot(gameManagerScr.buySound, gameManagerScr.SFXVolume);
        }
        else //if player doesn't have enough coins for item
        {
            Debug.Log("Not Enough Coins!");
            //play negative feedback sound
            mySound.PlayOneShot(gameManagerScr.failedLevelSound, gameManagerScr.SFXVolume);
        }
    }

    //Function called when a single time-warp is purchased with coins
    public void OnShopTimeWarpClick()
    {
        if (gameManagerScr.coins >= 10) //if the player has enough coins for item
        {
            //buy time warp
            gameManagerScr.coins -= 10; //reduce coin count by cost of item
            gameManagerScr.timeWarpCount++; //increase time-warp count
            gameManagerScr.UpdateScores(); //update UI stats

            //play positive feedback sound of purchase
            mySound.PlayOneShot(gameManagerScr.buySound, gameManagerScr.SFXVolume);
        }
        else //if player doesn't have enough coins for item
        {
            Debug.Log("Not Enough Coins!");
            //play negative feedback sound
            mySound.PlayOneShot(gameManagerScr.failedLevelSound, gameManagerScr.SFXVolume);
        }
    }

    //Function called when 10 shields are purchased with coins
    public void OnShop10ShieldClick()
    {
        if (gameManagerScr.coins >= 48) //if the player has enough coins for items
        {
            //buy 10 shields
            gameManagerScr.coins -= 48; //reduce coin count by cost of item
            gameManagerScr.shieldCount+= 10; //increase shield count by 10
            gameManagerScr.UpdateScores(); //update UI stats

            //play positive feedback sound of purchase
            mySound.PlayOneShot(gameManagerScr.buySound, gameManagerScr.SFXVolume);
        }
        else //if player doesn't have enough coins for item
        {
            Debug.Log("Not Enough Coins!");
            //play negative feedback sound
            mySound.PlayOneShot(gameManagerScr.failedLevelSound, gameManagerScr.SFXVolume);
        }
    }

    //Function called when 10 time-warps are purchased with coins
    public void OnShop10TimeWarpClick()
    {
        if (gameManagerScr.coins >= 95) //if the player has enough coins for items
        {
            //buy 10 time warps
            gameManagerScr.coins -= 95; //reduce coin count by cost of item
            gameManagerScr.timeWarpCount+= 10; //increase time-warp count by 10
            gameManagerScr.UpdateScores(); //update UI stats

            //play positive feedback sound of purchase
            mySound.PlayOneShot(gameManagerScr.buySound, gameManagerScr.SFXVolume);
        }
        else //if player doesn't have enough coins for item
        {
            Debug.Log("Not Enough Coins!");
            //play negative feedback sound
            mySound.PlayOneShot(gameManagerScr.failedLevelSound, gameManagerScr.SFXVolume);
        }
    }

    //Function called when free coins for watching ad is clicked
    public void OnShopADClick()
    {
        //play positive feedback sound
        mySound.PlayOneShot(gameManagerScr.buySound, gameManagerScr.SFXVolume);

        gameManagerScr.ShowRewardedVideo(); //play a rewarded video from Unity Ads
        gameManagerScr.bIsAdPlaying = true; //lock to stop players interacting with other UI buttons if ad is slow to load
    }

    //function called when a money item is purchased
    public void OnShopPaymentConfirmation()
    {
        paymentScreen.SetActive(true); //make confirmation screen visible
        //play feedback sound
        mySound.PlayOneShot(gameManagerScr.buySound, gameManagerScr.SFXVolume);
    }

    //function called when money item purchase is cancelled
    public void OnShopConfirmationCancel()
    {
        paymentScreen.SetActive(false); //hide confirmation screen
        //play feedback sound
        mySound.PlayOneShot(gameManagerScr.buySound, gameManagerScr.SFXVolume);
    }

    //Function called when game is paused
    public void OnPauseClicked()
    {
        pauseMenu.SetActive(true); //show the pause menu UI
        Time.timeScale = 0.0f; //pause the game
        mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume); //play feedback sound
    }

    //Function called when resume button is clicked on pause menu
    public void OnResumeClicked()
    {
        pauseMenu.SetActive(false); //hide pause menu UI
        mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume); //play feedback sound
        if (gameManagerScr.bGameInProgress) //if game was in progress when paused
        {
            gameManagerScr.bGameInProgress = false; //set gameplay to not in progress
            Time.timeScale = 1.0f; //reser timescale
            StartCoroutine(Countdown(3)); //start countdown timer to resume game
            pauseCountdownText.gameObject.SetActive(true); //display countdown text
        }
        else //if game wasnt in progress when paused, reset timescale
        {
            Time.timeScale = 1.0f;
        }
    }

    //Co0routine called when countdown timer to resume game is started
    IEnumerator Countdown(int time)
    {
        for (int i = time; i > 0; i--) //co-routine runs whilst timer hasn't finished
        {
            pauseCountdownText.text = time.ToString(); //display time text
            time--; //decrease time variable
            yield return new WaitForSeconds(1.0f); //delay function by a second
        }
        if (time <= 0) //if timer has finished
        {
            yield return null;
            gameManagerScr.bGameInProgress = true; //set the game to be back in progress (unpause)
            pauseCountdownText.gameObject.SetActive(false); //hide countdown text
        }
    }

    //Function called when settings is clicked in pause menu
    public void OnSettingsClicked()
    {
        settingsMenu.SetActive(true); //show settings menu UI
        //play feedback sound
        mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        musicController = GameObject.FindGameObjectWithTag("MusicController").GetComponent<AudioSource>(); //get reference to singleton music controller
    }

    //Function called when music volume slider is changed
    public void OnVolumeChanged()
    {
        gameManagerScr.volume = volumeSlider.value; //stores new volume level in singleton gamemanager
        musicController.volume = volumeSlider.value; //sets the volume of the music on the controller to new value
    }

    //Function called when sound effect volume slider is changed
    public void OnSFXVolumeChanged()
    {
        gameManagerScr.SFXVolume = SFXVolumeSlider.value; //stores new volume level in singleton gamemanager
    }

    //Function called when back button is clicked on settings menu
    public void OnSettingsBackClicked()
    {
        settingsMenu.SetActive(false); //hide settings UI elements
        //play feedback sound
        mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
    }

    //Function called when return to main menu is clicked on pause menu
    public void OnMainMenuClicked()
    {
        //play feedback sound
        mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);

        //reset any necessary variables
        //gameManagerScr.currentLevel = 0; //if active this line stops resume game and forces player to start from beginning every time
        gameManagerScr.bGameInProgress = false;
        gameManagerScr.bTutorialFinished = false;

        SceneManager.LoadScene(0); //load menu scene
    }
}
