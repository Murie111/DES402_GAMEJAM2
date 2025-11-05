using UnityEngine;

public class FakeFood : MonoBehaviour
{
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mouth"))
        {
            audioSource.Play();

            //logic for fish biting food
        }
    }
}
