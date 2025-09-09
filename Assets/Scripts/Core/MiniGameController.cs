using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameController : MonoBehaviour
{
    public delegate void LevelComplete();
    public static event LevelComplete OnLevelComplete;
    
    [Header("SFX")]
    [SerializeField] private AudioClip victorySound;
    
    [Header("Level Settings")]
    public int levelIndex; // Unique index for each mini-game level
    public int totalCollectibles; // Total collectibles in the level
    public float targetTime; // Target time in seconds for 3rd star

    [Header("UI Elements")]
    public GameObject endLevelUIPanel; // UI panel for end of level
    public Text collectibleText; // UI text to display collectible count
    public Text RunningCollectibleText;
    public Text timeText; // UI text to display level completion time
    public Text runningTimeText; // UI text to display level completion time
    public Image[] starIcons; // Array of star icons for earned stars

    private int starsEarned = 0;
    private int collectiblesCollected = 0;
    private float levelTime = 0f;
    private bool levelCompleted = false;

    private void Start()
    {
        endLevelUIPanel.SetActive(false);
    }

    private void Update()
    {
        // Increment the level timer if the level is not yet completed
        if (!levelCompleted)
        {
            levelTime += Time.deltaTime;
            runningTimeText.text = Mathf.Round(levelTime) + "";
        }
    }

    public void CollectItem()
    {
        collectiblesCollected++;
        RunningCollectibleText.text = collectiblesCollected.ToString();
    }

    public void EndGame()
    {
        levelCompleted = true;
        SoundManager.instance.PlaySound(victorySound);
        // Calculate stars based on completion criteria
        starsEarned = 1; // Base star for completion
        if (collectiblesCollected >= totalCollectibles) starsEarned++; // Second star for all collectibles
        if (levelTime <= targetTime) starsEarned++; // Third star for completing within time

        // Save highest score for level in PlayerPrefs
        SaveStarsToPlayerPrefs();

        // Display end level UI and stats
        ShowEndLevelUI();
        
        OnLevelComplete?.Invoke();
    }

    private void SaveStarsToPlayerPrefs()
    {
        // Save individual stars earned for this attempt
        PlayerPrefs.SetInt("Level_" + levelIndex + "_CompletionStar", starsEarned >= 1 ? 1 : 0);
        PlayerPrefs.SetInt("Level_" + levelIndex + "_CollectiblesStar", collectiblesCollected >= totalCollectibles ? 1 : 0);
        PlayerPrefs.SetInt("Level_" + levelIndex + "_TimeStar", levelTime <= targetTime ? 1 : 0);

        // Calculate best stars across attempts
        int bestCompletionStars = PlayerPrefs.GetInt("Level_" + levelIndex + "_BestCompletionStars", 0);
        int bestCollectiblesStars = PlayerPrefs.GetInt("Level_" + levelIndex + "_BestCollectiblesStars", 0);
        int bestTimeStars = PlayerPrefs.GetInt("Level_" + levelIndex + "_BestTimeStars", 0);

        // Update best stars if this attempt is better
        if (starsEarned >= 1 && bestCompletionStars < 1)
            PlayerPrefs.SetInt("Level_" + levelIndex + "_BestCompletionStars", 1);
        if (collectiblesCollected >= totalCollectibles && bestCollectiblesStars < 1)
            PlayerPrefs.SetInt("Level_" + levelIndex + "_BestCollectiblesStars", 1);
        if (levelTime <= targetTime && bestTimeStars < 1)
            PlayerPrefs.SetInt("Level_" + levelIndex + "_BestTimeStars", 1);
    }


    private void ShowEndLevelUI()
    {
        endLevelUIPanel.SetActive(true); // Display end level UI

        // Update collectible and time texts
        collectibleText.text = collectiblesCollected + "/" + totalCollectibles;
        timeText.text = Mathf.Round(levelTime) + "s/" + targetTime + "s" ;

        // Display earned stars for this attempt
        for (int i = 0; i < starIcons.Length; i++)
        {
            if (starIcons[i] != null) // Check if each icon is not null
            {
                starIcons[i].enabled = i < starsEarned;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When player reaches the end signpost, trigger the EndGame
        if (other.CompareTag("Player") && !levelCompleted)
        {
            EndGame();
        }
    }
}
