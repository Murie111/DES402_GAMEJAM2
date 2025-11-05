using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class checkInput : MonoBehaviour
{
    public UnityEvent readyEvent;
    public Image[] fishcontrollerSprite;
    public Image[] fishermancontrollerSprite;

    bool fishActive;
    bool playerActive;

    public float delay;

    void CheckIfReady(int id)
    {
        if (id == 0)
        {
            fishActive = !fishActive;

            if (fishActive)
            {
                foreach (var i in fishcontrollerSprite)
                    i.color = Color.green;
            }
            else
            {
                foreach (var i in fishcontrollerSprite)
                    i.color = Color.red;
            }
        }

        if (id == 1)
        {
            playerActive = !playerActive;

            if (fishActive)
            {
                foreach (var i in fishcontrollerSprite)
                    i.color = Color.green;
            }
            else
            {
                foreach (var i in fishcontrollerSprite)
                    i.color = Color.red;
            }
        }

        if (fishActive && playerActive)
            StartCoroutine(DelayToStart());
    }

    IEnumerator DelayToStart()
    {
        yield return new WaitForSeconds(delay);

        readyEvent.Invoke();
    }

    //fisher
    public void GetFish(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            CheckIfReady(1);
        }
    }

    public void GetFisher(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            CheckIfReady(0);
        }
    }
}
