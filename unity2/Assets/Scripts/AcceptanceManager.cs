using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;  

public class AcceptanceManager : MonoBehaviour
{
    public UIManager uiManager;
    public Inventory inventory;
    public string nextScene = "MixingScen";
    public string stageName = "Приёмка";
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
    
    // привязываем кнопку в коде
    Button continueButton = GameObject.Find("ContinueButton")?.GetComponent<Button>();
    if (continueButton != null)
    {
        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => CheckStage());
        Debug.Log("кнопка continue привязана к checkstage");
    }
    
    Debug.Log($"AcceptanceManager: uiManager = {(uiManager != null ? "найден" : "не найден")}");
    Debug.Log($"AcceptanceManager: inventory = {(inventory != null ? "найден" : "не найден")}");
}

    public void CheckStage()
{
    Debug.Log(" CheckStage ВЫЗВАН!");

    if (uiManager == null)
    {
        uiManager = FindAnyObjectByType<UIManager>();
        Debug.Log(uiManager != null ? "UIManager найден автоматически" : " UIManager НЕ НАЙДЕН");
    }

    if (inventory == null)
    {
        inventory = Inventory.Instance;
        if (inventory == null)
            inventory = FindAnyObjectByType<Inventory>();
        Debug.Log(inventory != null ? "Inventory найден" : "Inventory НЕ НАЙДЕН");
    }

    if (uiManager == null)
    {
        Debug.LogError("uiManager = null невозможно показать окно.");
        return;
    }

    if (inventory == null)
    {
        Debug.LogError("inventory = null!");
        return;
    }

    bool allGood = true;
    string errorMsg = "";

    for (int i = 0; i < 9; i++)
    {
        Debug.Log($"Проверка слота {i}: itemData = {(inventory.items[i].itemData != null ? inventory.items[i].itemData.itemName : "null")}, count = {inventory.items[i].count}");
        
        if (inventory.items[i].itemData == null || inventory.items[i].count == 0)
        {
            allGood = false;
            errorMsg += $"Не хватает ингредиента в слоте {i}. ";
        }
    }

    Debug.Log($"allGood = {allGood}, errorMsg = {errorMsg}");
    if (allGood)
    {
        uiManager.ShowSuccess("Приёмка пройдена!", nextScene, stageName);
    }
    else
    {
        uiManager.ShowError("Не все ингредиенты приняты. Проверьте слоты.", stageName);
    }
}
public void NextScene()
{
    Debug.Log("=== NextScene() ВЫЗВАН НА ПРИЁМКЕ ===");
    
    if (Inventory.Instance == null)
    {
        Debug.LogError("Inventory.Instance = null!");
        return;
    }
    
    for (int i = 0; i < Inventory.Instance.items.Count; i++)
    {
        var item = Inventory.Instance.items[i];
        Debug.Log($"Слот {i}: {(item.itemData != null ? item.itemData.itemName : "пусто")} x{item.count}");
    }
    
    Inventory.Instance.SaveItems();
    
    Debug.Log($"После SaveItems: savedItems.Count = {Inventory.Instance.savedItems.Count}");
    
    SceneManager.LoadScene("2");
}
}