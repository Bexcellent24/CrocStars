using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject shopUI;
    [SerializeField] private List<CrocsType> availableCrocs;
    [SerializeField] private GameObject notEnoughMoney;
    [SerializeField] private ChangeRoomManager changeRoomManager;
    
    [Header("SFX")]
    [SerializeField] private AudioClip purchaseSound;
    
    public PlayerInventory playerInventory;
    private List<ShopButton> shopButtons;

    private void Awake()
    {
        // Ensure shopUI is assigned
        if (shopUI == null)
        {
            Debug.LogError("Shop UI reference is missing in ShopManager.");
            return;
        }

        // Initialize shop buttons once the shop manager is active
        shopButtons = new List<ShopButton>(shopUI.GetComponentsInChildren<ShopButton>());
        if (shopButtons.Count == 0)
        {
            Debug.LogError("No ShopButton components found in the shop UI.");
            return;
        }

        // Set the reference to ShopManager for each ShopButton
        foreach (var button in shopButtons)
        {
            button.shopManager = this;
        }

        // Initially hide the shop UI
        shopUI.SetActive(false);
    }

    private void OnEnable()
    {
        // Refresh buttons when ShopManager is enabled in the scene
        RefreshShopButtons();
    }

    public void PurchaseCrocs(CrocsType crocsToBuy)
    {
        int cost = 0;

        // Determine the cost based on Crocs type
        if (crocsToBuy.crocsName == "PlatformerCrocs")
        {
            cost = 60;
        }
        else if (crocsToBuy.crocsName == "SpeedCrocs")
        {
            cost = 50;
        }
        
        if (GameManager.instance.Currency >= cost)
        {
            SoundManager.instance.PlaySound(purchaseSound);
            // Subtract currency from the GameManager and add Crocs to the inventory
            GameManager.instance.SubtractCurrency(cost);
            GameManager.instance.playerInventory.AddCrocs(crocsToBuy);
            RefreshShopButtons();
            Debug.Log($"Purchased {crocsToBuy.crocsName} for {cost} currency.");
        }
        else
        {
            // Show "Not Enough Money" message for 2 seconds
            StartCoroutine(ShowNotEnoughMoneyMessage());
            Debug.Log("Couldn't buy crocs");
        }
        
    }
    
    private IEnumerator ShowNotEnoughMoneyMessage()
    {
        notEnoughMoney.SetActive(true);
        yield return new WaitForSeconds(2f);
        notEnoughMoney.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            shopUI.SetActive(true);
            RefreshShopButtons();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && shopUI != null)
        {
            shopUI.SetActive(false);
        }
    }

    public void RefreshShopButtons()
    {
        if (shopButtons == null) return;

        foreach (var button in shopButtons)
        {
            button.RefreshPurchaseStatus();
        }
    }
}
