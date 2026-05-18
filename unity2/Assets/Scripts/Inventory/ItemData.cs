using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]


public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public ItemType itemType;
    public Sprite icon;
    public int maxStackSize = 99;
}


[System.Serializable]


public enum ItemType
{
    None,
    Barley,
    Hops,
    Water,
    Malt,
    GlassBottle033,
    GlassBottle05,
    MetalCan033,
    MetalCan05,
    GlassBottle1, 
}