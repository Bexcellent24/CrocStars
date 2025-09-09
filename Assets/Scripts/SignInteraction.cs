using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignInteraction : MonoBehaviour
{
    public GameObject interactionPrompt; // Reference to the "E to Interact" text UI
    public GameObject levelUIPanel; // Reference to the Level UI Panel

    private bool isPlayerNearby = false;

    private void Start()
    {
        // Ensure prompt is initially inactive
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        if (levelUIPanel != null)
            levelUIPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the trigger area
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(true);
            Debug.Log("Player hit sign");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player exits the trigger area
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
            if (levelUIPanel != null)
                levelUIPanel.SetActive(false);
        }
    }

    private void Update()
    {
        // Check if the player is nearby and presses the "E" key
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
            if (levelUIPanel != null)
                levelUIPanel.SetActive(true);
        }
    }
}