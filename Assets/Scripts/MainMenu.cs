using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public EventSystem eventSystem;
    void Start()
    {
        
    }

    public void exitGame()     
    {
        Application.Quit();
    }

    public void startGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void HighlightedButton(GameObject selectedObj)
    {
        eventSystem.SetSelectedGameObject(selectedObj);
    }
}
