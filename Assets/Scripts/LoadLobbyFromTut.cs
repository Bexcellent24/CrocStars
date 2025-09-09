using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLobbyFromTut : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
       
        
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
                
        }
        
        
    }
}
