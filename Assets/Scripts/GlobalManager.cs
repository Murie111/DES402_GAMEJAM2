using UnityEngine;
using UnityEngine.UI;
public class GlobalManager : MonoBehaviour
{

    public Text timerTextFish;
    public Text timerTextFisherman;
    public Text fishPointsText;
    public Text fishermanPointsText;
    public Text fishPointsTextOpp;
    public Text fishermanPointsTextOpp;
    public float timer = 0.0f;
    public float startTime;

    public int fishPointsIncrease;
    public int fishermanPointsIncrease;

    public int fishPoints;
    public int fishermanPoints;

    private void Start()
    {
        timer = startTime;
        Debug.Log("displays connected: " + Display.displays.Length);
        //if (Display.displays.Length > 1)
        //    Display.displays[1].Activate();
        //else
        //    Debug.Log("Miss");
    }
    void Update()
    {
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
            timerTextFish.text = timer.ToString("0");
            timerTextFisherman.text = timer.ToString("0");
        }
        else
        {
            Debug.Log("Game Over!");
        }
    }

    public void increaseFishPoints()
    {
        fishPoints += fishermanPointsIncrease;
        fishPointsText.text = ("" + fishPoints);
        fishPointsTextOpp.text = ("" + fishPoints);
    }

    public void increaseFishermanPoints()
    {
        fishermanPoints += fishermanPointsIncrease;
        fishermanPointsText.text = ("" + fishermanPoints);
        fishermanPointsTextOpp.text = ("" + fishermanPoints);
    }
}
