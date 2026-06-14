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
    private string expectedPackedType = "Bottle05"; // правильный тип (из БД)
    
    void Start()
    {
        // ========= ЗАГРУЗКА ПРАВИЛЬНОЙ ТАРЫ ИЗ БД =========
        string correctPackagingFromDB = PlayerPrefs.GetString("CorrectPackaging", "стекло_0.5");
        Debug.Log($"Правильная тара из БД: {correctPackagingFromDB}");
        
        // Сопоставляем строку из БД с типом в игре
        switch (correctPackagingFromDB)
        {
            case "стекло_0.33":
                expectedPackedType = "Bottle033";
                break;
            case "стекло_0.5":
                expectedPackedType = "Bottle05";
                break;
            case "стекло_1":
                expectedPackedType = "Bottle1";
                break;
            case "металл_0.33":
                expectedPackedType = "Can033";
                break;
            case "металл_0.5":
                expectedPackedType = "Can05";
                break;
            default:
                expectedPackedType = "Bottle05";
                break;
        }
        Debug.Log($"Ожидаемый тип упаковки: {expectedPackedType}");
        // ==================================================
        
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
        
        // Кнопки всегда активны
        if (stickerButton != null) stickerButton.interactable = true;
        if (nextButton != null) nextButton.interactable = true;
        
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
        
        // Восстанавливаем инвентарь
        Transform panel = GameObject.Find("InventoryPanel")?.transform;
        if (panel != null && Inventory.Instance != null)
        {
            Inventory.Instance.SetInventoryPanel(panel);
        }
        
        Debug.Log("PackagingManager Start: inventory = " + (inventory != null ? "найден" : "null"));
    }
    
    // Упаковка выбранной тары
    public void OnBoxClick()
    {
        Debug.Log("OnBoxClick вызван");
        
        if (inventory == null)
        {
            inventory = FindAnyObjectByType<Inventory>();
            if (inventory == null)
            {
                Debug.LogError("Inventory не найден!");
                return;
            }
        }
        
        if (isPacked)
        {
            Debug.Log("Уже упаковано");
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
        
        // Показываем соответствующий ящик
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
        
        inventory.ClearSlot(selectedSlot);
        inventory.SelectSlot(-1);
        
        isPacked = true;
        
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
    
    // Проверка этапа
    public void CheckStage()
    {
        Debug.Log("CheckStage вызван");
        
        // Проверка 1: упаковано ли?
        if (!isPacked)
        {
            if (ErrorManager.Instance != null)
                ErrorManager.Instance.AddError("Упаковка: попытка завершить без упаковки");
            if (uiManager != null)
                uiManager.ShowError("Сначала упакуйте товар!", stageName);
            return;
        }
        
        // Проверка 2: наклеена ли этикетка?
        if (!hasLabel)
        {
            if (ErrorManager.Instance != null)
                ErrorManager.Instance.AddError("Упаковка: попытка завершить без этикетки");
            if (uiManager != null)
                uiManager.ShowError("Сначала наклейте этикетку!", stageName);
            return;
        }
        
        // Проверка 3: правильная ли тара? (используем данные из БД)
        if (packedType != expectedPackedType)
        {
            if (ErrorManager.Instance != null)
                ErrorManager.Instance.AddError($"Упаковка: выбрана неверная тара ({packedType}). Нужна {expectedPackedType}");
            if (uiManager != null)
                uiManager.ShowError($"Упакована неправильная тара! Нужна бутылка 0.5л.", stageName);
            return;
        }
        
        // Всё правильно
        Debug.Log("Упаковка пройдена успешно");
        
        if (Inventory.Instance != null)
            Inventory.Instance.SaveItems();
        
        if (uiManager != null)
            uiManager.ShowSuccess("Упаковка пройдена!", nextScene, stageName);
    }
    
    public void NextScene()
    {
        Debug.Log($"=== NextScene() ВЫЗВАН НА ЭТАПЕ {stageName} ===");
        
        if (Inventory.Instance != null)
            Inventory.Instance.SaveItems();
        
        SceneManager.LoadScene(nextScene);
    }
    
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
        
        Debug.Log("Этап упаковки перезапущен");
    }
}