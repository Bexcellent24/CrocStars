using UnityEngine;

[CreateAssetMenu(fileName = "NewCrocsType", menuName = "Inventory/CrocsType")]
public class CrocsType : ScriptableObject
{
    public string crocsName;       // Name of the Crocs
    public float speedBoost;         // Stat for speed boost
    public float jumpBoost;          // Stat for jump boost
    public int jibbetSlots;
    public AnimatorOverrideController overrideController; // Reference to the Animator Override Controller for this Crocs

}