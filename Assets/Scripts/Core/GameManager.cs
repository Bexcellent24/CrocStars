using System;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public delegate void LoadCrocs(CrocsType equippedCrocs);
    public static event LoadCrocs OnLoadCrocsLoaded;

    public delegate void LoadFireballJibbit(bool hasFireballJibbit);
    public static event LoadFireballJibbit OnLoadFireballJibbit;

    public delegate void LoadDoubleJumpJibbit(bool hasDoubleJumpJibbit);
    public static event LoadDoubleJumpJibbit OnLoadDoubleJumpJibbit;
    
    public delegate void LoadDashJibbit(bool hasDashJibbit);
    public static event LoadDashJibbit OnLoadDashJibbit;
    
    public static Action<JibbitManager.JibbitType, JibbitManager.JibbitType> OnLoadSlotData;
    
    public PlayerInventory playerInventory;
    
    public int currency;
    private CrocsType equippedCrocs;
    
    
    [Header("Crocs List")]
    public List<CrocsType> allCrocsTypes;
    
    [Header("Default Crocs")]
    public CrocsType defaultCrocs;
    
    [Header("Jibbit Bools")]
    public bool hasDoubleJumpJibbit;
    public bool hasFireballJibbit;
    public bool hasDashJibbit;
    
    [Header("Jibbit Slots")]
    public JibbitManager.JibbitType leftSlotJibbit = JibbitManager.JibbitType.None;
    public JibbitManager.JibbitType rightSlotJibbit = JibbitManager.JibbitType.None;
    
   
    
    //Singltion
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeGame();
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to scene loaded events
            InvokeLoadEvents(); // Call here as well to ensure first load
        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    private void OnEnable()
    {
        JibbitManager.OnJibbitEquipped += HandleJibbitEquipped;
        JibbitManager.OnJibbitUnequipped += HandleJibbitUnequipped;
        CurrencyPickup.OnMoneySent += AddCurrency;
        CrocsManager.OnCrocsEquipped += UpdateEqippedCrocs;
        Health.OnPlayerDied += OnPlayerDiedHandler;
        
    }

    private void OnDisable()
    {
        JibbitManager.OnJibbitEquipped -= HandleJibbitEquipped;
        JibbitManager.OnJibbitUnequipped -= HandleJibbitUnequipped;
        CurrencyPickup.OnMoneySent -= AddCurrency;
        CrocsManager.OnCrocsEquipped -= UpdateEqippedCrocs;
        Health.OnPlayerDied -= OnPlayerDiedHandler;
        
    }

    private void InitializeGame()
    {
        // Load initial values
        currency = PlayerPrefs.GetInt("PlayerCurrency", 0);
        playerInventory ??= new PlayerInventory();
        playerInventory.LoadInventory(allCrocsTypes);

        string savedCrocsName = PlayerPrefs.GetString("EquippedCrocs", "");
        if (equippedCrocs == null)
        {
            equippedCrocs = defaultCrocs; // Assign default crocs if no match is found
        }
        hasDoubleJumpJibbit = PlayerPrefs.GetInt("HasDoubleJumpJibbit", 0) == 1;
        hasFireballJibbit = PlayerPrefs.GetInt("HasFireballJibbit", 0) == 1;
        hasDashJibbit = PlayerPrefs.GetInt("HasDashJibbit", 0) == 1;
        leftSlotJibbit = (JibbitManager.JibbitType)PlayerPrefs.GetInt("LeftSlotJibbit", (int)JibbitManager.JibbitType.None);
        rightSlotJibbit = (JibbitManager.JibbitType)PlayerPrefs.GetInt("RightSlotJibbit", (int)JibbitManager.JibbitType.None);
        
        InvokeLoadEvents(); // Initialize load events for listeners
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadPlayerPrefs();
        InvokeLoadEvents(); // Re-invoke load events on new scenes
    }

    private void InvokeLoadEvents()
    {
        Debug.Log("Loading player data in new scene...");
        JibbitManager.OnLoadSlotData?.Invoke(leftSlotJibbit, rightSlotJibbit);
        OnLoadCrocsLoaded?.Invoke(equippedCrocs);
        OnLoadFireballJibbit?.Invoke(hasFireballJibbit);
        OnLoadDoubleJumpJibbit?.Invoke(hasDoubleJumpJibbit);
        OnLoadDashJibbit?.Invoke(hasDashJibbit);
        OnLoadSlotData?.Invoke(leftSlotJibbit, rightSlotJibbit);
    }

    public int Currency
    {
        get => currency;
        private set
        {
            currency = value;
            PlayerPrefs.SetInt("PlayerCurrency", currency);
            PlayerPrefs.Save();
        }
    }

    public void AddCurrency(int amount) => Currency += amount;
    public void SubtractCurrency(int amount) => Currency -= amount;

    private void UpdateEqippedCrocs(CrocsType equippedCrocsType)
    {
        equippedCrocs = equippedCrocsType;
        SaveEquippedData(); // Save each time a jibbit is equipped
    }

    private void HandleJibbitEquipped(JibbitManager.JibbitType jibbit, bool isLeftSlot)
    {
        if (isLeftSlot)
        {
            leftSlotJibbit = jibbit;
        }
        else
        {
            rightSlotJibbit = jibbit;
        }

        if (jibbit == JibbitManager.JibbitType.DoubleJump) hasDoubleJumpJibbit = true;
        if (jibbit == JibbitManager.JibbitType.Fireball) hasFireballJibbit = true;
        if (jibbit == JibbitManager.JibbitType.Dash) hasDashJibbit = true;

        SaveEquippedData(); // Save each time a jibbit is equipped
    }

    private void HandleJibbitUnequipped(JibbitManager.JibbitType jibbit, bool isLeftSlot)
    {
        if (isLeftSlot)
        {
            leftSlotJibbit = JibbitManager.JibbitType.None;
        }
        else
        {
            rightSlotJibbit = JibbitManager.JibbitType.None;
        }

        if (jibbit == JibbitManager.JibbitType.DoubleJump) hasDoubleJumpJibbit = false;
        if (jibbit == JibbitManager.JibbitType.Fireball) hasFireballJibbit = false;
        if (jibbit == JibbitManager.JibbitType.Dash) hasDashJibbit = false;

        SaveEquippedData(); // Save each time a jibbit is equipped
    }

    private void OnPlayerDiedHandler()
    {
        
    }
    
    private void SaveEquippedData()
    {
        PlayerPrefs.SetString("EquippedCrocs", equippedCrocs.crocsName); 
        PlayerPrefs.SetInt("LeftSlotJibbit", (int)leftSlotJibbit);
        PlayerPrefs.SetInt("RightSlotJibbit", (int)rightSlotJibbit);
        PlayerPrefs.SetInt("HasDoubleJumpJibbit", hasDoubleJumpJibbit ? 1 : 0);
        PlayerPrefs.SetInt("HasFireballJibbit", hasFireballJibbit ? 1 : 0);
        PlayerPrefs.SetInt("HasDashJibbit", hasDashJibbit ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadPlayerPrefs()
    {
        string savedCrocsName = PlayerPrefs.GetString("EquippedCrocs", "");
    
        
        // Try to find the equipped Crocs based on the saved name
        equippedCrocs = allCrocsTypes.Find(crocs => crocs.crocsName == savedCrocsName);
    
        // If no matching Crocs are found, assign default Crocs
        if (equippedCrocs == null)
        {
            //Debug.Log("Equipped crocs were null: " + equippedCrocs.crocsName);
            equippedCrocs = defaultCrocs; // Ensure defaultCrocs is assigned in the Inspector
        }

        hasDoubleJumpJibbit = PlayerPrefs.GetInt("HasDoubleJumpJibbit", 0) == 1;
        hasFireballJibbit = PlayerPrefs.GetInt("HasFireballJibbit", 0) == 1;
        hasDashJibbit = PlayerPrefs.GetInt("HasDashJibbit", 0) == 1;
        
        leftSlotJibbit = (JibbitManager.JibbitType)PlayerPrefs.GetInt("LeftSlotJibbit", (int)JibbitManager.JibbitType.None);
        rightSlotJibbit = (JibbitManager.JibbitType)PlayerPrefs.GetInt("RightSlotJibbit", (int)JibbitManager.JibbitType.None);
    }
    
    
    

}
