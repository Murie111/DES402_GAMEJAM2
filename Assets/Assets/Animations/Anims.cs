using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Anims: MonoBehaviour
{
    Animator anim;

    public string[] Animations;
    [Space(10)]
    public int resetNext;
    public int debugPlayer;
    

    private bool resetbreak;

    //This just a really bog standard animation calling script
    //The names of the animations are stored on the public string tied to the game object
    //Currently they are listed as
    //  0. Idle
    //  1. Cast (Automatically transitioned back into idle)
    //  2. Reeling


    void Start()
    {
        anim = GetComponent<Animator>();
        debugPlayer = -1;
        resetNext= 0;
    }

    private void FixedUpdate()
    {
        if (debugPlayer != -1)
        {
            if (debugPlayer >=0 && debugPlayer < Animations.Length)
            {
                PlayAnim(debugPlayer);
                debugPlayer= -1;
            }
        }

        if (resetNext != 0 && !resetbreak)
        {
            //Allows for loop breaks to occur again via animations.       
            anim.SetBool(String.Format("Next{0}",resetNext), false) ;
                            
            resetbreak= true;            
            StartCoroutine(smallPause());                
        }
    }

    //To call the animations;
    //  -get an instance of this script with get component
    //  -Call PlayAnim(the position in the array) 
    public void PlayAnim(int animNum)
    {
        anim.Play(Animations[animNum]);

        Debug.Log(Animations[animNum]);
    }

    public void UIProgress(int nextStep)
    {
        if(nextStep == 0)
        {
            anim.SetBool("Next1", true);
        }
        else
        {
            anim.SetBool(String.Format("Next{0}", nextStep), true); 
        }
    }

    IEnumerator smallPause()
    {
        yield return new WaitForSeconds(.3f);
        resetbreak= false;
    }
   

}
