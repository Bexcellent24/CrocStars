using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header ("Currency")]
    [SerializeField] private Text currencyText;

    [Header ("Game Over")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Pause")]
    [SerializeField] private GameObject pauseScreen;

    [Header("Level 1")]
    [SerializeField] private GameObject level1Screen;

    private void Awake()
    {
        level1Screen.SetActive(false);
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(!pauseScreen.activeInHierarchy);
        }

        // Assuming Currency is a property or a method that returns an integer
        currencyText.text = GameManager.instance.Currency.ToString();
    }

    private void OnEnable()
    {
        Health.OnPlayerDied += OnPlayerDiedHandler;
    }

    private void OnDisable()
    {
        Health.OnPlayerDied -= OnPlayerDiedHandler;
    }
    
    public void CloseLevelUI()
    {
        level1Screen.SetActive(false);
    }
    
    private void OnPlayerDiedHandler()
    {
        StartCoroutine(DeathSequence());
    }

    #region Game Over
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        SoundManager.instance.PlaySound(gameOverSound);
    }

    public void LoadLobby()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void ReloadTutorial()
    {
        Time.timeScale = 1;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("PlayerCurrency", 0);
        PlayerPrefs.Save();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(5);
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion

    #region Pause
    public void PauseGame(bool status)
    {
        pauseScreen.SetActive(status);
        Time.timeScale = status ? 0 : 1;
    }

    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }

    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }
    #endregion

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(1);
        GameOver();
    }
}