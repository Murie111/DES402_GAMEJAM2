using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class bobberScript : MonoBehaviour
{
    public InputActionAsset InputActions;
    public FishingPlayer fishingScript;
    public Slider catchMeter;
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

            //fishingScript.mainScript = false;
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
    private void Start()
    {
        F_InteractAction = InputSystem.actions.FindAction("Interact2");
        F_MoveAction = InputSystem.actions.FindAction("Move2");
    }

    private void FixedUpdate()
    {
        if (!fishingScript.mainScript)
        {
            if (catchingDefault)
            {
                catchMeterObj.SetActive(true);
                catchMeter.value -= 0.005f;
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
            }
            if (catchingPlayer)
            {

            }
        }
    }
    void caughtDefFish()
    {
        catchMeter.value = 0f; 
        fishingScript.mainScript = true;
        fishingScript.ResetCast();
    }
}
