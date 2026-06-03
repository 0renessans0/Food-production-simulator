using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public class Inventory : MonoBehaviour
{
    public ItemData data;
    public List<ItemInventory> items = new List<ItemInventory>();
    public List<ItemInventory> savedItems = new List<ItemInventory>();
    public GameObject slotPrefab;
    public Transform inventoryPanel;
    public int maxCount = 9;
    public static Inventory Instance;

    private bool isInitialized = false;
    private List<Image> slotIcons = new List<Image>();
    private List<Text> slotNameTexts = new List<Text>();
    private int selectedSlot = -1;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Сцена {scene.name} загружена, обновляем UI");
        
        GameObject panel = GameObject.Find("InventoryPanel");
        if (panel != null)
        {
            inventoryPanel = panel.transform;
            RefreshUI();
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("Inventory.Instance создан");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {

        if (isInitialized) return;
        isInitialized = true;
        if (items.Count == 0)
        {
            for (int i = 0; i < maxCount; i++)
            {
                items.Add(new ItemInventory());
            }
        }
        
        if (inventoryPanel != null && slotPrefab != null && items.Count > 0 && items[0].slotGameObject == null)
        {
            AddGraphics();
        }
    }

    public void SetInventoryPanel(Transform newPanel)
    {
        Debug.Log($"SetInventoryPanel вызван, сохранено предметов: {savedItems.Count}");
        
        inventoryPanel = newPanel;
        
        if (inventoryPanel == null)
        {
            Debug.LogError("inventoryPanel = null!");
            return;
        }
        
        // Восстанавливаем данные из savedItems в существующие items
        for (int i = 0; i < savedItems.Count && i < items.Count; i++)
        {
            if (savedItems[i].itemData != null)
            {
                items[i].itemData = savedItems[i].itemData;
                items[i].count = savedItems[i].count;
                Debug.Log($"Восстановлен слот {i}: {savedItems[i].itemData.itemName}");
            }
        }
        
        // Пересоздаём UI слоты
        if (items[0].slotGameObject == null)
        {
            AddGraphics();
        }
        else
        {
            // Обновляем UI
            for (int i = 0; i < items.Count && i < inventoryPanel.childCount; i++)
            {
                Transform child = inventoryPanel.GetChild(i);
                if (child != null)
                    items[i].slotGameObject = child.gameObject;
                UpdateSlotUI(i);
            }
        }
    }

    public void SaveItems()
    {
        savedItems.Clear();
        for (int i = 0; i < items.Count; i++)
        {
            var copy = new ItemInventory();
            copy.itemData = items[i].itemData;
            copy.count = items[i].count;
            savedItems.Add(copy);
            Debug.Log($"Сохранён слот {i}: {(copy.itemData != null ? copy.itemData.itemName : "пусто")} x{copy.count}");
        }
        Debug.Log($"Всего сохранено: {savedItems.Count}");
    }

    public List<ItemInventory> GetSavedItems()
    {
        return savedItems;
    }

    public void AddItem(int slotIndex, ItemData item, int count)
    {
        if (slotIndex >= items.Count) return;
        items[slotIndex].itemData = item;
        items[slotIndex].count = count;
        UpdateSlotUI(slotIndex);
    }

    public void ClearSlot(int slotIndex)
    {
        if (slotIndex >= items.Count) return;
        items[slotIndex].itemData = null;
        items[slotIndex].count = 0;
        if (items[slotIndex].slotGameObject != null)
        {
            UpdateSlotUI(slotIndex);
        }
    }

    void AddGraphics()
{
    // Проверяем, нужно ли создавать слоты
    if (items.Count > 0 && items[0].slotGameObject != null)
    {
        Debug.Log("Слоты уже созданы, пропускаем AddGraphics()");
        return;
    }
    
    Debug.Log("AddGraphics() начал работу, создаю слоты...");

    if (inventoryPanel == null)
    {
        Debug.LogError("inventoryPanel не назначен!");
        return;
    }
    
    if (slotPrefab == null)
    {
        Debug.LogError("slotPrefab не назначен!");
        return;
    }

    // Полная очистка старых слотов
    foreach (Transform child in inventoryPanel)
    {
        if (child != null)
            DestroyImmediate(child.gameObject);
    }
    
    // Очищаем все списки
    items.Clear();
    slotIcons.Clear();
    slotNameTexts.Clear();

    // Создаём новые слоты
    for (int i = 0; i < maxCount; i++)
    {
        GameObject newSlot = Instantiate(slotPrefab, inventoryPanel);
        newSlot.name = i.ToString(); 
        Debug.Log($"Создан слот с именем {newSlot.name}");

        ItemInventory ii = new ItemInventory();
        ii.slotGameObject = newSlot;
        ii.itemData = null;
        ii.count = 0;
        ii.id = i;
        items.Add(ii);

        Image icon = newSlot.transform.Find("Icon")?.GetComponent<Image>();
        Text nameText = newSlot.transform.Find("NameText")?.GetComponent<Text>();
        
        slotIcons.Add(icon);
        slotNameTexts.Add(nameText);
        
        InventorySlotUI slotUI = newSlot.GetComponent<InventorySlotUI>();
        if (slotUI == null)
        {
            slotUI = newSlot.AddComponent<InventorySlotUI>();
        }
        slotUI.SetSlotIndex(i);
        
        UpdateSlotUI(i);
    }
    
    Debug.Log($"AddGraphics() завершён, создано {items.Count} слотов");
}

    public void SelectSlot(int slotIndex)
{
    if (slotIndex == -1)
    {
        selectedSlot = -1;
        Debug.Log("слот сброшен");
        return;
    }
    
    if (slotIndex < 0 || slotIndex >= items.Count)
    {
        Debug.LogError($"индекс {slotIndex} вне диапазона (0-{items.Count - 1})");
        return;
    }
    
    selectedSlot = slotIndex;
    Debug.Log($"выбран слот {slotIndex}, предмет: {items[slotIndex].itemData?.itemName}");
}

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }

    public void UpdateSlotUI(int index)
    {
        if (index >= items.Count) return;
        
        var slot = items[index].slotGameObject;
        if (slot == null) return;
        
        Transform iconTransform = slot.transform.Find("Icon");
        Transform nameTransform = slot.transform.Find("NameText");
        
        if (iconTransform != null)
        {
            Image icon = iconTransform.GetComponent<Image>();
            if (icon != null)
            {
                if (items[index].itemData != null)
                {
                    icon.sprite = items[index].itemData.icon;
                    icon.enabled = true;
                }
                else
                {
                    icon.sprite = null;
                    icon.enabled = false;
                }
            }
        }
        
        if (nameTransform != null)
        {
            Text nameText = nameTransform.GetComponent<Text>();
            if (nameText != null)
            {
                if (items[index].itemData != null)
                {
                    nameText.text = items[index].itemData.itemName;
                    nameText.enabled = true;
                }
                else
                {
                    nameText.text = "";
                    nameText.enabled = false;
                }
            }
        }
    }
    
    public void RefreshUI()
    {
        if (inventoryPanel == null)
        {
            Debug.LogError("inventoryPanel = null, невозможно пересоздать слоты");
            return;
        }
        
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }
        
        slotIcons.Clear();
        slotNameTexts.Clear();
        
        AddGraphics();
    }
    
    public void ResetInventoryUI()
    {
        Debug.Log("ResetInventoryUI: очистка и пересоздание слотов");
        
        slotIcons.Clear();
        slotNameTexts.Clear();
        
        if (inventoryPanel != null)
        {
            foreach (Transform child in inventoryPanel)
            {
                DestroyImmediate(child.gameObject);
            }
        }
        
        AddGraphics();
        
        MagicBox magicBox = FindAnyObjectByType<MagicBox>();
        if (magicBox != null)
        {
            magicBox.ResetBox();
        }
        
        Debug.Log($"ResetInventoryUI завершён, создано {items.Count} слотов");
    }
}

[System.Serializable]
public class ItemInventory
{
    public int id;
    public GameObject slotGameObject;
    public ItemData itemData;
    public int count;
}