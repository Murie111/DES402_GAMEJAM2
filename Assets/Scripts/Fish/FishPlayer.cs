using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class FishPlayer : MonoBehaviour
{

    public bool isBeingReeled;
    public bool startingGame;
    public bool isBattling;
    public GameObject catchMeterObj;
    public Slider catchMeter;
    public Slider fisherCatchMeter;
    float fishingIncrease;
    int timesPlayerFishHooked = 0;
    public bobberScript bobberScript;
    public GlobalManager globalManager;

    private Vector2 F_MoveAmt;
    private Rigidbody F_Rigidbody;

    public float SwimSpeed = 5;
    public float RotateSpeed = 5;
    public float Speed;

    private bool eating = false;

    public GameObject Mouth;

    public Animator anim;
    public Animator trackedAnim;

    private PlayerInput input;
    [SerializeField] Anims spr_splash;

    public void Move(CallbackContext context)
    {
        //print("aa " + input.playerIndex);

        // if (input.playerIndex != 1)
        //     return;

        F_MoveAmt = context.ReadValue<Vector2>();
    }

    public void Fish(CallbackContext context)
    {
        if (input.playerIndex != 1)
            return;

        if (context.phase == InputActionPhase.Started)
        {
            if (!eating && !isBeingReeled)
            {
                StartCoroutine(Eat());
            }
            if (isBeingReeled) 
            {
                increaseBar();
            }
        }
    }

    private void Awake()
    {
        F_Rigidbody = GetComponent<Rigidbody>();
        input = GetComponent<PlayerInput>();
    }

    IEnumerator Eat()
    {
        Mouth.SetActive(true);
        eating = true;
        yield return new WaitForSeconds(1);
        Mouth.SetActive(false);
        eating = false;
    }
    
    void increaseBar()
    {
        calcFishStruggle();
        catchMeter.value += fishingIncrease;
        bobberScript.decreaseBar(fishingIncrease);
    }

    public void decreaseBar()
    {
        catchMeter.value -= 0.05f;
    }

    public void FishPlayerBite()
    {
        Invoke("startBattle",2.5f);
    }

    public void startBattle()
    {
        isBattling = true;
    }

    void calcFishStruggle()
    {
        if (timesPlayerFishHooked == 0)
        {
            fishingIncrease = 0.075f;
        }
        if (timesPlayerFishHooked == 1)
        {
            fishingIncrease = 0.05f;
        }
        if (timesPlayerFishHooked >= 2)
        {
            fishingIncrease = 0.035f;
        }

    }


    private void FixedUpdate()
    {
        if (!isBeingReeled && globalManager.gameInProg)
        {
            Swimming();
        }
        if (isBattling)
        {
            catchMeterObj.SetActive(true);
            if (catchMeter.value == 1f)
            {
                //copy here
                spr_splash.UIProgress(2);
                
                isBeingReeled = false;
                isBattling = false;
                catchMeterObj.SetActive(false);
                catchMeter.value = 0.5f;
                bobberScript.fisherLoseBattle();
                timesPlayerFishHooked += 1;
            }

            if (catchMeter.value == 0f)
            {
                //Copy here
                spr_splash.UIProgress(2);
                
                spr_splash.PlayAnim(5);
                isBeingReeled = false;
                isBattling = false;
                catchMeterObj.SetActive(false);
                catchMeter.value = 0.5f;
                //should end game
                Debug.Log("fish caught!");
            }
        }
    }


    public void playSnaggedAnim()
    {
        //Copy here (Change the number to 4?
        spr_splash.PlayAnim(6);
        Invoke("playMashAnim", 2f);
    }
    void playMashAnim()
    {
        spr_splash.PlayAnim(2);
    }
    private void Swimming()
    {
        Speed = Mathf.Max(Mathf.Abs(F_MoveAmt.x), Mathf.Abs(F_MoveAmt.y));

        F_Rigidbody.MovePosition(F_Rigidbody.position + transform.forward * Speed * SwimSpeed * Time.deltaTime);

        Vector3 moveDir = new Vector3(F_MoveAmt.x, 0, F_MoveAmt.y);
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            F_Rigidbody.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, RotateSpeed * Time.deltaTime));
        }

        bool moving = F_MoveAmt != Vector2.zero;

        anim.SetBool("Moving", moving);
        trackedAnim.SetBool("Moving", moving);  //this is so fucking stupid but i dont care its 3 fucking am and i am beyond tierd i just hope to god this works so i can go to sleep please

    }
}
