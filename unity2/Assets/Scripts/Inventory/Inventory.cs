using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public ItemData data;
    public List<ItemInventory> items = new List<ItemInventory>();
    public GameObject slotPrefab;
    public Transform inventoryPanel;
    public int maxCount = 9;

    public static Inventory Instance;

    private List<Image> slotIcons = new List<Image>();
    private List<Text> slotNameTexts = new List<Text>();  // ← теперь для названий
    private List<ItemInventory> savedItems = new List<ItemInventory>();
    private int selectedSlot = -1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (inventoryPanel != null && slotPrefab != null)
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
        
        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }
        
        items.Clear();
        slotIcons.Clear();
        slotNameTexts.Clear();
        
        AddGraphics();
        
        for (int i = 0; i < savedItems.Count && i < maxCount; i++)
        {
            if (savedItems[i].itemData != null)
            {
                items[i].itemData = savedItems[i].itemData;
                items[i].count = savedItems[i].count;
                UpdateSlotUI(i);
            }
        }
    }

    public void SaveItems()
    {
        Debug.Log($"Сохраняем {items.Count} предметов");
        savedItems.Clear();
        foreach (var item in items)
        {
            var copy = new ItemInventory();
            copy.itemData = item.itemData;
            copy.count = item.count;
            savedItems.Add(copy);
            Debug.Log($"Сохранён: {copy.itemData?.itemName}");
        }
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
        UpdateSlotUI(slotIndex);
    }

    void AddGraphics()
    {
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

        foreach (Transform child in inventoryPanel)
        {
            Destroy(child.gameObject);
        }
        
        items.Clear();
        slotIcons.Clear();
        slotNameTexts.Clear();

        for (int i = 0; i < maxCount; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, inventoryPanel);
            newSlot.name = i.ToString();

            ItemInventory ii = new ItemInventory();
            ii.slotGameObject = newSlot;
            ii.itemData = null;
            ii.count = 0;
            ii.id = i;

            Image icon = newSlot.transform.Find("Icon")?.GetComponent<Image>();
            Text nameText = newSlot.transform.Find("NameText")?.GetComponent<Text>();
            
            slotIcons.Add(icon);
            slotNameTexts.Add(nameText);
            items.Add(ii);
            
            if (newSlot.GetComponent<InventorySlotUI>() == null)
            {
                var slotUI = newSlot.AddComponent<InventorySlotUI>();
                slotUI.SetSlotIndex(i);
            }
            
            UpdateSlotUI(i);
        }
    }

    public void SelectSlot(int slotIndex)
    {
        selectedSlot = slotIndex;
        Debug.Log($"Выбран слот {slotIndex}, предмет: {items[slotIndex].itemData?.itemName}");
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
}

[System.Serializable]
public class ItemInventory
{
    public int id;
    public GameObject slotGameObject;
    public ItemData itemData;
    public int count;   // оставлен, но не используется в UI
}