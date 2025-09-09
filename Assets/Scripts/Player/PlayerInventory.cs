using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerInventory
{
    private List<CrocsType> inventory = new List<CrocsType>();
    public event Action<CrocsType> OnCrocsAdded;

    public void AddCrocs(CrocsType newCrocs)
    {
        // Check for ownership by crocsName instead of reference
        if (inventory.Exists(c => c.crocsName == newCrocs.crocsName)) 
        {
            Debug.Log($"Crocs already owned: {newCrocs.crocsName}");
            return;
        }

        inventory.Add(newCrocs);
        Debug.Log($"Added Crocs to inventory: {newCrocs.crocsName}"); // Debug log
        SaveInventory();
        OnCrocsAdded?.Invoke(newCrocs);
    }

    public bool HasCrocs(CrocsType crocs)
    {
        // Check if crocs is null before proceeding
        if (crocs == null)
        {
            Debug.LogWarning("Attempted to check for null crocs in inventory.");
            return false;
        }

        return inventory.Exists(c => c.crocsName == crocs.crocsName);
    }

    public void SaveInventory()
    {
        var saveData = new CrocsSaveData { ownedCrocsNames = inventory.ConvertAll(c => c.crocsName) };
        PlayerPrefs.SetString("PlayerInventory", JsonUtility.ToJson(saveData));
        PlayerPrefs.Save();
        Debug.Log($"Inventory loaded: {string.Join(", ", inventory)}");
    }

    public void LoadInventory(List<CrocsType> availableCrocs)
    {
        if (!PlayerPrefs.HasKey("PlayerInventory")) return;

        var saveData = JsonUtility.FromJson<CrocsSaveData>(PlayerPrefs.GetString("PlayerInventory"));
        inventory = availableCrocs.FindAll(crocs => saveData.ownedCrocsNames.Contains(crocs.crocsName));
    
        // Debug log to verify the inventory after loading
        Debug.Log($"Inventory loaded: {string.Join(", ", inventory)}");
    }
}