using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class FermentationManager : MonoBehaviour
{
    public UIManager uiManager;
    public Slider temperatureSlider;
    public Slider timeSlider;
    public TextMeshProUGUI tempValueText;
    public TextMeshProUGUI timeValueText;
    public string nextScene = "PackagingScene";
    public string stageName = "Брожение";

    private float tempMin = 12f;
    private float tempMax = 16f;
    private int timeMin = 5;
    private int timeMax = 7;

    void Start()
    {
        tempMin = PlayerPrefs.GetFloat("TempMin", 12f);
        tempMax = PlayerPrefs.GetFloat("TempMax", 16f);
        timeMin = PlayerPrefs.GetInt("TimeMin", 5);
        timeMax = PlayerPrefs.GetInt("TimeMax", 7);
        Debug.Log($"Параметры брожения из БД: {tempMin}-{tempMax}°C, {timeMin}-{timeMax} дней");
        
        Debug.Log("=== сцена брожения ===");
        
        if (uiManager == null)
            uiManager = FindAnyObjectByType<UIManager>();
        
        UIManager.Instance.FindPanelsOnCurrentScene();
        
        Transform panel = GameObject.Find("InventoryPanel")?.transform;
        if (panel != null && Inventory.Instance != null)
        {
            Inventory.Instance.SetInventoryPanel(panel);
            
            var saved = Inventory.Instance.savedItems;
            if (saved != null && saved.Count > 0)
            {
                Debug.Log($"Восстанавливаем {saved.Count} предметов");
                for (int i = 0; i < saved.Count && i < Inventory.Instance.items.Count; i++)
                {
                    if (saved[i].itemData != null)
                    {
                        Inventory.Instance.items[i].itemData = saved[i].itemData;
                        Inventory.Instance.items[i].count = saved[i].count;
                        Inventory.Instance.UpdateSlotUI(i);
                        Debug.Log($" Восстановлен {saved[i].itemData.itemName} в слот {i}");
                    }
                }
            }
        }
        
        UpdateTempDisplay();
        UpdateTimeDisplay();

        if (temperatureSlider != null)
            temperatureSlider.onValueChanged.AddListener((v) => UpdateTempDisplay());
        if (timeSlider != null)
            timeSlider.onValueChanged.AddListener((v) => UpdateTimeDisplay());
        
        Button nextButton = GameObject.Find("Button_ToPackaging")?.GetComponent<Button>();
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => CheckStage());
            Debug.Log("кнопка привязана к CheckStage");
        }
        else
        {
            Debug.LogError("кнопка 'Button_ToPackaging' не найдена");
        }
    }

    void UpdateTempDisplay()
    {
        if (tempValueText != null && temperatureSlider != null)
            tempValueText.text = temperatureSlider.value.ToString("F1") + "°C";
    }

    void UpdateTimeDisplay()
    {
        if (timeValueText != null && timeSlider != null)
            timeValueText.text = timeSlider.value.ToString("F0") + " дн.";
    }

    public void CheckStage()
    {
        Debug.Log("CheckStage вызван");
        
        if (temperatureSlider == null || timeSlider == null)
        {
            Debug.LogError("Слайдеры не назначены!");
            return;
        }
        
        float temp = temperatureSlider.value;
        int time = (int)timeSlider.value;
        
        bool tempOk = (temp >= tempMin && temp <= tempMax);
        bool timeOk = (time >= timeMin && time <= timeMax);
        
        if (!tempOk || !timeOk)
        {
            string error = "";
            if (!tempOk) error += $"Температура {temp}°C (норма {tempMin}-{tempMax}). ";
            if (!timeOk) error += $"Время {time} дн. (норма {timeMin}-{timeMax}). ";
            
            if (ErrorManager.Instance != null)
                ErrorManager.Instance.AddError($"Брожение: {error}");
            
            Debug.Log($"Ошибка брожения: {error}");
            
            if (uiManager != null)
                uiManager.ShowError($"Ошибка брожения: {error}", stageName);
        }
        else
        {
            Debug.Log("Параметры брожения в норме");
            
            // ========= ИСПРАВЛЕНО: вызов с задержкой =========
            StartCoroutine(ShowSuccessWithDelay());
            // ================================================
        }
        
        if (Inventory.Instance != null)
            Inventory.Instance.SaveItems();
    }
    
    IEnumerator ShowSuccessWithDelay()
    {
        // Ждём 0.1 секунды, чтобы UIManager успел найти панели
        yield return new WaitForSeconds(0.1f);
        
        if (uiManager != null)
            uiManager.ShowSuccess("Брожение пройдено!", nextScene, stageName);
    }
}