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

    void CheckIfReady(int id)
    {
        print(id);

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

            if (playerActive)
            {
                foreach (var i in fishermancontrollerSprite)
                    i.color = Color.green;
            }
            else
            {
                foreach (var i in fishermancontrollerSprite)
                    i.color = Color.red;
            }
        }

        if (fishActive && playerActive)
            StartCoroutine(DelayToStart());
    }

    IEnumerator DelayToStart()
    {
        yield return new WaitForSeconds(0.5f);

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
