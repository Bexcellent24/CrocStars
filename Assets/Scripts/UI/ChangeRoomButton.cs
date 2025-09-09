using UnityEngine;
using UnityEngine.UI;

public class ChangeRoomButton : MonoBehaviour
{
    public CrocsType crocsType; // The CrocsType this button represents
    public ChangeRoomManager changeRoomManager; // Reference to the ChangeRoomManager

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(() => changeRoomManager.EquipFromInventory(crocsType));
    }
}