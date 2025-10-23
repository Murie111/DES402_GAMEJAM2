using UnityEngine;
using UnityEngine.UI;

public class FishingPlayer : MonoBehaviour
{
    public Slider fishingPower;
    bool isIdle;
    public float fillSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isIdle = true;
    }


    void Update()
    {
        if (isIdle && Input.GetKeyDown(KeyCode.Z))
        {
            fishingPower.value = fishingPower.value * fillSpeed * Time.deltaTime;
        }
    }
}
