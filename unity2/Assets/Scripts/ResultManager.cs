using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;

public class ResultManager : MonoBehaviour
{
    public TextMeshProUGUI titleText;       
    public TextMeshProUGUI errorListText;   

    private float weightStage1 = 0.15f;
    private float weightStage2 = 0.35f;
    private float weightStage3 = 0.35f;
    private float weightStage4 = 0.15f;
    private bool isResultSaved = false;

    void Start()
    {
        weightStage1 = PlayerPrefs.GetFloat("WeightStage1", 0.15f);
        weightStage2 = PlayerPrefs.GetFloat("WeightStage2", 0.35f);
        weightStage3 = PlayerPrefs.GetFloat("WeightStage3", 0.35f);
        weightStage4 = PlayerPrefs.GetFloat("WeightStage4", 0.15f);
        Debug.Log($"Веса этапов из БД: {weightStage1}, {weightStage2}, {weightStage3}, {weightStage4}");
        
        CalculateAndDisplayResults();
    }

    float ErrorsToScore(int errorCount)
    {
        if (errorCount == 0) return 100f;
        if (errorCount == 1) return 70f;
        if (errorCount == 2) return 40f;
        if (errorCount == 3) return 10f;
        return 0f;
    }

    void CalculateAndDisplayResults()
    {
        Debug.Log($"=== ErrorManager.Instance = {(ErrorManager.Instance != null ? "ЕСТЬ" : "NULL")} ===");
        
        int errorsStage1 = 0, errorsStage2 = 0, errorsStage3 = 0, errorsStage4 = 0;
        List<string> errorMessages = new List<string>();

        if (ErrorManager.Instance != null)
        {
            Debug.Log($"Количество ошибок в ErrorManager: {ErrorManager.Instance.errors.Count}");
            
            foreach (string err in ErrorManager.Instance.errors)
            {
                Debug.Log($"Ошибка из ErrorManager: {err}");
                
                if (err.Contains("Приёмка") || err.Contains("приёмка"))
                    errorsStage1++;
                else if (err.Contains("Замес") || err.Contains("замес"))
                    errorsStage2++;
                else if (err.Contains("Брожение") || err.Contains("брожение"))
                    errorsStage3++;
                else if (err.Contains("Упаковка") || err.Contains("упаковка"))
                    errorsStage4++;
                
                errorMessages.Add(err);
            }
        }
        else
        {
            Debug.LogError("ErrorManager.Instance = NULL! Ошибки не будут сохранены!");
        }

        Debug.Log($"errorsStage1={errorsStage1}, errorsStage2={errorsStage2}, errorsStage3={errorsStage3}, errorsStage4={errorsStage4}");
        Debug.Log($"errorMessages.Count = {errorMessages.Count}");

        float score1 = ErrorsToScore(errorsStage1);
        float score2 = ErrorsToScore(errorsStage2);
        float score3 = ErrorsToScore(errorsStage3);
        float score4 = ErrorsToScore(errorsStage4);

        float totalScore = weightStage1 * score1 + weightStage2 * score2 + 
                           weightStage3 * score3 + weightStage4 * score4;

        string grade;
        if (totalScore >= 90) grade = "Отлично";
        else if (totalScore >= 75) grade = "Хорошо";
        else if (totalScore >= 50) grade = "Удовлетворительно";
        else grade = "Брак";

        if (titleText != null)
            titleText.text = $"{grade} | {totalScore:F1} / 100";

        // ========= УЛУЧШЕННОЕ ОТОБРАЖЕНИЕ ОШИБОК =========
        if (errorListText != null)
        {
            string errorText;
            if (errorMessages.Count > 0)
            {
                List<string> uniqueErrors = new List<string>(new HashSet<string>(errorMessages));

                errorText = "<color=#FF6B6B>Допущены ошибки:</color>\n";
                for (int i = 0; i < uniqueErrors.Count; i++)
                {
                    errorText += $"  • {uniqueErrors[i]}\n";
                }
            }
            else
            {
                errorText = "<color=#4ECDC4>✓ Все этапы пройдены без ошибок</color>";
            }
            errorListText.text = errorText;
            Debug.Log($"errorListText обновлён");
        }
        else
        {
            Debug.LogError("errorListText = NULL!");
        }
        
        // ========= СОХРАНЕНИЕ ТОЛЬКО ОДИН РАЗ =========
        if (!isResultSaved)
        {
            SaveResultToServer(grade, totalScore, errorMessages);
            isResultSaved = true;
        }
    }
    
    void SaveResultToServer(string grade, float totalScore, List<string> errorMessages)
    {
        Debug.Log($"=== SAVE RESULT TO SERVER ===");
        
        if (ApiClient.Instance == null)
        {
            Debug.LogError("ApiClient.Instance = null! Невозможно сохранить результаты.");
            return;
        }
        
        Debug.Log($"ApiClient.Instance = есть");
        
        string sessionId = System.Guid.NewGuid().ToString();
        string gradeText = $"{grade} | {totalScore:F1} / 100";
        
        // ========= УБИРАЕМ ДУБЛИ И СКЛЕИВАЕМ ЧЕРЕЗ "; " =========
        List<string> uniqueErrors = new List<string>(new HashSet<string>(errorMessages));
        string errorReport = string.Join("; ", uniqueErrors);
        
        // Очистка от спецсимволов
        errorReport = Regex.Replace(errorReport, @"[^\w\s\.\,\-\!\?]", "");
        errorReport = errorReport.Replace("\"", "'");
        errorReport = errorReport.Replace("\n", " ").Replace("\r", " ");
        
        Debug.Log($"errorReport (очищенный): {errorReport}");
        
        StartCoroutine(ApiClient.Instance.SaveResult(
            sessionId, gradeText, errorReport, 1, 1,
            (success) => {
                if (success)
                    Debug.Log("Результаты сохранены на сервере");
                else
                    Debug.LogError("Ошибка сохранения результатов");
            }
        ));
    }

    public void RestartGame()
    {
        if (ErrorManager.Instance != null)
            ErrorManager.Instance.ClearErrors();
        
        isResultSaved = false;
        
        SceneManager.LoadScene("StartScen");
    }

    public void ExitToWebsite()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            Application.ExternalEval("window.location.href = '../website/index.html';");
        #else
            Debug.Log("Выход на сайт: ../website/index.html");
        #endif
    }
}