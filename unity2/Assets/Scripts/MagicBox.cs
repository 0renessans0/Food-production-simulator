using UnityEngine;

public class MagicBox : MonoBehaviour
{
    public ItemData[] items;           
    public int amountPerClick = 1;

    private Inventory inventory;
    private int currentSlot = 0;

    private void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
        
        if (inventory == null)
        {
            Debug.LogError("❌ MagicBox: Inventory не найден!");
        }
        else
        {
            Debug.Log("✅ MagicBox: Inventory найден");
        }
    }

    public void OnBoxClick()
    {
        Debug.Log($"📦 MagicBox: нажата! currentSlot={currentSlot}");

        if (inventory == null)
        {
            Debug.LogError("❌ Inventory = null, добавление невозможно");
            return;
        }

        if (currentSlot >= items.Length)
        {
            Debug.Log("⚠️ Все предметы уже добавлены!");
            return;
        }

        if (items[currentSlot] == null)
        {
            Debug.LogError($"❌ Предмет в слоте {currentSlot} не назначен!");
            return;
        }

        inventory.AddItem(currentSlot, items[currentSlot], amountPerClick);
        Debug.Log($"✅ Добавлен {items[currentSlot].itemName} в слот {currentSlot}");
        currentSlot++;
    }
}