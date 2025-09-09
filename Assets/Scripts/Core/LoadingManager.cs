using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadLobby()
    {
        SceneManager.LoadScene(1);
        Debug.Log("Loading Lobby");
    }

    public void LoadNewGame()
    {
        GameManager.instance.currency = 0;
        //PlayerPrefs.SetInt("PlayerCurrency", 0);
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(5);
    }
    public void LoadPlatformer()
    {
        SceneManager.LoadScene(2);
    }
    public void LoadRunner()
    {
        SceneManager.LoadScene(3);
    }
    public void LoadJumper()
    {
        SceneManager.LoadScene(4);
    }
   
    public void Restart()
    {
        // Get the active scene's name
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Reload the current scene
        SceneManager.LoadScene(currentSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}