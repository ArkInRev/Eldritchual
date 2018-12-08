using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{


    public enum GameState
    {
        InMenus,
        InTutorial,
        InProgress,
        LosingScreen,
        WinningScreen
    }

    public GameState thisGameState;
    private int totalCultists;

    [Header("Ritual Values")]
    public float ritualNeeded;
    public float ritualEarned;
    public float ritualProgress;
    public Slider ritualSlider;
    public float ritualLostEachDecay;
    public float sacrificeValue;
    public float sacrificeObscurityCost;



    [Header("Obscurity Values")]
    public float obscurityMax;
    public float obscurityLevel;
    public float obscurityNormalized;
    public Slider obscuritySlider;
    public float obscurityFromWorship;
    public float obscurityFromRecruiting;
    public float obscurityFromRelaxing;
    public float obscurityFromStudying;
    [Header("Faith Values")]
    public float faithMax;
    public float faithLevel;
    public float faithNormalized;
    public Slider faithSlider;
    public float faithFromWorship;
    public float faithFromRecruiting;
    public float faithFromRelaxing;
    public float faithFromStudying;

    [Header("Production Progress")]
    public float[] happinessMultipliers;

    [Header("Recruitment Values")]
    public float recruitMax;
    public float recruitLevel;
    public float recruitNormalized;
    public Slider recruitmentSlider;
    public float recruitmentFromRecruiting;
    public float recruitmentFromWorshiping;
    public float recruitmentFromStudying;
    public float recruitmentFromRelaxing;
    public GameObject newRecruit;
    public Transform spawnPoint;


    [Header("Global Decay Values")]
    public float secondsBetweenDecay;
    private float lastDecay;

    [Header("UI Elements")]
    public Canvas mainMenuCanvas;
    public Canvas tutorialMenuCanvas;
    public Canvas winScreenCanvas;
    public Canvas loseScreenCanvas;
    public Canvas gameUICanvas;
    public TMP_Text GameOverReasonMessageTextbox;


    [Header("UI Elements")]
    public string gameOverTextNoCultists;
    public string gameOverTextNoFaith;
    public string gameOverTextNoObscurity;

    private void Awake()
    {
        mainMenuCanvas.enabled = true;
        gameUICanvas.enabled = false;
        winScreenCanvas.enabled = false;
        loseScreenCanvas.enabled = false;
        tutorialMenuCanvas.enabled = false;
    }

    // Use this for initialization
    void Start()
    {
        thisGameState = GameState.InMenus;
        lastDecay = secondsBetweenDecay;
        recruitLevel = recruitMax;
        spawnNewRecruit();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Game controller Update");
        if (thisGameState == GameState.InProgress)
        {
            totalCultists = this.transform.childCount;

            ritualProgress = ritualEarned / ritualNeeded;
            ritualSlider.value = ritualProgress;

            obscurityNormalized = obscurityLevel / obscurityMax;
            obscuritySlider.value = obscurityNormalized;

            faithNormalized = faithLevel / faithMax;
            faithSlider.value = faithNormalized;

            recruitNormalized = recruitLevel / recruitMax;
            recruitmentSlider.value = recruitNormalized;

            if (recruitLevel >= recruitMax) spawnNewRecruit();

            if (recruitLevel <= 0) recruitLevel = 0; // don't go below 0 when recruiting. 

            // cap the bar values
            if (ritualEarned > ritualNeeded) ritualEarned = ritualNeeded;
            if (obscurityLevel > obscurityMax) obscurityLevel = obscurityMax;
            if (faithLevel > faithMax) faithLevel = faithMax;


            if (faithLevel <= 0) //ran out of faith, game over
            {
                //temp for testing
                faithLevel = 0;
                GameOverReasonMessageTextbox.text = gameOverTextNoFaith;
                thisGameState = GameState.LosingScreen;
                //Debug.Log("Lost by running out of Faith!");
            }

            if (obscurityLevel <= 0) //ran out of obscurity, game over
            {
                //temp for testing
                obscurityLevel = 0;
                GameOverReasonMessageTextbox.text = gameOverTextNoObscurity;
                thisGameState = GameState.LosingScreen;
                //Debug.Log("Lost by running out of Obscurity!");


            }

            //check for cultist count
            //Debug.Log("GameObject child Count: " + this.transform.childCount.ToString());
            if (totalCultists <= 0) // lost by running out of cultists
            {
                //Debug.Log("Lost by running out of Cultists!");
                GameOverReasonMessageTextbox.text = gameOverTextNoCultists;
                thisGameState = GameState.LosingScreen;
            }

            // Need to Win
            if(ritualEarned >= ritualNeeded)
            {
                // the game is over and you win

                // do something cool with a summon animation? (time permitting)

                // change the game condition to winning
                thisGameState = GameState.WinningScreen;
            }

            lastDecay -= Time.deltaTime;
            if (lastDecay <= 0)
            {
                ritualDecay();
            }


            if (Input.GetKeyDown(KeyCode.Escape))
            {
                thisGameState = GameState.InMenus;
                mainMenuCanvas.enabled = true;
                gameUICanvas.enabled = false;
                loseScreenCanvas.enabled = false;

            }

        } else if (thisGameState == GameState.LosingScreen)
        {
            mainMenuCanvas.enabled = false;
            gameUICanvas.enabled = false;
            loseScreenCanvas.enabled = true;
            tutorialMenuCanvas.enabled = false;
            winScreenCanvas.enabled = false;
        }
        else if (thisGameState == GameState.WinningScreen)
        {
            mainMenuCanvas.enabled = false;
            gameUICanvas.enabled = false;
            loseScreenCanvas.enabled = false;
            tutorialMenuCanvas.enabled = false;
            winScreenCanvas.enabled = true;
        }



    }

    public void gameProduceRequest(string itemProduced, int happinessInt)
    {
        Debug.Log("GC request to produce " + itemProduced + " with happiness " + happinessInt.ToString());
        switch (itemProduced)
        {
            case ("Relaxing"):
                faithLevel += faithFromRelaxing * happinessMultipliers[happinessInt];
                recruitLevel += recruitmentFromRelaxing * happinessMultipliers[happinessInt];
                obscurityLevel += obscurityFromRelaxing * happinessMultipliers[happinessInt];
                break;
            case ("Worshiping"):
                faithLevel += faithFromWorship * happinessMultipliers[happinessInt];
                recruitLevel += recruitmentFromWorshiping * happinessMultipliers[happinessInt];
                obscurityLevel += obscurityFromWorship * happinessMultipliers[happinessInt];
                break;
            case ("Recruiting"):
                faithLevel += faithFromRecruiting * happinessMultipliers[happinessInt];
                recruitLevel += recruitmentFromRecruiting * happinessMultipliers[happinessInt];
                obscurityLevel += obscurityFromRecruiting * happinessMultipliers[happinessInt];
                break;
            case ("Studying"):
                faithLevel += faithFromStudying * happinessMultipliers[happinessInt];
                recruitLevel += recruitmentFromStudying * happinessMultipliers[happinessInt];
                obscurityLevel += obscurityFromRelaxing * happinessMultipliers[happinessInt];
                break;

        }
    }

    public void ritualDecay()
    {
        ritualEarned -= ritualLostEachDecay * (1 - faithNormalized);
        if (ritualEarned < 0) ritualEarned = 0;
        lastDecay = secondsBetweenDecay;
    }

    public void spawnNewRecruit()
    {
        recruitLevel = 0;
        GameObject newCultist = Instantiate(newRecruit, spawnPoint);
        newCultist.transform.SetParent(this.transform, true);

    }

    public void sacrificeOffered(Transform sacrificedAt, int happinessLevel)
    {
        //Debug.Log("Sacrifice offered to game controller at :" + sacrificedAt.ToString() + " with a happiness of " + happinessLevel);
        //spawn particles
        //add ritual amount to the meter
        ritualEarned += sacrificeValue * happinessMultipliers[happinessLevel];

        // Downsides
        // reduce all cultists happiness, less happy reduces happiness more than happy
        reduceAllCultistHappiness();
        // take a hit to obscurity
        obscurityLevel -= sacrificeObscurityCost * (1 - faithNormalized);
    }

    public void StartGame()
    {
        mainMenuCanvas.enabled = false;
        gameUICanvas.enabled = true;
        tutorialMenuCanvas.enabled = false;
        loseScreenCanvas.enabled = false;
        winScreenCanvas.enabled = false;
        thisGameState = GameState.InProgress;

    }

    public void StartTutorial()
    {
        tutorialMenuCanvas.enabled = true;
        mainMenuCanvas.enabled = false;
        gameUICanvas.enabled = false;
        loseScreenCanvas.enabled = false;
        winScreenCanvas.enabled = false;
        thisGameState = GameState.InTutorial;
    }

    public void startGameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void reduceAllCultistHappiness()
    {
        Cultist[] allChildren = this.gameObject.GetComponentsInChildren<Cultist>();
        foreach (Cultist child in allChildren)
        {
            //
            child.loseHappiness(faithNormalized);
        }
    }
}
