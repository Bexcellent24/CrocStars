using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JibbitManager : MonoBehaviour
{
    public delegate void JibbitEquipped(JibbitType jibbit, bool isLeftSlot);
    public static event JibbitEquipped OnJibbitEquipped;
    
    public delegate void JibbitUnequipped(JibbitType jibbit, bool isLeftSlot);
    public static event JibbitUnequipped OnJibbitUnequipped;
    
    public static Action<JibbitType, JibbitType> OnLoadSlotData;
    
    public enum JibbitType { None, DoubleJump, Fireball, Dash };

    [Header("Equipped Jibbets")]
    public JibbitType leftSlot = JibbitType.None;
    public JibbitType rightSlot = JibbitType.None;
    
    
    
    public bool CanDoubleJump => leftSlot == JibbitType.DoubleJump || rightSlot == JibbitType.DoubleJump;
    public bool CanShootFireball => leftSlot == JibbitType.Fireball || rightSlot == JibbitType.Fireball;
    
    public bool CanDash => leftSlot == JibbitType.Dash || rightSlot == JibbitType.Dash;
    
    public void EquipJibbi(JibbitType jibbit, bool isLeftSlot)
    {
        if (isLeftSlot)
            leftSlot = jibbit;
        else
            rightSlot = jibbit;

        // Fire event to notify the GameManager
        OnJibbitEquipped?.Invoke(jibbit, isLeftSlot);
    }

    public void UnequipJibbit(bool isLeftSlot)
    {
        JibbitType jibbit = isLeftSlot ? leftSlot : rightSlot;
        
        if (isLeftSlot)
            leftSlot = JibbitType.None;
        else
            rightSlot = JibbitType.None;
        
        // Fire event to notify the GameManager
        OnJibbitUnequipped?.Invoke(jibbit, isLeftSlot);
        
        GetComponent<PlayerMovement>().ResetJumpCounter();
        
    }
    
    public bool EquipToAvailableSlot(JibbitType jibbitType)
    {
        if (leftSlot == JibbitType.None)
        {
            EquipJibbi(jibbitType, true);
            return true;
        }
        else if (rightSlot == JibbitType.None)
        {
            EquipJibbi(jibbitType, false);
            return true;
        }
        return false;
    }

    private void UpdateFireballJibbitStatus(bool hasJibbit)
    {
        if (hasJibbit)
        {
            EquipToAvailableSlot(JibbitType.Fireball);
        }
    }
    
    private void UpdateDoubleJumpJibbitStatus(bool hasJibbit)
    {
        if (hasJibbit)
        {
            EquipToAvailableSlot(JibbitType.DoubleJump);
        }
    }
    private void UpdateDashJibbitStatus(bool hasJibbit)
    {
        if (hasJibbit)
        {
            EquipToAvailableSlot(JibbitType.Dash);
        }
    }
    
    private void LoadJibbitSlots(JibbitType leftJibbit, JibbitType rightJibbit)
    {
        Debug.Log("Loading Jibbit Slots in JibbitManager: Left = " + leftJibbit + ", Right = " + rightJibbit);
        leftSlot = leftJibbit;
        rightSlot = rightJibbit;
    }

    private void OnEnable()
    {
        GameManager.OnLoadFireballJibbit += UpdateFireballJibbitStatus;
        GameManager.OnLoadDoubleJumpJibbit += UpdateDoubleJumpJibbitStatus;
        GameManager.OnLoadDashJibbit += UpdateDashJibbitStatus;
        GameManager.OnLoadSlotData += LoadJibbitSlots;
    }

    private void OnDisable()
    {
        GameManager.OnLoadFireballJibbit -= UpdateFireballJibbitStatus;
        GameManager.OnLoadDoubleJumpJibbit -= UpdateDoubleJumpJibbitStatus;
        GameManager.OnLoadDashJibbit -= UpdateDashJibbitStatus;
        GameManager.OnLoadSlotData -= LoadJibbitSlots;
    }
}

