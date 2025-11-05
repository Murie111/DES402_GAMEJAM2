using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this);

        Debug.Log("displays connected: " + Display.displays.Length);
            
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            Display.displays[1].SetParams(1024,512, 2, 1);
            Screen.SetResolution(1024, 512, true);
            Application.targetFrameRate = 60;
        }
    }
}
