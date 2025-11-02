using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class bobberScript : MonoBehaviour
{
    public InputActionAsset InputActions;
    public FishingPlayer fishingScript;
    public Slider catchMeter;
    public float fishingStruggle;
    public int timesPlayerFishHooked;
    public GameObject catchMeterObj;
    public bool catchingDefault = false;
    public bool catchingPlayer = false;
    private InputAction F_MoveAction;
    private Vector2 F_MoveAmt;
    private InputAction F_InteractAction;

    GameObject currentFish;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pond"))
        {
            fishingScript.withinPondBounds = true;
            Debug.Log("within!");
        }
        if (other.CompareTag("FishPlayer"))
        {
            Invoke("hookedPlayer", 1.5f);
            currentFish = null; //should set currentfish to the fish that touched the bobber
        }
        if (other.CompareTag("FishDefault"))
        {
            Invoke("hookedDefault", 1.5f);
            currentFish = null; //should set currentfish to the fish that touched the bobber
        }
    }

    void hookedDefault()
    {
        catchingDefault = true;
        fishingScript.mainScript = false;
    }
    void hookedPlayer()
    {
        catchingPlayer = true;
        fishingScript.mainScript = false;
    }
    private void Start()
    {
        F_InteractAction = InputSystem.actions.FindAction("Interact2");
        F_MoveAction = InputSystem.actions.FindAction("Move2");
        timesPlayerFishHooked = 0;
    }

    private void FixedUpdate()
    {
        if (!fishingScript.mainScript)
        {
            if (catchingDefault)
            {
                catchMeterObj.SetActive(true);
                catchMeter.value -= 0.001f;
                if (F_InteractAction.IsPressed())
                {
                    catchMeter.value += 0.01f;
                }
                
                if (catchMeter.value == 1f)
                {
                    catchingDefault = false;
                    catchMeterObj.SetActive(false);
                    //play catch animation
                    //add points
                    //despawn fish
                    Invoke("caughtDefFish", 1f);
                }

                if (catchMeter.value == 0f)
                {
                    catchingDefault = false;
                    catchMeterObj.SetActive(false);
                    Invoke("failedDefFish", 1f);
                }

            }

            if (catchingPlayer)
            {
                catchMeterObj.SetActive(true);
                calcFishStruggle();
                catchMeter.value -= fishingStruggle;
                if (F_InteractAction.IsPressed())
                {
                    catchMeter.value += 0.01f;
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
                    catchingDefault = false;
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
            fishingStruggle = 0.085f;
        }
        if (timesPlayerFishHooked == 1)
        {
            fishingStruggle = 0.0075f;
        }
        if (timesPlayerFishHooked == 2)
        {
            fishingStruggle = 0.005f;
        }
        if (timesPlayerFishHooked <= 3)
        {
            fishingStruggle = 0.0025f;
        }
    }
    void caughtDefFish()
    {
        catchMeter.value = 0.5f; 
        fishingScript.mainScript = true;
        fishingScript.ResetCast();
    }
    void failedDefFish()
    {
        catchMeter.value = 0.5f;
        fishingScript.mainScript = true;
        fishingScript.ResetCast();
    }

    void caughtPlayFish()
    {
        catchMeter.value = 0.5f;
        fishingScript.mainScript = true;
        fishingScript.ResetCast();
        //winner!
    }

    void failedPlayFish()
    {
        catchMeter.value = 0.5f;
        fishingScript.mainScript = true;
        fishingScript.ResetCast();
        timesPlayerFishHooked += 1;
    }
}
