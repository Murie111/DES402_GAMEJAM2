using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class FishingPlayer : MonoBehaviour
{
    public Slider fishingPower;
    public GameObject fishingPowerObj;
    public bool mainScript;
    bool isIdle;
    bool casting;
    bool fishing;
    public float fillSpeed;
    public GameObject bobber;
    public GameObject bobberDuplicate;
    public GameObject target;
    public GameObject bobberBody;
    private SpriteRenderer bobberSprite; 

    public Rigidbody bobberRb;
    public bool withinPondBounds;
    bool fishingBarUp;
    private Vector2 F_MoveAmt;
    private CallbackContext F_InteractAction;

    private PlayerInput input;
    public int inputID = -1;

    [Space(10)]
    [SerializeField] private AudioClip[] SoundClip;
    private AudioSource audioSource;
    //Array of the various animators called to throughout. Should be fisherman, pop up, then splash text.
    [SerializeField] private Anims[] spr_animators;
    [SerializeField] private SpriteRenderer AimArrow;

    public void Move(CallbackContext context)
    {
        F_MoveAmt = context.ReadValue<Vector2>();
    }

    public void Interact(CallbackContext context)
    {
        F_InteractAction = context;
    }

    void Start()
    {
        bobberDuplicate.SetActive(false);
        bobberSprite = bobberBody.GetComponent<SpriteRenderer>();
        bobberSprite.enabled= false;

        mainScript = true;
        isIdle = true;
        casting = false;
        fishing = false;
        withinPondBounds = false;

        audioSource = GetComponent<AudioSource>();
        input = GetComponent<PlayerInput>();
    }


    void FixedUpdate()
    {
        if (mainScript)
        {
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

            if (isIdle = F_InteractAction.phase == InputActionPhase.Performed && !fishing)
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

            if (F_InteractAction.phase == InputActionPhase.Started && fishing && !casting)
            {
                ResetCast();
            }

            if (casting && F_InteractAction.phase != InputActionPhase.Performed)
            {
                //casting anim
                spr_animators[0].PlayAnim(1);
                AimArrow.enabled = false;
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
            bobberSprite.enabled= false;
            Invoke("bobberToggle", .5f);
            spr_animators[3].PlayAnim(0);
            bobberDuplicate.SetActive(true);
            fishingPowerObj.SetActive(false);

            audioSource.PlayOneShot(SoundClip[0]);
        }
        else 
        { 
            //this would be the failed cast anim, but itll play the return to precast soooo idk
            ResetCast();
        }
    }
    void bobberToggle()
    {
        bobberSprite.enabled = true;
    }
    void inputDelay()
    {
        fishing = false;
    }
    public void ResetCast()
    {
        //return to precast animation
        spr_animators[0].PlayAnim(0);
        AimArrow.enabled = true;
        target.SetActive(true);
        fishingPower.value = 0f;

        bobber.transform.localPosition = new Vector3(0f, -0.4f, 0f);
        target.transform.localPosition = new Vector3(0f, -0.32f, 0f);

        isIdle = true;
        Invoke("inputDelay", 0.25f);

        bobberBody.SetActive(false);
        bobberDuplicate.SetActive(false);
        
        withinPondBounds = false;
    }
}
