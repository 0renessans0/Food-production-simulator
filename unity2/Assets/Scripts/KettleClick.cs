using UnityEngine;

public class KettleClick : MonoBehaviour
{
    public MixingManager mixingManager;
    public Inventory inventory;

    void Start()
    {
        if (mixingManager == null)
            mixingManager = FindAnyObjectByType<MixingManager>();
        
        if (inventory == null)
        {
            inventory = Inventory.Instance;
            if (inventory == null)
                inventory = FindAnyObjectByType<Inventory>();
        }
        
        Debug.Log($"kettle click: mixing manager = {(mixingManager != null ? "найден" : "не найден")}");
        Debug.Log($"kettle click: inventory = {(inventory != null ? "найден" : "не найден")}");
    }

    public void OnKettleClick()
    {
        if (inventory == null)
        {
            Debug.LogError("inventory равен null");
            return;
        }
        
        int selectedSlot = inventory.GetSelectedSlot();
        
        if (selectedSlot == -1)
        {
            Debug.Log("сначала выберите ингредиент в инвентаре");
            return;
        }

        if (selectedSlot >= 4) 
        {
            Debug.Log($"слот {selectedSlot} не является ингредиентом для замеса");
            return;
        }

        if (selectedSlot >= inventory.items.Count)
        {
            Debug.Log($"слот {selectedSlot} вне диапазона");
            return;
        }

        if (inventory.items[selectedSlot].itemData == null || inventory.items[selectedSlot].count == 0)
        {
            Debug.Log($"в слоте {selectedSlot} нет ингредиента");
            return;
        }

        mixingManager.AddIngredientFromSlot(selectedSlot);
        
        if (selectedSlot != -1)
            inventory.SelectSlot(-1);
    }
}