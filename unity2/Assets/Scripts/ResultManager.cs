using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    public TextMeshProUGUI titleText;       
    public TextMeshProUGUI errorListText;   

    private float weightStage1 = 0.15f;
    private float weightStage2 = 0.35f;
    private float weightStage3 = 0.35f;
    private float weightStage4 = 0.15f;

    void Start()
    {
        // Загружаем веса этапов из БД
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
        int errorsStage1 = 0, errorsStage2 = 0, errorsStage3 = 0, errorsStage4 = 0;
        List<string> errorMessages = new List<string>();

        if (ErrorManager.Instance != null)
        {
            foreach (string err in ErrorManager.Instance.errors)
            {
                if (err.Contains("Приёмка")) errorsStage1++;
                else if (err.Contains("Замес")) errorsStage2++;
                else if (err.Contains("Брожение")) errorsStage3++;
                else if (err.Contains("Упаковка")) errorsStage4++;
                errorMessages.Add(err);
            }
        }

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

        if (errorListText != null)
        {
            string errorText = errorMessages.Count > 0 ? string.Join("\n", errorMessages) : "Все этапы пройдены без ошибок";
            errorListText.text = errorText;
        }
        
        // Сохраняем результаты на сервер
        SaveResultToServer(grade, totalScore, errorMessages);
    }
    
    void SaveResultToServer(string grade, float totalScore, List<string> errorMessages)
    {
        string sessionId = System.Guid.NewGuid().ToString();
        string gradeText = $"{grade} | {totalScore:F1} / 100";
        string errorReport = string.Join("\n", errorMessages);
        
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
            ErrorManager.Instance.errors.Clear();
        
        SceneManager.LoadScene("StartScan"); // ← название твоей стартовой сцены
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