using UnityEngine;

public class FakeFood : MonoBehaviour
{
    public AudioSource audioSource;
    public bobberScript playerBobber;
    public FishPlayer fishPlayer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mouth"))
        {
            //audioSource.Play();
            Debug.Log("hooked!");
            //logic for fish biting food
            fishPlayer.FishPlayerBite();
            fishPlayer.isBeingReeled = true;
            fishPlayer.playSnaggedAnim();
            playerBobber.FishPlayerBite();
        }
    }

}
