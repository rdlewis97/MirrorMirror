using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_TutorialManager : MonoBehaviour {

    S_GameManager gameManagerScr;
    [SerializeField]
    int tutorialCount;
    [SerializeField]
    GameObject firstBarrier, shieldBarrier, coin1, coin2; 
    [SerializeField]    
    Text tutorialText;
        
    void Start () {
        gameManagerScr = GameObject.Find("_GameManager").GetComponent<S_GameManager>();
        tutorialCount = gameManagerScr.tutorialCount; //this value stores what stage of the tutorial the player is currently on
        coin1.SetActive(false);
        coin2.SetActive(false);
        //set up the tutorial level based on the current tutorial stage
        switch (tutorialCount)
        {
            case 0:
                //first step of tut: movement
                tutorialText.text = "Tilt to move";
                break;
            case 1:
                //second step of tut: obstacles
                Instantiate(firstBarrier);
                tutorialText.text = "RED MEANS DANGER!";
                break;
            case 2:
                //third step of tut: coins
                Instantiate(firstBarrier);
                coin1.SetActive(true);
                coin2.SetActive(true);
                tutorialText.text = "Coins are your friend!";
                break;
            case 3:
                //fourth step of tut: shop and shield
                coin1.SetActive(true);
                coin2.SetActive(true);
                Instantiate(shieldBarrier);
                tutorialText.text = "Who doesn't love shopping?!";
                break;
        }
    }
}
