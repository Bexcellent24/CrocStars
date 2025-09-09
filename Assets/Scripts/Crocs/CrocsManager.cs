using System;
using UnityEngine;

public class CrocsManager : MonoBehaviour
{
    private PlayerMovement playerMovement;       // Reference to the PlayerMovement
    private Animator playerAnimator;             // Reference to the player's Animator
    private CrocsType equippedCrocs;             // Currently equipped Crocs
    public PlayerInventory inventory;            // Reference to the player's inventory
    public CrocsType EquippedCrocs => equippedCrocs; // Add this property to get the currently equipped Crocs
    
    public  delegate void CrocsEquipped(CrocsType crocsType);
    public static event CrocsEquipped OnCrocsEquipped;
    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimator = GetComponent<Animator>();

        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component is missing from this GameObject.");
            return; // Early exit to avoid further null reference issues
        }

        // Use the inventory from GameManager
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager instance is null. Ensure it is created before CrocsManager.");
            return; // Prevent further errors
        }

        inventory = GameManager.instance.playerInventory; // Get inventory from GameManager

        if (inventory == null)
        {
            Debug.LogError("PlayerInventory from GameManager is null.");
        }
    }

    // Method to equip new Crocs
    public void EquipCrocs(CrocsType newCrocs)
    {
        // Check if player owns the Crocs
        if (inventory.HasCrocs(newCrocs))
        {
            equippedCrocs = newCrocs;

            // Apply the Crocs stats to the player movement
            ApplyCrocsStats();

            // Update the animator if there's an override controller
            if (equippedCrocs.overrideController != null)
            {
                playerAnimator.runtimeAnimatorController = equippedCrocs.overrideController;
            }

            OnCrocsEquipped?.Invoke(equippedCrocs);
            Debug.Log($"{newCrocs.crocsName} equipped.");
        }
        else
        {
            Debug.Log("You do not own this Crocs type.");
        }
    }

    // Apply Crocs stats to the player
    private void ApplyCrocsStats()
    {
        if (equippedCrocs == null) return;

        // Example: apply movement speed and jump power boosts
        playerMovement.SetSpeed(equippedCrocs.speedBoost);
        playerMovement.SetJumpPower(equippedCrocs.jumpBoost);
    }

    private void OnEnable()
    {
        GameManager.OnLoadCrocsLoaded += EquipCrocs;
    }

    private void OnDisable()
    {
        GameManager.OnLoadCrocsLoaded -= EquipCrocs;
    }
}

