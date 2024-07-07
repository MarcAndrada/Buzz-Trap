using UnityEngine;
using UnityEngine.SceneManagement;



public class MenuController : MonoBehaviour
{
    [SerializeField]
    private string menuSceneName;
    [SerializeField]
    private string gameSceneName;
    [SerializeField]
    private GameObject howToPlayWindow;
    [SerializeField]
    private GameObject creditsWindow;

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
    public void GoToGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void ShowHowToPlay()
    {
        howToPlayWindow.SetActive(true);
    }
    public void HideHowToPlay()
    {
        howToPlayWindow.SetActive(false);
    }
   
    public void ShowCredits()
    {
        creditsWindow.SetActive(true);
    }

    public void HideCredits()
    {
        creditsWindow.SetActive(false);
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
