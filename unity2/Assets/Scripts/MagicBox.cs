using UnityEngine;

public class MagicBox : MonoBehaviour
{
    public ItemData[] items;           
    public int amountPerClick = 1;
    public Inventory inventoryReference;  

    private Inventory inventory;
    private int currentSlot = 0;

    void Start()
    {
        if (inventoryReference != null)
        {
            inventory = inventoryReference;
        }
        else
        {
            inventory = FindAnyObjectByType<Inventory>();
        }
        
        Debug.Log(inventory != null ? "✅ MagicBox: Inventory найден" : "❌ MagicBox: Inventory НЕ НАЙДЕН");
    }

    public void OnBoxClick()
    {
        if (Inventory.Instance  == null)
        {
            Debug.LogError("❌ Inventory.Instance  = null, добавление невозможно");
            return;
        }

        if (currentSlot >= items.Length)
        {
            Debug.Log("Все предметы уже добавлены!");
            return;
        }

        Inventory.Instance.AddItem(currentSlot, items[currentSlot], amountPerClick);
        Debug.Log($"Добавлен {items[currentSlot].itemName} в слот {currentSlot}");
        currentSlot++;
    }

    public void ResetBox()
    {
        currentSlot = 0;
        Debug.Log("MagicBox сброшен");
    }

    void OnEnable()
    {
        if (inventory == null && inventoryReference != null)
        {
            inventory = inventoryReference;
        }
        else if (inventory == null)
        {
            FindInventory();
        }
    }

    void FindInventory()
    {
        inventory = FindAnyObjectByType<Inventory>();
        
        if (inventory == null)
        {
            GameObject gm = GameObject.Find("GameManager");
            if (gm != null)
            {
                inventory = gm.GetComponent<Inventory>();
            }
        }
        
        if (inventory == null)
        {
            Debug.LogError("❌ MagicBox: Inventory НЕ НАЙДЕН! Перетащите GameManager в поле inventoryReference");
        }
        else
        {
            Debug.Log("✅ MagicBox: Inventory найден на объекте: " + inventory.gameObject.name);
        }
    }
}