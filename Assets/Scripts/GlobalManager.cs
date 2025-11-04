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

    public GameObject fishTutorial;
    public GameObject fishermanTutorial;

    public bool fishReady;
    public bool fishermanReady;

    public GameObject readyText;
    public GameObject text3;
    public GameObject text2;
    public GameObject text1;
    public GameObject goText;

    private void Start()
    {
        timer = startTime;
        //  fishTutorial.SetActive(true);
        //  fishermanTutorial.SetActive(true);

        Debug.Log("displays connected: " + Display.displays.Length);
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            Display.displays[1].SetParams(1024,512, 2, 1);
            Screen.SetResolution(1024, 512, true);
            Application.targetFrameRate = 60;
        }

        else
            Debug.Log("Miss");
    }

    void startCountdown()
    {
        Invoke("readyDisp", 0f);
        Invoke("text3Disp", 1f);
        Invoke("text2Disp", 2f);
        Invoke("text1Disp", 3f);
        Invoke("goDisp", 4f);
    }
    void startGame()
    {
        timer = startTime;
    }
    void Update()
    {
        if (fishReady && fishermanReady)
        {
            //startCountdown();
        }    



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
        fishPoints += fishPointsIncrease;
        fishPointsText.text = ("" + fishPoints);
        fishPointsTextOpp.text = ("" + fishPoints);
    }
    public void increaseFishPointsBonus()
    {
        fishPoints += fishPointsIncrease * 2;
        fishPointsText.text = ("" + fishPoints);
        fishPointsTextOpp.text = ("" + fishPoints);
    }

    public void increaseFishermanPoints()
    {
        fishermanPoints += fishermanPointsIncrease;
        fishermanPointsText.text = ("" + fishermanPoints);
        fishermanPointsTextOpp.text = ("" + fishermanPoints);
    }

    void readyDisp()
    {
        readyText.SetActive(true);
    }
    void text3Disp()
    {
        readyText.SetActive(false);
        text3.SetActive(true);
    }
    void text2Disp()
    {
        text3.SetActive(false);
        text2.SetActive(true);
    }
    void text1Disp()
    {
        text2.SetActive(false);
        text1.SetActive(true);
    }
    void goDisp()
    {
        text1.SetActive(false);
        goText.SetActive(true);
    }
}
