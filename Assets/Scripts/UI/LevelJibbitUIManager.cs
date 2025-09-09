using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelJibbitUIManager : MonoBehaviour
{
    [Header("Jibbit UI Elements")]
    [SerializeField] private Image leftJibbitSlotUI; // UI slot for left Jibbit
    [SerializeField] private Image rightJibbitSlotUI; // UI slot for right Jibbit
    
    [Header("Jibbit Sprites")]
    [SerializeField] private Sprite doubleJumpSprite;
    [SerializeField] private Sprite fireballSprite;
    [SerializeField] private Sprite dashSprite;
    [SerializeField] private Sprite emptySprite;

    

    private void OnEnable()
    {
        if (GameManager.instance != null)
        {
            GameManager.OnLoadSlotData += UpdateJibbitSlots;
        }
        else
        {
            Debug.LogError("GameManager instance is null!");
        }
    }

    private void OnDisable()
    {
        if (GameManager.instance != null)
        {
            GameManager.OnLoadSlotData -= UpdateJibbitSlots;
        }
    }

    private void UpdateJibbitSlots(JibbitManager.JibbitType leftSlotJibbit, JibbitManager.JibbitType rightSlotJibbit)
    {
        
        UpdateSlotUI(leftJibbitSlotUI, leftSlotJibbit);
        UpdateSlotUI(rightJibbitSlotUI, rightSlotJibbit);
    }

    private void UpdateSlotUI(Image slotUI, JibbitManager.JibbitType jibbitType)
    {
        
        switch (jibbitType)
        {
            case JibbitManager.JibbitType.DoubleJump:
                slotUI.sprite = doubleJumpSprite;
                break;
            case JibbitManager.JibbitType.Fireball:
                slotUI.sprite = fireballSprite;
                break;
            case JibbitManager.JibbitType.None:
                slotUI.sprite = emptySprite;
                break;
            case JibbitManager.JibbitType.Dash:
                slotUI.sprite = dashSprite;
                break;
            default:
                slotUI.sprite = null; // Clear the slot if no jibbit is equipped
                break;
        }
    }
}