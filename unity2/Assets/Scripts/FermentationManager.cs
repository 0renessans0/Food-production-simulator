using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class FermentationManager : MonoBehaviour
{
    public UIManager uiManager;
    public Slider temperatureSlider;
    public Slider timeSlider;
    public TextMeshProUGUI tempValueText;
    public TextMeshProUGUI timeValueText;
    public string nextScene = "PackagingScene";
    public string stageName = "Брожение";

    // Правила из БД (потом замените на реальные данные)
    private float tempMin = 12f;
    private float tempMax = 16f;
    private int timeMin = 5;
    private int timeMax = 7;

    void Start()
    {
        // Обновляем отображение значений
        UpdateTempDisplay();
        UpdateTimeDisplay();

        // Слушаем изменения слайдеров
        temperatureSlider.onValueChanged.AddListener((v) => UpdateTempDisplay());
        timeSlider.onValueChanged.AddListener((v) => UpdateTimeDisplay());
    }

    void UpdateTempDisplay()
    {
        if (tempValueText != null)
            tempValueText.text = temperatureSlider.value.ToString("F1") + "°C";
    }

    void UpdateTimeDisplay()
    {
        if (timeValueText != null)
            timeValueText.text = timeSlider.value.ToString("F0") + " дней";
    }

    public void CheckStage()
    {
        float temp = temperatureSlider.value;
        int time = (int)timeSlider.value;

        bool tempOk = (temp >= tempMin && temp <= tempMax);
        bool timeOk = (time >= timeMin && time <= timeMax);

        if (tempOk && timeOk)
        {
            uiManager.ShowSuccess("✅ Брожение пройдено!", nextScene, stageName);
        }
        else
        {
            string error = "";
            if (!tempOk) error += $"Температура {temp}°C (нужно {tempMin}-{tempMax}). ";
            if (!timeOk) error += $"Время {time} дней (нужно {timeMin}-{timeMax}). ";
            uiManager.ShowError($"❌ Ошибка: {error}", stageName);
        }
    }
}