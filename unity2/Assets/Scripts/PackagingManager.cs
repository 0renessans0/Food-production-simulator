using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PackagingManager : MonoBehaviour
{
    public Inventory inventory;
    public GameObject emptyBox;
    public GameObject boxWithBottles;
    public GameObject boxWithLabel;
    public Button stickerButton;
    public Button nextButton;

    private bool isPacked = false;
    private bool hasLabel = false;

    void Start()
    {
        emptyBox.SetActive(true);
        boxWithBottles.SetActive(false);
        boxWithLabel.SetActive(false);
        
        if (stickerButton != null) stickerButton.interactable = false;
        if (nextButton != null) nextButton.interactable = false;
        
        // Восстанавливаем инвентарь
        Transform panel = GameObject.Find("InventoryPanel")?.transform;
        if (panel != null && Inventory.Instance != null)
        {
            Inventory.Instance.SetInventoryPanel(panel);
        }
    }

    // Вызывается при клике на ящик (после выбора бутылки в слоте)
    public void OnBoxClick()
    {
        if (isPacked) return;
        
        int selectedSlot = inventory.GetSelectedSlot();
        
        if (selectedSlot == -1)
        {
            Debug.Log("❌ Сначала выберите бутылку в инвентаре!");
            return;
        }
        
        // Проверяем, что в выбранном слоте — бутылка 0.33
        var item = inventory.items[selectedSlot].itemData;
        if (item == null || item.itemType != ItemType.GlassBottle033)
        {
            Debug.Log("❌ Выберите бутылку 0.33 в инвентаре!");
            return;
        }
        
        // Упаковка
        isPacked = true;
        inventory.ClearSlot(selectedSlot);
        inventory.SelectSlot(-1); // сбрасываем выбор
        
        emptyBox.SetActive(false);
        boxWithBottles.SetActive(true);
        if (stickerButton != null) stickerButton.interactable = true;
        
        Debug.Log("✅ Бутылка упакована!");
    }

    public void ApplySticker()
    {
        if (!isPacked)
        {
            Debug.Log("❌ Сначала упакуйте бутылку!");
            return;
        }
        if (hasLabel) return;
        
        hasLabel = true;
        boxWithBottles.SetActive(false);
        boxWithLabel.SetActive(true);
        if (nextButton != null) nextButton.interactable = true;
        Debug.Log("✅ Этикетка наклеена!");
    }

    public void NextStage()
    {
        if (!isPacked || !hasLabel)
        {
            Debug.Log("❌ Упакуйте бутылку и наклейте этикетку!");
            return;
        }
        
        if (Inventory.Instance != null)
            Inventory.Instance.SaveItems();
        
        SceneManager.LoadScene("5");
    }
}