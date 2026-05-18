using UnityEngine;

public class AddItemButton : MonoBehaviour
{
    public Inventory inventory;
    public int slotIndex;
    public ItemData item;

    public void AddItem()
{
    Debug.Log($"Нажата кнопка {gameObject.name}, слот {slotIndex}, предмет {item?.itemName}");
    
    if (inventory == null)
    {
        Debug.LogError("Inventory не назначен!");
        return;
    }

    if (item == null)
    {
        Debug.LogError($"Item не назначен для слота {slotIndex}!");
        return;
    }

    inventory.AddItem(slotIndex, item, 1);
    Debug.Log($"Добавлен {item.itemName} в слот {slotIndex}");
    
    var slot = inventory.items[slotIndex];
    Debug.Log($"Слот {slotIndex}: itemData = {slot.itemData?.itemName}, count = {slot.count}");
}
}