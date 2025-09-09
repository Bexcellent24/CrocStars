using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class StarManager : MonoBehaviour
{
    
    public delegate void DashUnlocked();
    public static event DashUnlocked OnDashUnlocked;
    
    int totalStarsForLevel1 = 0;
    public List<LevelUI> levelUIs; // Each UI with stars for each level

    private bool justUnlockedDash;
    
    [Header("Dash Unlocked UI")]
    public GameObject dashUnlockedUI;
    private void Start()
    {
        foreach (var levelUI in levelUIs)
        {
            // Retrieve individual star counts from PlayerPrefs
            int completionStars = PlayerPrefs.GetInt("Level_" + levelUI.levelIndex + "_BestCompletionStars", 0);
            int collectiblesStars = PlayerPrefs.GetInt("Level_" + levelUI.levelIndex + "_BestCollectiblesStars", 0);
            int timeStars = PlayerPrefs.GetInt("Level_" + levelUI.levelIndex + "_BestTimeStars", 0);
            
            // Calculate total stars
            int totalStars = completionStars + collectiblesStars + timeStars;

            // Set stars on the UI
            levelUI.SetStars(totalStars);
            
            
            if (levelUI.levelIndex == 3 && totalStars >= 3)
            {
                totalStarsForLevel1 += totalStars;
            }
            if (levelUI.levelIndex == 4 && totalStars >= 3)
            {
                totalStarsForLevel1 += totalStars;
            }
            if (levelUI.levelIndex == 5 && totalStars >= 3)
            {
                totalStarsForLevel1 += totalStars;
            }
            
            // Check if all stars collected for level 1
            if (totalStarsForLevel1 >= 9)
            {
                OnDashUnlocked?.Invoke();
                Debug.Log("Got All Stars");
                DashUnlockedUI();
            }
        }
    }
    private void DashUnlockedUI()
    {
        // Check if the Dash Jibbit was already unlocked
        if (PlayerPrefs.GetInt("DashJibbitUnlocked", 0) == 0) // 0 means it hasn't been unlocked before
        {
            justUnlockedDash = true;
            StartCoroutine(DashUnlockedSequence());
        
            // Set the PlayerPrefs key to mark that Dash Jibbit has been unlocked
            PlayerPrefs.SetInt("DashJibbitUnlocked", 1);
            PlayerPrefs.Save();
        }
    }
    
    private IEnumerator DashUnlockedSequence()
    {
        if (justUnlockedDash)
        {
            yield return new WaitForSeconds(1f);
            dashUnlockedUI.SetActive(true);
            yield return new WaitForSeconds(3f);
            dashUnlockedUI.SetActive(false);
        }
    }

    [System.Serializable]
    public class LevelUI
    {
        public int levelIndex;
        public GameObject[] starIcons; // Array of star icons to enable/disable

        public void SetStars(int stars)
        {
            for (int i = 0; i < starIcons.Length; i++)
                starIcons[i].SetActive(i < stars);
        }
    }
}