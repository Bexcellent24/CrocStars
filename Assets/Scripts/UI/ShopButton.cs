using UnityEngine;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    public CrocsType crocsType; 
    public ShopManager shopManager; 
    [SerializeField] private Text purchasedText; 

    public void Initialize(ShopManager manager)
    {
        shopManager = manager;
        RefreshPurchaseStatus();
    }

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(OnPurchaseButtonClick);
    }

    private void OnPurchaseButtonClick()
    {
        if (shopManager != null)
        {
            shopManager.PurchaseCrocs(crocsType);
        }
    }

    public void RefreshPurchaseStatus()
    {
        if (shopManager != null && GameManager.instance.playerInventory.HasCrocs(crocsType))
        {
            purchasedText.text = "PURCHASED";
            Debug.Log($"Crocs owned: {crocsType.crocsName}");
        }
        else
        {
            purchasedText.text = "";
            Debug.Log($"Crocs not owned: {crocsType.crocsName}");
        }
    }
}