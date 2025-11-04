using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FishingPlayer : MonoBehaviour
{
    public InputActionAsset InputActions;
    public Slider fishingPower;
    public GameObject fishingPowerObj;
    public bool mainScript;
    bool isIdle;
    bool casting;
    bool fishing;
    public float fillSpeed;
    public GameObject bobber;
    public GameObject target;
    public GameObject bobberBody;
    public Rigidbody bobberRb;
    public bool withinPondBounds;
    bool fishingBarUp;
    private InputAction F_MoveAction;
    private Vector2 F_MoveAmt;
    private InputAction F_InteractAction;
    [SerializeField] private AudioClip[] SoundClip;
    private AudioSource audioSource;



    void Start()
    {
        mainScript = true;
        isIdle = true;
        casting = false;
        fishing = false;
        withinPondBounds = false;
        F_InteractAction = InputSystem.actions.FindAction("Interact2");
        F_MoveAction = InputSystem.actions.FindAction("Move2");

        audioSource = GetComponent<AudioSource>();
    }


    void FixedUpdate()
    {
        if (mainScript)
        {
            F_MoveAmt = F_MoveAction.ReadValue<Vector2>();

            if (fishingPower.value <= 0)
            {
                fishingBarUp = true;
            }
            if (fishingPower.value >= 1)
            {
                fishingBarUp = false;
            }


            if (!fishing)
            {
                bobber.transform.Rotate(0f, 50 * (F_MoveAmt.x * Time.deltaTime), 0f);
            }

            if (isIdle = F_InteractAction.IsPressed() && !fishing)
            {
                fishingPowerObj.SetActive(true);    
                casting = true;
                if (fishingBarUp)
                {
                    fishingPower.value = fishingPower.value + (fillSpeed * Time.deltaTime);
                    target.transform.position += (target.transform.forward * Time.deltaTime * 10);
                }
                else
                {
                    fishingPower.value = fishingPower.value - (fillSpeed * Time.deltaTime);
                    target.transform.position += -(target.transform.forward * Time.deltaTime * 10);
                }
            }

            if (F_InteractAction.IsPressed() && fishing && !casting)
            {
                ResetCast();
            }

            if (casting && !F_InteractAction.IsPressed())
            {
                //casting anim here
                fishing = true;
                casting = false;
                audioSource.PlayOneShot(SoundClip[1]);
                isIdle = false;
                target.SetActive(false);
                bobber.transform.position += (bobber.transform.forward * (fishingPower.value * 9.5f));
                Invoke("bobberCast", 1.5f);
            }
        }
    }



    void bobberCast()
    {
        if (withinPondBounds)
        {
            bobberBody.SetActive(true);
            fishingPowerObj.SetActive(false);

            audioSource.PlayOneShot(SoundClip[0]);
        }
        else 
        { 
            //this would be the failed cast anim, but itll play the return to precast soooo idk
            ResetCast();
        }
    }

    void inputDelay()
    {
        fishing = false;
    }
    public void ResetCast()
    {
        //return to precast animation
        target.SetActive(true);
        fishingPower.value = 0f;
        bobber.transform.localPosition = new Vector3(0f, -0.4f, 0f);
        target.transform.localPosition = new Vector3(0f, 0f, 0f);
        isIdle = true;
        Invoke("inputDelay", 0.25f);
        bobberBody.SetActive(false);
        withinPondBounds = false;
    }

    private void OnEnable()
    {
        InputActions.FindActionMap("Fisher").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Fisher").Disable();
    }
}
