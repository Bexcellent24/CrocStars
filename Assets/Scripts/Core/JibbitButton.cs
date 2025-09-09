using UnityEngine;
using UnityEngine.UI;

public class JibbitButton : MonoBehaviour
{
    public JibbitManager.JibbitType jibbitType;
    private ChangeRoomManager changeRoomManager;

    public void Initialize(ChangeRoomManager manager)
    {
        changeRoomManager = manager;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        // Check if the Jibbit is already equipped
        if (changeRoomManager.jibbitManager.leftSlot == jibbitType || 
            changeRoomManager.jibbitManager.rightSlot == jibbitType)
        {
            // If equipped, unequip it
            changeRoomManager.UnequipJibbit(changeRoomManager.jibbitManager.leftSlot == jibbitType);
        }
        else
        {
            // Otherwise, equip it
            changeRoomManager.OnJibbitSelected(jibbitType);
        }
    }
}