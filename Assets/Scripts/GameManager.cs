using UnityEngine;

public class GameManager : MonoBehaviour
{

    public int FishPoints;  
    public float FishermanPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
    }
}
