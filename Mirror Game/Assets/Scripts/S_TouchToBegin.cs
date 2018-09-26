using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]

public class S_TouchToBegin : MonoBehaviour {

    S_PlayerMovement playerMovementScr;
    S_GameManager gameManagerScr;

    [SerializeField] GameObject popUpBG, shopImage, scrollImg, leaderboardsImage;
    [SerializeField] RectTransform coinsButtons, moneyButtons;
    [SerializeField] Animator playAnim, popUpAnim;

    AudioSource mySound;

    private void Start()
    {
        playerMovementScr = GameObject.Find("Player").GetComponent<S_PlayerMovement>();
        gameManagerScr = GameObject.Find("_GameManager").GetComponent<S_GameManager>();
        moneyButtons.gameObject.SetActive(false);
        mySound = GetComponent<AudioSource>();
        popUpAnim.SetTrigger("Use"); //trigger pop up UI to slide in from bottom
        InvokeRepeating("PlayButtonAnimation", 2.0f, 4.0f); //start the animation loop for the playbutton animations
    }

    //Function used to loop play button animation
    public void PlayButtonAnimation()
    {
        playAnim.SetTrigger("Use");
    }

    //Function called when play button is clicked
    public void OnButtonClick()
    {
        if (!gameManagerScr.bIsAdPlaying) //if theres no ad playing or loading
        {
            playerMovementScr.ZeroTilt(); //reset the tilt centre to be from current rotation
            gameManagerScr.bGameInProgress = true; //set game to be in progress for other scripts to track
            gameManagerScr.bOnLevelFailedLock = false; //disable the lock used if a level is failed
            gameManagerScr.coinsTemp = 0; //reset temporary coins, used to track coins collected per level
            popUpBG.SetActive(false); //hide the UI element
            //play feedback sound
            if (!mySound.isPlaying)
            {
                mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
            }
        }
    }

    //Function called when leaderboards button is clicked
    public void OnLeaderboardsClicked()
    {
        leaderboardsImage.SetActive(true); //display leaderboards pop up UI
        //give feedback sound
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
        popUpBG.SetActive(false); //hide other UI elements
    }

    //Function called when shop button is clicked at start of level
    public void OnShopClicked()
    {
        shopImage.SetActive(true); //display shop UI elements
        //play feedback sound
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
        popUpBG.SetActive(false); //hide other UI elements
    }

    //Function called when shop or leaderboards back button is clicked
    public void OnShopBackClicked()
    {
        //hides unnecessary UI elements
        shopImage.SetActive(false);
        leaderboardsImage.SetActive(false);
        popUpBG.SetActive(true); //display main start of level UI
        //play feedback sound
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
    }

    //Function called when coins button is clicked within shop
    public void OnCoinsClicked()
    {
        coinsButtons.gameObject.SetActive(true);
        scrollImg.GetComponent<ScrollRect>().content = coinsButtons; //swaps items in scrolling list
        moneyButtons.gameObject.SetActive(false);
        //play feedback sound
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
    }

    //Function called when money button is clicked within shop
    public void OnMoneyClicked()
    {
        moneyButtons.gameObject.SetActive(true);
        scrollImg.GetComponent<ScrollRect>().content = moneyButtons; //swaps items in scrolling list
        coinsButtons.gameObject.SetActive(false);
        //play feedback sound
        if (!mySound.isPlaying)
        {
            mySound.PlayOneShot(gameManagerScr.buttonClickSound, gameManagerScr.SFXVolume);
        }
    }

}
