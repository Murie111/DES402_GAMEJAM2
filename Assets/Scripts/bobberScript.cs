using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class bobberScript : MonoBehaviour
{
    public InputActionAsset InputActions;
    public GlobalManager gameManager;
    public FishingPlayer fishingScript;
    public Slider catchMeter;
    public float fishingStruggle;
    public int timesPlayerFishHooked;
    public GameObject catchMeterObj;
    public bool catchingDefault = false;
    public bool catchingPlayer = false;
    private InputAction F_MoveAction;
    private Vector2 F_MoveAmt;
    public bool mainBobber;
    bool loopCheck = false;
    bool canPress = true;
    private InputAction F_InteractAction;
    [SerializeField] private AudioClip[] splashSoundClip;
    private AudioSource audioSource;
    

    GameObject currentFish;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pond") && mainBobber)
        {
            fishingScript.withinPondBounds = true;
            Debug.Log("within!");
            //play audio 

            //audioSource.clip = spashSoundClip;
            //audioSource.Play();
        }


        if (other.CompareTag("FishPlayer") && !mainBobber && !loopCheck)
        {
            loopCheck = true;
            fishingScript.mainScript = false;
            Invoke("hookedPlayer", 0.5f);
            currentFish = other.gameObject; //should set currentfish to the fish that touched the bobber
        }
        if (other.CompareTag("FishDefault") && !mainBobber && !loopCheck)
        {
            loopCheck = true;
            fishingScript.mainScript = false;
            Invoke("hookedDefault", 0.5f);
            currentFish = other.gameObject; //should set currentfish to the fish that touched the bobber
            audioSource.PlayOneShot(splashSoundClip[0]);
        }
    }

    void hookedDefault()
    {
        catchingDefault = true;
    }
    void hookedPlayer()
    {
        catchingPlayer = true;
    }
    private void Start()
    {
        F_InteractAction = InputSystem.actions.FindAction("Interact2");
        F_MoveAction = InputSystem.actions.FindAction("Move2");
        timesPlayerFishHooked = 0;
        
        audioSource = GetComponent<AudioSource>();
    }



    public void Mashing(InputAction.CallbackContext context)
    {
        if (context.performed)
        {

        }
    }

    private void FixedUpdate()
    {
        if (!fishingScript.mainScript && !mainBobber)
        {
            if (catchingDefault && !catchingPlayer)
            {
                catchMeterObj.SetActive(true);
                catchMeter.value -= 0.005f;
                if (F_InteractAction.IsPressed())
                {
                    if (canPress)
                    {
                        catchMeter.value += 0.1f;
                        canPress = false;
                    }
                }
                else
                {
                    Debug.Log("buttonReleased");
                    canPress = true;
                }
                
                if (catchMeter.value == 1f)
                {
                    catchingDefault = false;
                    catchMeterObj.SetActive(false);
                    //play catch animation
                    //add points
                    currentFish.SetActive(false);
                    Invoke("caughtDefFish", 1f);
                }

                if (catchMeter.value == 0f)
                {
                    catchingDefault = false;
                    catchMeterObj.SetActive(false);
                    Invoke("failedDefFish", 1f);
                }

            }

            if (catchingPlayer && !catchingDefault)
            {
                catchMeterObj.SetActive(true);
                calcFishStruggle();
                Debug.Log(fishingStruggle);
                catchMeter.value -= fishingStruggle;
                if (F_InteractAction.IsPressed())
                {
                    if (canPress)
                    {
                        catchMeter.value += 0.1f;
                        canPress = false;
                    }
                }
                else
                {
                    Debug.Log("buttonReleased");
                    canPress = true;
                }

                if (catchMeter.value == 1f)
                {
                    catchingPlayer = false;
                    catchMeterObj.SetActive(false);
                    //play catch animation
                    //add points
                    //despawn fish
                    Invoke("caughtPlayFish", 1f);
                }

                if (catchMeter.value == 0f)
                {
                    catchingPlayer = false;
                    catchMeterObj.SetActive(false);
                    Invoke("failedPlayFish", 1f);
                }
            }
        }
    }

    void calcFishStruggle()
    {
        if (timesPlayerFishHooked == 0)
        {
            fishingStruggle = 0.0125f;
        }
        if (timesPlayerFishHooked == 1)
        {
            fishingStruggle = 0.01125f;
        }
        if (timesPlayerFishHooked == 2)
        {
            fishingStruggle = 0.01f;
        }
        if (timesPlayerFishHooked >= 3)
        {
            fishingStruggle = 0.0075f;
        }
    }
    void caughtDefFish()
    {
        loopCheck = false;
        canPress = true;
        catchMeter.value = 0.5f; 
        fishingScript.mainScript = true;
        gameManager.increaseFishermanPoints();
        fishingScript.ResetCast();
        Debug.Log("method test");
        audioSource.loop = false;
        audioSource.Stop();
    }
    void failedDefFish()
    {
        loopCheck = false;
        canPress = true;
        catchMeter.value = 0.5f;
        fishingScript.mainScript = true;
        fishingScript.ResetCast();
    }

    void caughtPlayFish()
    {
        loopCheck = false;
        canPress = true;
        catchMeter.value = 0.5f;
        fishingScript.mainScript = true;
        fishingScript.ResetCast();
        //winner!
        //yipee
    }

    void failedPlayFish()
    {
        loopCheck = false;
        canPress = true;
        catchMeter.value = 0.5f;
        fishingScript.mainScript = true;
        fishingScript.ResetCast();
        timesPlayerFishHooked += 1;
    }
}
