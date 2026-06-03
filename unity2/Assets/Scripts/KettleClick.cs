using UnityEngine;

public class KettleClick : MonoBehaviour
{
    public MixingManager mixingManager;

    public void OnKettleClick()
    {
        Debug.Log("🔨 Котёл (UI) нажат!");
        
        // Находим Inventory на сцене
        Inventory inventory = FindAnyObjectByType<Inventory>();
    if (inventory == null)
    {
        Debug.LogError("❌ Inventory не найден на сцене!");
        return;
    }
    
    int selectedSlot = inventory.GetSelectedSlot();
    Debug.Log($"Выбранный слот: {selectedSlot}");
    
    if (selectedSlot != -1)
    {
        inventory.SelectSlot(-1);  
    }

        if (selectedSlot >= 4) 
        {
            Debug.Log("❌ Это не ингредиент для замеса (нужны слоты 0-3)!");
            return;
        }

        if (inventory.items[selectedSlot].itemData == null || inventory.items[selectedSlot].count == 0)
        {
            Debug.Log("❌ В выбранном слоте нет ингредиента!");
            return;
        }

        if (mixingManager == null)
        {
            mixingManager = FindAnyObjectByType<MixingManager>();
        }
        
        mixingManager.AddIngredientFromSlot(selectedSlot);
        inventory.SelectSlot(-1);
    }
}