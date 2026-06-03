using UnityEngine;

public class InventorySlotUI : MonoBehaviour
{
    private int slotIndex;
    private Inventory inventory;

    void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
        
        // Определяем индекс из имени объекта
        if (int.TryParse(gameObject.name, out int parsedIndex))
        {
            slotIndex = parsedIndex;
        }
        else
        {
            slotIndex = transform.GetSiblingIndex();
        }
        
        Debug.Log($"InventorySlotUI на {gameObject.name}, индекс = {slotIndex}");
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
        Debug.Log($"Установлен индекс слота: {slotIndex}");
    }

    public void OnSlotClick()
    {
        if (inventory == null)
        {
            inventory = FindAnyObjectByType<Inventory>();
            if (inventory == null)
            {
                Debug.LogError("Inventory не найден");
                return;
            }
        }
        
        if (slotIndex < 0 || slotIndex >= inventory.items.Count)
        {
            Debug.LogError($"slotIndex {slotIndex} вне диапазона (0-{inventory.items.Count - 1})");
            return;
        }
        
        inventory.SelectSlot(slotIndex);
        Debug.Log($"Клик по слоту {slotIndex} (предмет: {inventory.items[slotIndex].itemData?.itemName})");
    }
}