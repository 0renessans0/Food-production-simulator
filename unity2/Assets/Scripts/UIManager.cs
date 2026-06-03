using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using TMPro;  

public class UIManager : MonoBehaviour
{
    public GameObject successPanel;
    public GameObject errorPanel;
    public TMP_Text successText;
    public TMP_Text errorText;

    private string nextSceneName;
    private string currentStageName;
    private string errorStageName;


    
    void Start()
    {
        FindPanels();
        
        if (successPanel != null)
            successPanel.SetActive(false);
        else
            Debug.LogError("SuccessPanel не найден на сцене");
        
        if (errorPanel != null)
            errorPanel.SetActive(false);
        else
            Debug.LogError("ErrorPanel не найден на сцене");
    }

    void OnEnable()
    {
        FindPanels();
    }

    void FindPanels()
{
    Debug.Log("FindPanels() вызван");
    
    if (successPanel == null)
    {
        successPanel = GameObject.Find("SuccessPanel");
        Debug.Log($"Поиск SuccessPanel: {(successPanel != null ? "найден" : "НЕ НАЙДЕН")}");
        if (successPanel != null)
            successText = successPanel.GetComponentInChildren<TMP_Text>();
    }
    
    if (errorPanel == null)
    {
        errorPanel = GameObject.Find("ErrorPanel");
        Debug.Log($"Поиск ErrorPanel: {(errorPanel != null ? "найден" : "НЕ НАЙДЕН")}");
        if (errorPanel != null)
            errorText = errorPanel.GetComponentInChildren<TMP_Text>();
    }
    
   GameObject[] allObjects = FindObjectsByType<GameObject>();
    Debug.Log("все объекты на сцене:");
    foreach (var obj in allObjects)
    {
        Debug.Log($" - {obj.name}");
    }
}

public void ShowError(string message, string stageName)
{
    if (errorPanel == null)
        errorPanel = GameObject.Find("ErrorPanel");
    
    if (errorText == null && errorPanel != null)
        errorText = errorPanel.GetComponentInChildren<TMP_Text>();
    
    if (errorPanel == null)
    {
        Debug.LogError("errorPanel = null создать ErrorPanel");
        return;
    }
    
    errorText.text = message;
    errorPanel.SetActive(true);
    Debug.Log($"ошибка на этапе {stageName}: {message}");
}

public void ShowSuccess(string message, string nextScene, string stageName)
{
    if (successPanel == null)
        successPanel = GameObject.Find("SuccessPanel");
    
    if (successText == null && successPanel != null)
        successText = successPanel.GetComponentInChildren<TMP_Text>();
    
    if (successPanel == null)
    {
        Debug.LogError("successPanel = null создать SuccessPanel");
        return;
    }
    
    successText.text = message;
    nextSceneName = nextScene;
    successPanel.SetActive(true);
    Debug.Log($"этап {stageName} пройден");
}

     public void OnSuccessNext()
    {
        Debug.Log("OnSuccessNext() вызван, сохраняем инвентарь");
        
        if (Inventory.Instance != null)
        {
            Inventory.Instance.SaveItems();
            Debug.Log($"Сохранено предметов: {Inventory.Instance.savedItems.Count}");
        }
        
        successPanel.SetActive(false);
        
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("nextSceneName не задан!");
        }
    }

    public void OnErrorRestart()
{
    errorPanel.SetActive(false);
    
    for (int i = 0; i < 9; i++)
    {
        Inventory.Instance.ClearSlot(i);
    }
    
    MagicBox box = FindAnyObjectByType<MagicBox>();
    if (box != null) box.ResetBox();
}

    public void OnErrorCancel()
    {
        errorPanel.SetActive(false);
    }
}