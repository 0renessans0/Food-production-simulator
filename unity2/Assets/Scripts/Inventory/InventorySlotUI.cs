using UnityEngine;

public class InventorySlotUI : MonoBehaviour
{
    private int slotIndex;
    private Inventory inventory;

    void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
        
        // Если индекс ещё не установлен через SetSlotIndex, определяем сами
        if (slotIndex == 0 && gameObject.name != "0")
        {
            slotIndex = transform.GetSiblingIndex();
        }
        
        Debug.Log($"InventorySlotUI на {gameObject.name}: индекс = {slotIndex}");
    }

    public void SetSlotIndex(int index)
    {
        slotIndex = index;
    }

    public void OnSlotClick()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory не найден!");
            return;
        }
        
        inventory.SelectSlot(slotIndex);
        Debug.Log($"Клик по слоту {slotIndex}");
    }
}