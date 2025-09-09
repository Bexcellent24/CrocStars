using UnityEngine;

public class CurrencyPickup : MonoBehaviour
{
    // Delegate and event for currency update
    public  delegate void MoneySent(int amount);
    public static event MoneySent OnMoneySent;
    
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private MiniGameController miniGameController;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SoundManager.instance.PlaySound(pickupSound);
            OnMoneySent?.Invoke(1);
            Destroy(gameObject, .1f);
            miniGameController.CollectItem();
        }
        
    }
}