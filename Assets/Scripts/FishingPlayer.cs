using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FishingPlayer : MonoBehaviour
{
    public InputActionAsset InputActions;
    public Slider fishingPower;
    bool isIdle;
    bool casting;
    public float fillSpeed;
    public GameObject bobber;
    public Rigidbody target;

    private InputAction F_MoveAction;
    private Vector2 F_MoveAmt;
    private InputAction F_InteractAction;


    void Start()
    {
        isIdle = true;
        casting = false;
        F_InteractAction = InputSystem.actions.FindAction("Interact2");
        F_MoveAction = InputSystem.actions.FindAction("Move2");
    }


    void Update()
    {

        F_MoveAmt = F_MoveAction.ReadValue<Vector2>();

        if (!casting)
        {
            if (target.position.x > -10 && target.position.x < 10)
            {
                target.MovePosition(target.transform.position + transform.right * F_MoveAmt.x * Time.deltaTime * 25);
            }
            if (target.position.x < -10f)
            {
                float targetX = target.position.x;
                float targetY = target.position.y;
                float targetZ = target.position.z;
                targetX = (targetX + 0.05f);
                target.position = new Vector3(targetX, targetY, targetZ);
            }
            if (target.position.x > 10f)
            {
                float targetX = target.position.x;
                float targetY = target.position.y;
                float targetZ = target.position.z;
                targetX = (targetX - 0.05f);
                target.position = new Vector3(targetX, targetY, targetZ);
            }
        }

        if (isIdle = F_InteractAction.IsPressed())
        {
            fishingPower.value = fishingPower.value + (fillSpeed * Time.deltaTime);
            casting = true;
        }

        if (casting && !F_InteractAction.IsPressed())
        {
            isIdle = false;
            //float bobberExp = fishingPower.value;
            //float targetExpX = (MathF.Abs(MathF.Abs(target.position.x)-10));
            //bobberExp = bobberExp * targetExpX;

            //bobber.transform.position = Vector3.Lerp(transform.position, target.position, bobberExp);

            //bobber.transform.position = new Vector3(bobberNewX, bobberY, bobberZ);
            //Invoke("ResetCast", 1f);
        }
    }

    void ResetCast()
    {
        fishingPower.value = 0f;
        casting = false;
        isIdle = true;
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