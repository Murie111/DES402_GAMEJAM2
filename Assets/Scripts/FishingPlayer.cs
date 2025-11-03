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
    public Text timerText;
    public float timer = 0.0f;
    public float startTime;


    void Start()
    {
        mainScript = true;
        isIdle = true;
        casting = false;
        fishing = false;
        withinPondBounds = false;
        F_InteractAction = InputSystem.actions.FindAction("Interact2");
        F_MoveAction = InputSystem.actions.FindAction("Move2");
        startGame();
    }

    void startGame()
    {
        timer = startTime;
    }

    private void Update()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            timerText.text = timer.ToString("0");
        }
        else
        {
            Debug.Log("Game Over!");
        }
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
                fishing = true;
                casting = false;
                isIdle = false;
                target.SetActive(false);
                bobber.transform.position += (bobber.transform.forward * (fishingPower.value * 9.5f));
                Invoke("bobberCast", 1.5f);
                //float bobberExp = fishingPower.value;
                //float targetExpX = (MathF.Abs(MathF.Abs(target.position.x)-10));
                //bobberExp = bobberExp * targetExpX;

                //bobber.transform.position = Vector3.Lerp(transform.position, target.position, bobberExp);

                //bobber.transform.position = new Vector3(bobberNewX, bobberY, bobberZ);
                //Invoke("ResetCast", 1f);
            }
        }
    }



    void bobberCast()
    {
        if (withinPondBounds)
        {
            bobberBody.SetActive(true);
            fishingPowerObj.SetActive(false);
        }
        else 
        { 
            ResetCast();
        }
    }

    void inputDelay()
    {
        fishing = false;
    }
    public void ResetCast()
    {
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
//float playerZ = transform.position.z;
//float bobberBuffer = fishingPower.value;
//float bobberAngle = MathF.Atan2((target.position.z - transform.position.z), target.position.x);
//Debug.Log(bobberAngle);
//float bobberZ = playerZ + (9 * bobberBuffer);
//float bobberNewX = bobberZ * MathF.Cos(bobberAngle);
// float bobberNewZ = bobberZ * MathF.Cos(bobberAngle);
//float bobberNewX = MathF.Cos(bobberAngle);
//float bobberNewY = MathF.Sin(bobberAngle);
//bobberNewX = bobberNewX * bobberZ;
//bobberNewY = bobberNewY * bobberZ;
//bobberNewX = bobberNewX / 20;
//Debug.Log(bobberNewX);
//float bobberX = target.position.x;
//float bobberY = bobber.transform.position.y;
//bobber.transform.rotation = Quaternion.Euler(0f, bobberAngle, 0f);