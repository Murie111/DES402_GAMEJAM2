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
    bool gameDone = false;

    public GameObject readyText;
    public GameObject text3;
    public GameObject text2;
    public GameObject text1;
    public GameObject goText;

    public GameObject fishermanCatchM;
    public GameObject fishermanCatchF;
    public GameObject fishermanPointsM;
    public GameObject fishermanPointsF;
    public GameObject fishPointsM;
    public GameObject fishPointsF;
    public GameObject drawM;
    public GameObject drawF;

    [Space(10)]
    [SerializeField] private Anims FisherSplash;
    [SerializeField] private Anims FishSplash;
    private void Start()
    {
        timer = startTime;
        //  fishTutorial.SetActive(true);
        //  fishermanTutorial.SetActive(true);

        FisherSplash.PlayAnim(4);
        FishSplash.PlayAnim(4);

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
        //invokestartGame by animation seconds
        FisherSplash.PlayAnim(4);
        FishSplash.PlayAnim(4);


    }
    void startGame()
    {
        timer = startTime;
        goText.SetActive(false);
    }
    void Update()
    {
        if (fishReady && fishermanReady)
        {
            //startCountdown();
        }    



        if (timer > 0.0f && !gameDone)
        {
            timer -= Time.deltaTime;
            timerTextFish.text = timer.ToString("0");
            timerTextFisherman.text = timer.ToString("0");
        }
        else if (!gameDone)
        {
            gameDone = true;
            Debug.Log("Game Over!");
            Invoke("pickWinner", 1f);
        }
    }

    void pickWinner()
    {
        if (fishPoints == fishermanPoints)
        {
            drawF.SetActive(true);
            drawM.SetActive(true);
        }
        if (fishPoints > fishermanPoints)
        {
            fishPointsF.SetActive(true);
            fishPointsM.SetActive(true);
        }
        if (fishPoints < fishermanPoints)
        {
            fishermanPointsF.SetActive(true);
            fishermanPointsM.SetActive(true);
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
