using UnityEngine;

public class KettleClick : MonoBehaviour
{
    public MixingManager mixingManager;
    public Inventory inventory;

    void Start()
    {
        // Пытаемся найти, но не падаем, если не нашли
        TryFindComponents();
    }

    void TryFindComponents()
    {
        if (mixingManager == null)
            mixingManager = FindAnyObjectByType<MixingManager>();
        
        if (inventory == null)
        {
            inventory = Inventory.Instance;
            if (inventory == null)
                inventory = FindAnyObjectByType<Inventory>();
        }
    }

    public void OnKettleClick()
    {
        // Ищем КАЖДЫЙ РАЗ при клике
        TryFindComponents();
        
        if (inventory == null)
        {
            Debug.LogError("inventory равен null! Не могу добавить ингредиент.");
            return;
        }
        
        if (mixingManager == null)
        {
            Debug.LogError("mixingManager равен null! Не могу добавить ингредиент.");
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
        inventory.SelectSlot(-1);
    }
}