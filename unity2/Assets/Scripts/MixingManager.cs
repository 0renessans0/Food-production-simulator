using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MixingManager : MonoBehaviour
{
    public Inventory inventory;
    public Slider progressBar;
    public string nextScene = "FermentationScen";
    public string stageName = "Замес";

    private int ingredientsAdded = 0;
    private int requiredIngredients = 4;

    void Start()
    {
        Debug.Log("=== СЦЕНА ЗАМЕСА ===");
        
        if (inventory == null)
        {
            inventory = Inventory.Instance;
            if (inventory == null)
                inventory = FindAnyObjectByType<Inventory>();
        }
        
        // Восстанавливаем инвентарь
        Transform panel = GameObject.Find("InventoryPanel")?.transform;
        if (panel != null && Inventory.Instance != null)
        {
            Inventory.Instance.SetInventoryPanel(panel);
            
            var saved = Inventory.Instance.savedItems;
            if (saved != null && saved.Count > 0)
            {
                for (int i = 0; i < saved.Count && i < Inventory.Instance.items.Count; i++)
                {
                    if (saved[i].itemData != null)
                    {
                        Inventory.Instance.items[i].itemData = saved[i].itemData;
                        Inventory.Instance.items[i].count = saved[i].count;
                        Inventory.Instance.UpdateSlotUI(i);
                    }
                }
            }
        }
        
        if (progressBar != null)
        {
            progressBar.maxValue = requiredIngredients;
            progressBar.value = ingredientsAdded;
        }

        // Привязываем кнопку
        Button nextButton = GameObject.Find("Button_ToFermentation")?.GetComponent<Button>();
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => NextScene());
            Debug.Log("✅ Кнопка привязана к NextScene()");
        }
    }

    public void AddIngredientFromSlot(int slotIndex)
    {
        if (ingredientsAdded >= requiredIngredients)
        {
            Debug.Log("⚠️ Все ингредиенты уже добавлены!");
            return;
        }

        if (inventory == null)
        {
            inventory = FindAnyObjectByType<Inventory>();
            if (inventory == null) return;
        }

        if (slotIndex >= inventory.items.Count) return;
        
        var item = inventory.items[slotIndex].itemData;
        if (item == null || inventory.items[slotIndex].count == 0)
        {
            Debug.Log($"❌ В слоте {slotIndex} нет ингредиента!");
            return;
        }

        if (slotIndex >= 4)
        {
            Debug.Log($"❌ Слот {slotIndex} не является ингредиентом для замеса!");
            return;
        }

        ingredientsAdded++;
        if (progressBar != null)
            progressBar.value = ingredientsAdded;
        
        inventory.ClearSlot(slotIndex);
        
        Debug.Log($"🍺 Добавлен {item.itemName} в чан! {ingredientsAdded}/{requiredIngredients}");
    }

    public void NextScene()
    {
        Debug.Log($"NextScene() вызван. ingredientsAdded={ingredientsAdded}, required={requiredIngredients}");
        
        if (ingredientsAdded >= requiredIngredients)
        {
            if (Inventory.Instance != null)
                Inventory.Instance.SaveItems();
            
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log($"❌ Не хватает ингредиентов: {ingredientsAdded} из {requiredIngredients}");
    
        }
    }
}