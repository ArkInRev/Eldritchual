using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CapsuleCollider2D))]

public class Cultist : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private Vector3 screenPoint;
    private Vector3 offset;
    public Transform whereAmICheck;
    public LayerMask whatAreAreas;
    public Transform particleEmitPoint;

    private GameObject gameController;
    private GameController gc;

    public float productionCoooldown;           // how many seconds in between producing something
    private float timeSinceLastProduction;      // how many seconds since last production

    [SerializeField]
    private State currentState;
    public Animator animator;

    [Header("Happiness")]
    [SerializeField]
    private Happiness happinessLevel;
    [SerializeField]
    private float[] happinessMultipliers; //from worried 0 to zealous 3 in the list
    public float happinessMax;
    public float happinessNow;
    private int happinessValue;
    public float happinessUnsure;
    public float happinessHappy;
    public float happinessZealous;

    public float happinessStudying;
    public float happinessRelaxing;
    public float happinessWorshiping;
    public float happinessRecruiting;

    public float[] sacrificeHappinessMultiplier;
    public float sacrificeBaseHappinessLoss;

    private enum Happiness
    {
            Worried,
            Unsure,
            Happy,
            Zealous
    }

    public Sprite[] faceSprites;


    private enum State
    {
        Idle,
        PickedUp,
        Worshiping,
        Studying,
        Recruiting,
        Relaxing,
        Dead
    }

    public ParticleSystem[] productionParticleSystems;
    public ParticleSystem deathParticles;

    [Header("Audio Files")]
    public AudioSource audioPlayer;
    public AudioClip audioPickUp;
    public AudioClip audioPutDown;
    public AudioClip audioRecruit;
    public AudioClip audioWorship;
    public AudioClip audioStudy;
    public AudioClip audioRelax;
    public AudioClip audioNewRecruit;
    public AudioClip audioLeaving;


    private void Awake()
    {
        currentState = State.Idle;
        happinessLevel = Happiness.Unsure;
        timeSinceLastProduction = productionCoooldown;
        gameController = GameObject.Find("GameController");
        gc = gameController.GetComponent<GameController>();
    }

    // Use this for initialization
    void Start()
    {
        //putDownCultist();
        audioPlayer.clip = audioNewRecruit;
        audioPlayer.Play();
    }
	
	// Update is called once per frame
	void Update () {
        
        // When the game is in progress, do these things, otherwise, don't do anything.
        if(gc.thisGameState == GameController.GameState.InProgress)
        {
            //temporary face changing
            checkHappinessLevel();
            //changeFace();

            if (timeSinceLastProduction <= 0)
            {
                //Produce Something
                cultistProduction();
            }
            else
            {
                timeSinceLastProduction -= Time.deltaTime;
            }
        }

	}

    public void checkHappinessLevel()
    {

        switch (happinessLevel)
        {
            case (Happiness.Worried):
                happinessValue = 0;
                break;
            case (Happiness.Unsure):
                happinessValue = 1;
                break;
            case (Happiness.Happy):
                happinessValue = 2;
                break;
            case (Happiness.Zealous):
                happinessValue = 3;
                break;
        }
        Happiness targeHappiness;
        if(happinessNow >= happinessUnsure && happinessNow < happinessHappy) // cultist is unsure
        {
            targeHappiness = Happiness.Unsure;
        } else if (happinessNow < happinessUnsure) // cultist is worried
        {
            targeHappiness = Happiness.Worried;
        } else if (happinessNow >= happinessHappy && happinessNow< happinessZealous) // cultist is happy
        {
            targeHappiness = Happiness.Happy;
        } else if (happinessNow >= happinessZealous)    // Cultist is zealous
        {
            targeHappiness = Happiness.Zealous;
        } else
        {
            targeHappiness = Happiness.Unsure;
        }
        // if target happiness isn't the same as current
        if (targeHappiness != happinessLevel)
        {
            happinessLevel = targeHappiness;
            changeFace();
        }

        if (happinessNow < 0)
        {
            // ran out of happiness.
            // happinessNow = 0; //placeholder limiter
            currentState = State.Dead;  //placeholder state

            //placeholder cultist removal
            ParticleSystem leaveParticles = deathParticles;
            leaveParticles.GetComponent<AudioSource>().clip = audioLeaving;
            Instantiate(leaveParticles, particleEmitPoint.transform.position, Quaternion.identity);
            Destroy(this.gameObject);

        }
        if (happinessNow > happinessMax) happinessNow = happinessMax;

    }

    private void cultistProduction()
    {
        // Only produce when the cultist is in a state that can produce
        if(this.currentState != State.Dead && this.currentState != State.PickedUp)
        {
            //Debug.Log(this.name + " just produced " + this.currentState);

            timeSinceLastProduction = productionCoooldown;
            gc.gameProduceRequest(this.currentState.ToString(),happinessValue);
            switch (this.currentState.ToString())
            {
                case ("Studying"):
                    cultistProduceStudying();
                    break;
                case ("Relaxing"):
                    cultistProduceRelaxing();
                    break;
                case ("Recruiting"):
                    cultistProduceRecruiting();
                    break;
                case ("Worshiping"):
                    cultistProduceWorshiping();
                    break;
                case ("Idle"):
                    cultistProduceIdle();
                    break;




            }
        }

    }

    private void cultistProduceStudying()
    {
        happinessNow -= happinessStudying * happinessMultipliers[happinessValue];
        Instantiate(productionParticleSystems[1], particleEmitPoint.transform.position, Quaternion.identity);
        Instantiate(productionParticleSystems[0], particleEmitPoint.transform.position, Quaternion.identity);
        audioPlayer.clip = audioStudy;
        audioPlayer.Play();
    }
    private void cultistProduceRelaxing()
    {
        happinessNow += happinessRelaxing * happinessMultipliers[happinessValue];
        Instantiate(productionParticleSystems[0], particleEmitPoint.transform.position, Quaternion.identity);
        Instantiate(productionParticleSystems[2], particleEmitPoint.transform.position, Quaternion.identity);
        audioPlayer.clip = audioRelax;
        audioPlayer.Play();
    }
    private void cultistProduceRecruiting()
    {
        happinessNow -= happinessRecruiting * happinessMultipliers[happinessValue];
        Instantiate(productionParticleSystems[2], particleEmitPoint.transform.position, Quaternion.identity);
        audioPlayer.clip = audioRecruit;
        audioPlayer.Play();
    }
    private void cultistProduceWorshiping()
    {
        happinessNow += happinessWorshiping * happinessMultipliers[happinessValue];
        Instantiate(productionParticleSystems[1], particleEmitPoint.transform.position, Quaternion.identity);
        audioPlayer.clip = audioWorship;
        audioPlayer.Play();

    }
    private void cultistProduceIdle()
    {
        happinessNow -= happinessRelaxing * happinessMultipliers[3];
        //Instantiate(productionParticleSystems[1], particleEmitPoint.transform.position, Quaternion.identity);

    }

    private void cultistSacrificed()
    {
        gc.sacrificeOffered(this.transform, happinessValue);
        Instantiate(deathParticles, particleEmitPoint.transform.position, Quaternion.identity);
    }

    private void pickUpCultist()
    {
        currentState = State.PickedUp;
        setAllChildSortingLayers("PickedUp");

    }

    public void putDownCultist()
    {
        State previousState = currentState;
        setAllChildSortingLayers("Default");

        //where am i now?
        Collider2D whereAmI = Physics2D.OverlapCircle(whereAmICheck.position, 0.1f, whatAreAreas);
        //Debug.Log("Cultist Dropped Here: " + whereAmI.name);
        if (whereAmI.CompareTag("Reading")){
            currentState = State.Studying;
        } else if (whereAmI.CompareTag("Recruiting"))
        {
            currentState = State.Recruiting;
        } else if (whereAmI.CompareTag("Worship"))
        {
            currentState = State.Worshiping;

        } else if (whereAmI.CompareTag("Relax"))
        {
            currentState = State.Relaxing;
        }
        else if (whereAmI.CompareTag("Sacrifice"))
        {
            currentState = State.Dead;
            cultistSacrificed();
            
            Destroy(this.gameObject);
        }
        else if (whereAmI.CompareTag("Idle"))
        {
            currentState = State.Idle;
        }
        else
        {
            currentState = State.Idle;
        }
              

        if (previousState != currentState)
        {
            chooseAnimation();
        }
        

    }

    private void setAllChildSortingLayers(string layer)
    {
        SpriteRenderer[] allChildren = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer child in allChildren)
        {
            child.sortingLayerName = layer;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
        audioPlayer.clip = audioPickUp;
        audioPlayer.Play();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");

        this.transform.position = eventData.position;
        Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;
        transform.position = currentMousePosition;


        //pickUp the cultist
        pickUpCultist();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");

    }

    public void OnPointerDown(PointerEventData eventData)
    {

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        //Debug.Log("OnPointerDown: ");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");

        //reset the offset when the mouse button is released
        offset = Vector3.zero;
        // Put down the cultist
        putDownCultist();
        audioPlayer.clip = audioPutDown;
        audioPlayer.Play();
    }

    private void chooseAnimation()
    {
        //clear all animation states
        animator.SetBool("Relaxing", false);
        animator.SetBool("Recruiting", false);
        animator.SetBool("Studying", false);
        animator.SetBool("Worshiping", false);
        animator.SetBool("PickedUp", false);

        switch (currentState)
        {
            case State.Relaxing:
                animator.SetBool("Relaxing", true);
                break;
            case State.Recruiting:
                animator.SetBool("Recruiting", true);
                break;
            case State.Studying:
                animator.SetBool("Studying", true);
                break;
            case State.Worshiping:
                animator.SetBool("Worshiping", true);
                break;
            case State.PickedUp:
                animator.SetBool("PickedUp", true);
                break;
            default:
                
                break;
                    
        }
    }

    private void changeFace()
    {
        int faceToUse=0;

        switch (happinessLevel)
        {
            case Happiness.Unsure:
                faceToUse = 1;
                break;
            case Happiness.Worried:
                faceToUse = 0;
                break;
            case Happiness.Happy:
                faceToUse = 2;
                break;
            case Happiness.Zealous:
                faceToUse = 3;
                break;

        }

        Transform cultistFace = this.gameObject.transform.Find("hood/cultistFace");
        cultistFace.GetComponent<SpriteRenderer>().sprite = faceSprites[faceToUse];
    }

    public void loseHappiness(float faithNormalized)
    {
        happinessNow -= sacrificeBaseHappinessLoss * sacrificeHappinessMultiplier[happinessValue] * (1-faithNormalized);
    }
}
