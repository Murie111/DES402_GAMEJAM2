using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishPlayer : MonoBehaviour
{
    public InputActionAsset InputActions;

    private InputAction F_MoveAction;
    private InputAction F_InteractAction;

    private Vector2 F_MoveAmt;
    private Rigidbody F_Rigidbody;

    public float SwimSpeed = 5;
    public float RotateSpeed = 5;
    public float Speed;

    private bool eating = false;

    public GameObject Mouth;

    public Animator anim;
    public Animator trackedAnim;

    private void OnEnable()
    {
        InputActions.FindActionMap("Fish").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Fish").Disable();
    }

    private void Start()
    {
        Mouth = this.gameObject.transform.GetChild(1).gameObject;
    }
    private void Awake()
    {
        F_MoveAction = InputSystem.actions.FindAction("Move");
        F_InteractAction = InputSystem.actions.FindAction("Interact");

        F_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        F_MoveAmt = F_MoveAction.ReadValue<Vector2>();

        if (F_InteractAction.WasPressedThisFrame())
        {
            if (!eating)
            {
                StartCoroutine(Eat());
            }
        }
    }

    IEnumerator Eat()
    {
        Mouth.SetActive(true);
        eating = true;
        yield return new WaitForSeconds(1);
        Mouth.SetActive(false);
        eating = false;
    }

    private void FixedUpdate()
    {
        Swimming();
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
