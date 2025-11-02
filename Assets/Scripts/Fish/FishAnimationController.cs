using UnityEngine;

public class FishAnimationController : MonoBehaviour
{
    public AI_Brain brain;
    public Animator anim;

    void Update()
    {
        anim.SetBool("Moving", brain.IsMoving());
    }
}
