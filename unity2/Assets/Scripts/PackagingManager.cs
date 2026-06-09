using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PackagingManager : MonoBehaviour
{
    public UIManager uiManager;
    public Inventory inventory;
    
    // Пустой ящик
    public GameObject emptyBox;
    
    // 5 типов упакованных ящиков
    public GameObject boxWithBottle033;
    public GameObject boxWithBottle05;
    public GameObject boxWithBottle1;
    public GameObject boxWithCan033;
    public GameObject boxWithCan05;
    
    public GameObject sticker;
    public Button stickerButton;
    public Button nextButton;
    
    public string nextScene = "ResultScen";
    public string stageName = "Упаковка";
    
    private bool isPacked = false;
    private bool hasLabel = false;
    private string packedType = "";
    
    void Start()
    {
        if (uiManager == null)
            uiManager = FindAnyObjectByType<UIManager>();
        
        if (inventory == null)
        {
            inventory = Inventory.Instance;
            if (inventory == null)
                inventory = FindAnyObjectByType<Inventory>();
        }
        
        UIManager.Instance.FindPanelsOnCurrentScene();
        
        // Показываем только пустой ящик
        emptyBox.SetActive(true);
        boxWithBottle033.SetActive(false);
        boxWithBottle05.SetActive(false);
        boxWithBottle1.SetActive(false);
        boxWithCan033.SetActive(false);
        boxWithCan05.SetActive(false);
        sticker.SetActive(false);
        
        if (stickerButton != null) stickerButton.interactable = false;
        if (nextButton != null) nextButton.interactable = false;
        
        // Восстанавливаем инвентарь
        Transform panel = GameObject.Find("InventoryPanel")?.transform;
        if (panel != null && Inventory.Instance != null)
        {
            Inventory.Instance.SetInventoryPanel(panel);
        }
        
        // Привязываем кнопки
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => CheckStage());
            Debug.Log("кнопка Next привязана к CheckStage");
        }
        
        if (stickerButton != null)
        {
            stickerButton.onClick.RemoveAllListeners();
            stickerButton.onClick.AddListener(() => ApplySticker());
            Debug.Log("кнопка Sticker привязана к ApplySticker");
        }
        
        Debug.Log("PackagingManager Start: inventory = " + (inventory != null ? "найден" : "null"));
    }
    
    // Упаковка выбранной тары
    public void OnBoxClick()
    {
        Debug.Log("OnBoxClick вызван");
        
        if (isPacked)
        {
            Debug.Log("Уже упаковано");
            return;
        }
        
        if (inventory == null)
        {
            Debug.LogError("Inventory null");
            return;
        }
        
        int selectedSlot = inventory.GetSelectedSlot();
        Debug.Log($"Выбранный слот: {selectedSlot}");
        
        if (selectedSlot == -1)
        {
            Debug.Log("❌ Сначала выберите тару в инвентаре!");
            if (uiManager != null)
                uiManager.ShowError("Сначала выберите тару в инвентаре!", stageName);
            return;
        }
        
        if (selectedSlot >= inventory.items.Count)
        {
            Debug.Log($"Слот {selectedSlot} вне диапазона");
            return;
        }
        
        var item = inventory.items[selectedSlot].itemData;
        if (item == null)
        {
            Debug.Log("В выбранном слоте нет предмета");
            return;
        }
        
        // Показываем соответствующий ящик в зависимости от типа тары
        switch (item.itemType)
        {
            case ItemType.GlassBottle033:
                packedType = "Bottle033";
                emptyBox.SetActive(false);
                boxWithBottle033.SetActive(true);
                break;
            case ItemType.GlassBottle05:
                packedType = "Bottle05";
                emptyBox.SetActive(false);
                boxWithBottle05.SetActive(true);
                break;
            case ItemType.GlassBottle1:
                packedType = "Bottle1";
                emptyBox.SetActive(false);
                boxWithBottle1.SetActive(true);
                break;
            case ItemType.MetalCan033:
                packedType = "Can033";
                emptyBox.SetActive(false);
                boxWithCan033.SetActive(true);
                break;
            case ItemType.MetalCan05:
                packedType = "Can05";
                emptyBox.SetActive(false);
                boxWithCan05.SetActive(true);
                break;
            default:
                Debug.Log("Этот предмет нельзя упаковать!");
                if (uiManager != null)
                    uiManager.ShowError("Этот предмет нельзя упаковать!", stageName);
                return;
        }
        
        // Удаляем тару из инвентаря и сбрасываем выбор
        inventory.ClearSlot(selectedSlot);
        inventory.SelectSlot(-1);
        
        isPacked = true;
        if (stickerButton != null) stickerButton.interactable = true;
        
        Debug.Log($"Упакована тара: {item.itemName} (тип: {packedType})");
    }
    
    // Наклейка этикетки
    public void ApplySticker()
    {
        Debug.Log("ApplySticker вызван");
        
        if (!isPacked)
        {
            if (uiManager != null)
                uiManager.ShowError("Сначала упакуйте товар!", stageName);
            return;
        }
        
        if (hasLabel) return;
        
        hasLabel = true;
        sticker.SetActive(true);
        
        Debug.Log("Этикетка наклеена!");
    }
    
    // Проверка этапа (аналог CheckStage)
    public void CheckStage()
    {
        Debug.Log("CheckStage вызван");
        
        // Проверка 1: упаковано ли?
        if (!isPacked)
        {
            if (uiManager != null)
                uiManager.ShowError("Сначала упакуйте товар!", stageName);
            return;
        }
        
        // Проверка 2: наклеена ли этикетка?
        if (!hasLabel)
        {
            if (uiManager != null)
                uiManager.ShowError("Сначала наклейте этикетку!", stageName);
            return;
        }
        
        // Проверка 3: правильная ли тара? (только бутылка 0.5л)
        if (packedType != "Bottle05")
        {
            if (ErrorManager.Instance != null)
                ErrorManager.Instance.AddError($"Упаковка: выбрана неверная тара ({packedType}). Нужна бутылка 0.5л");
            
            if (uiManager != null)
                uiManager.ShowError("Упакована неправильная тара! Нужна бутылка 0.5л.", stageName);
            return;
        }
        
        // Всё правильно – сохраняем и переходим
        Debug.Log("Упаковка пройдена успешно");
        
        if (Inventory.Instance != null)
            Inventory.Instance.SaveItems();
        
        if (uiManager != null)
            uiManager.ShowSuccess("Упаковка пройдена!", nextScene, stageName);
    }
    
    // Сброс этапа
    public void RestartStage()
    {
        Debug.Log("RestartStage вызван");
        
        isPacked = false;
        hasLabel = false;
        packedType = "";
        
        emptyBox.SetActive(true);
        boxWithBottle033.SetActive(false);
        boxWithBottle05.SetActive(false);
        boxWithBottle1.SetActive(false);
        boxWithCan033.SetActive(false);
        boxWithCan05.SetActive(false);
        sticker.SetActive(false);
        
        if (stickerButton != null) stickerButton.interactable = false;
        if (nextButton != null) nextButton.interactable = false;
    }
}