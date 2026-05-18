using UnityEngine;
using UnityEngine.UI;

public class MixingManager : MonoBehaviour
{
    public UIManager uiManager;
    public Inventory inventory;
    public Slider progressBar;
    public string nextScene = "FermentationScene";
    public string stageName = "Замес";

    private int ingredientsAdded = 0;
    private int requiredIngredients = 4;

    public void AddIngredient(int slotIndex)
    {
        if (ingredientsAdded >= requiredIngredients)
        {
            Debug.Log("⚠️ Все ингредиенты уже добавлены!");
            return;
        }

        var item = inventory.items[slotIndex].itemData;
        if (item == null || inventory.items[slotIndex].count == 0)
        {
            Debug.Log($"❌ В слоте {slotIndex} нет ингредиента!");
            return;
        }

        // Увеличиваем счётчик
        ingredientsAdded++;
        
        // Обновляем шкалу
        progressBar.value = ingredientsAdded;
        
        // Очищаем слот
        inventory.items[slotIndex].itemData = null;
        inventory.items[slotIndex].count = 0;
        inventory.UpdateSlotUI(slotIndex);
        
        Debug.Log($"🍺 Добавлен ингредиент {ingredientsAdded}/{requiredIngredients} из слота {slotIndex}");
        
        if (ingredientsAdded >= requiredIngredients)
        {
            Debug.Log("🎉 Все ингредиенты добавлены! Можно переходить на следующий этап.");
        }
    }

    void Start()
{
    // Находим панель инвентаря
    Transform panel = GameObject.Find("InventoryPanel")?.transform;
    if (panel != null)
    {
        Inventory.Instance.SetInventoryPanel(panel);
    }
    else
    {
        Debug.LogError("InventoryPanel не найден на сцене!");
    }
}

    public void AddIngredientFromSlot(int slotIndex)
{
    if (ingredientsAdded >= requiredIngredients)
    {
        Debug.Log("⚠️ Все ингредиенты уже добавлены!");
        return;
    }

    var item = inventory.items[slotIndex].itemData;
    if (item == null || inventory.items[slotIndex].count == 0)
    {
        Debug.Log($"❌ В слоте {slotIndex} нет ингредиента!");
        return;
    }

    ingredientsAdded++;
    progressBar.value = ingredientsAdded;
    
    inventory.items[slotIndex].itemData = null;
    inventory.items[slotIndex].count = 0;
    inventory.UpdateSlotUI(slotIndex);
    
    Debug.Log($"🍺 Добавлен {item.itemName} в чан! {ingredientsAdded}/{requiredIngredients}");
}

    public void CheckStage()
    {
        if (ingredientsAdded >= requiredIngredients)
        {
            uiManager.ShowSuccess("✅ Замес пройден!", nextScene, stageName);
        }
        else
        {
            uiManager.ShowError($"❌ Добавлено {ingredientsAdded} из {requiredIngredients} ингредиентов", stageName);
        }
    }
}