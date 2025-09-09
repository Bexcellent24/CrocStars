using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeRoomManager : MonoBehaviour
{
    [Header("Croc UI Elements")]
    [SerializeField] private GameObject changeRoomUI;
    [SerializeField] private GameObject defaultCrocsUI;
    [SerializeField] private GameObject platformerCrocsUI;
    [SerializeField] private GameObject speedCrocsUI;
  
    
    [Header("Equipped Text")]
    [SerializeField] private Text defaultEquippedText; // Text to indicate equipped Crocs
    [SerializeField] private Text platformerEquippedText; // Text to indicate equipped Crocs
    [SerializeField] private Text speedEquippedText; // Text to indicate equipped Crocs

    [Header("Croc Types")]
    public CrocsType defaultCrocsType;
    public CrocsType platformerCrocsType;
    public CrocsType speedCrocsType; // Default Crocs type to be owned initially
    
    [Header("Jibbit UI Elements")]
    [SerializeField] private List<JibbitButton> jibbitButtons; // List of Jibbit buttons
    [SerializeField] private GameObject DashJibbitUI;
    
    [Header("Jibbit UI Elements")]
    [SerializeField] private Image leftJibbitSlotUI; // UI slot for left Jibbit
    [SerializeField] private Image rightJibbitSlotUI; // UI slot for right Jibbit
    [SerializeField] private Sprite defaultJibbitSprite; // Default empty slot sprite
    
    [Header("Jibbit Sprites")]
    [SerializeField] private Sprite doubleJumpSprite;
    [SerializeField] private Sprite fireballSprite;
    [SerializeField] private Sprite dashSprite;
    
    [Header("References")]
    public CrocsManager crocsManager;
    public JibbitManager jibbitManager;
    
    [Header("SFX")]
    [SerializeField] private AudioClip equipSound;
    
    private Dictionary<CrocsType, GameObject> crocsUIElements = new Dictionary<CrocsType, GameObject>();
    private Dictionary<CrocsType, Text> crocsTextElements = new Dictionary<CrocsType, Text>();
    private void Start()
    {
        UpdateJibbitSlotsUI();
        // Equip default Crocs at the start
        if (crocsManager != null)
        {
            GameManager.instance.playerInventory.AddCrocs(defaultCrocsType);
        }
        
        GameManager.instance.playerInventory.OnCrocsAdded += (addedCrocs) => UpdateOwnedCrocsUI();
        SetupCrocsUI(); // Call your method to populate UI elements here
        changeRoomUI.SetActive(false);
        DashJibbitUI.SetActive(false);
        UpdateOwnedCrocsUI();
        SetupJibbitButtons(); 
    }
    private void SetupCrocsUI()
    {
        // Clear existing UI elements to avoid duplicates
        foreach (var uiElement in crocsUIElements.Values)
        {
            uiElement.SetActive(false);
        }

        // Initialize the UI elements for each Crocs type
        crocsUIElements = new Dictionary<CrocsType, GameObject>
        {
            { defaultCrocsType, defaultCrocsUI },
            { platformerCrocsType, platformerCrocsUI },
            { speedCrocsType, speedCrocsUI }
        };
        
        // Initialize the text elements for each Crocs type
        crocsTextElements = new Dictionary<CrocsType, Text>
        {
            { defaultCrocsType, defaultEquippedText },
            { platformerCrocsType, platformerEquippedText },
            { speedCrocsType, speedEquippedText }
        };

        // Setup the UI elements only for owned Crocs
        foreach (var crocsType in crocsUIElements.Keys)
        {
            bool owned = GameManager.instance.playerInventory.HasCrocs(crocsType);
            if (owned)
            {
                crocsUIElements[crocsType].SetActive(true); // Show only if owned
            }
        }

        Debug.Log($"Setup complete. Total Crocs UI Elements: {crocsUIElements.Count}");
        UpdateOwnedCrocsUI();
    }


    public void UpdateOwnedCrocsUI()
    {
        
        // Loop through all owned Crocs and update UI
        foreach (var entry in crocsUIElements)
        {
            if (entry.Value == null || entry.Key == null || !GameManager.instance.playerInventory.HasCrocs(entry.Key))
            {
                continue; // Skip if the UI element or Crocs type is null or not owned
            }

            entry.Value.SetActive(true); // Show or hide the UI element based on ownership
            Text buttonText = crocsTextElements.ContainsKey(entry.Key) ? crocsTextElements[entry.Key] : null;
            if (buttonText != null)
            {
                buttonText.text = (crocsManager.EquippedCrocs == entry.Key) ? "Equipped" : "Equip";
            }
        }
    }

    public void EquipFromInventory(CrocsType selectedCrocs)
    {
        if (!GameManager.instance.playerInventory.HasCrocs(selectedCrocs)) return;
        
        SoundManager.instance.PlaySound(equipSound);
        crocsManager.EquipCrocs(selectedCrocs);
        UpdateOwnedCrocsUI();
    }
    
    private void SetupJibbitButtons()
    {
        foreach (var button in jibbitButtons)
        {
            button.Initialize(this);
        }
    }

    public void OnJibbitSelected(JibbitManager.JibbitType selectedJibbit)
    {
        if (jibbitManager.EquipToAvailableSlot(selectedJibbit))
        {
            SoundManager.instance.PlaySound(equipSound);
            UpdateJibbitSlotsUI();
        }
        else
        {
            Debug.Log("Both slots are filled. Unequip a Jibbit to add a new one.");
        }
    }

    public void UnequipJibbit(bool isLeftSlot)
    {
        SoundManager.instance.PlaySound(equipSound);
        jibbitManager.UnequipJibbit(isLeftSlot);
        UpdateJibbitSlotsUI();
    }

    private void UpdateJibbitSlotsUI()
    {
        leftJibbitSlotUI.sprite = jibbitManager.leftSlot == JibbitManager.JibbitType.None
            ? defaultJibbitSprite : GetJibbitSprite(jibbitManager.leftSlot);
        rightJibbitSlotUI.sprite = jibbitManager.rightSlot == JibbitManager.JibbitType.None
            ? defaultJibbitSprite : GetJibbitSprite(jibbitManager.rightSlot);
    }

    private Sprite GetJibbitSprite(JibbitManager.JibbitType jibbitType)
    {
        return jibbitType switch
        {
            JibbitManager.JibbitType.DoubleJump => doubleJumpSprite,
            JibbitManager.JibbitType.Fireball => fireballSprite,
            JibbitManager.JibbitType.Dash => dashSprite,
            _ => defaultJibbitSprite
        };
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered change room.");
            changeRoomUI.SetActive(true);
            SetupCrocsUI(); 
            UpdateOwnedCrocsUI();// Setup UI based on current inventory
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (changeRoomUI == null)
        {
            return;
        }
        changeRoomUI.SetActive(false);
    }

    private void DashUnlockedHandler()
    {
        DashJibbitUI.SetActive(true);
    }

    private void OnEnable()
    {
        StarManager.OnDashUnlocked += DashUnlockedHandler;
    }

    private void OnDisable()
    {
        StarManager.OnDashUnlocked -= DashUnlockedHandler;
    }

    private void OnDestroy() => GameManager.instance.playerInventory.OnCrocsAdded -= (addedCrocs) => UpdateOwnedCrocsUI();
}