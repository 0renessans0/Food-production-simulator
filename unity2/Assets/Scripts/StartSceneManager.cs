using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class StartSceneManager : MonoBehaviour
{
    [Header("Кнопки")]
    public GameObject entryButton;   // кнопка "Играть"
    public GameObject exitButton;    // кнопка "Выйти на сайт"
    
    [Header("Загрузка данных")]
    public bool loadDataOnStart = true;
    
    private bool dataLoaded = false;
    
    void Start()
    {
        // Привязываем кнопку "Играть"
        if (entryButton != null)
        {
            var btn = entryButton.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(StartGame);
                Debug.Log("Кнопка EntryButton привязана к StartGame");
            }
            else
            {
                Debug.LogError("На EntryButton нет компонента Button!");
            }
        }
        else
        {
            Debug.LogError("EntryButton не назначен в инспекторе!");
        }
        
        // Привязываем кнопку "Выйти на сайт"
        if (exitButton != null)
        {
            var btn = exitButton.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(ExitToWebsite);
                Debug.Log("Кнопка ExitButton привязана к ExitToWebsite");
            }
            else
            {
                Debug.LogError("На ExitButton нет компонента Button!");
            }
        }
        else
        {
            Debug.LogError("ExitButton не назначен в инспекторе!");
        }
        
        // Загружаем данные с сервера
        if (loadDataOnStart)
        {
            StartCoroutine(LoadGameData());
        }
        else
        {
            dataLoaded = true;
        }
    }
    
    // Загрузка данных с сервера
    IEnumerator LoadGameData()
    {
        Debug.Log("Начинаем загрузку данных с сервера...");
        
        yield return StartCoroutine(LoadRegulations());
        yield return StartCoroutine(LoadGameRules());
        
        dataLoaded = true;
        Debug.Log("Все данные загружены!");
    }
    
    IEnumerator LoadRegulations()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://localhost:3001/api/regulations"))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log($"Регламент загружен");
                
                RegulationsData data = JsonUtility.FromJson<RegulationsData>(json);
                
                PlayerPrefs.SetFloat("TempMin", data.temp_min);
                PlayerPrefs.SetFloat("TempMax", data.temp_max);
                PlayerPrefs.SetInt("TimeMin", data.time_min);
                PlayerPrefs.SetInt("TimeMax", data.time_max);
                
                if (data.allowed_packaging != null && data.allowed_packaging.Length > 0)
                {
                    string packaging = string.Join(",", data.allowed_packaging);
                    PlayerPrefs.SetString("AllowedPackaging", packaging);
                    
                    string correctPackaging = "стекло_0.5";
                    foreach (string p in data.allowed_packaging)
                    {
                        if (p.Contains("0.5") && p.Contains("стекло"))
                            correctPackaging = p;
                    }
                    PlayerPrefs.SetString("CorrectPackaging", correctPackaging);
                }
                
                PlayerPrefs.Save();
                Debug.Log($"Параметры сохранены: температура {data.temp_min}-{data.temp_max}°C, время {data.time_min}-{data.time_max} дней");
            }
            else
            {
                Debug.LogError($"Ошибка загрузки регламента: {request.error}");
            }
        }
    }
    
    IEnumerator LoadGameRules()
    {
        using (UnityWebRequest request = UnityWebRequest.Get("http://localhost:3001/api/rules"))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                Debug.Log($"Правила загружены");
                
                GameRulesData data = JsonUtility.FromJson<GameRulesData>(json);
                
                PlayerPrefs.SetFloat("WeightStage1", data.weight_stage1);
                PlayerPrefs.SetFloat("WeightStage2", data.weight_stage2);
                PlayerPrefs.SetFloat("WeightStage3", data.weight_stage3);
                PlayerPrefs.SetFloat("WeightStage4", data.weight_stage4);
                PlayerPrefs.SetFloat("ThresholdGood", data.threshold_good);
                PlayerPrefs.SetFloat("ThresholdSatisfactory", data.threshold_satisfactory);
                PlayerPrefs.Save();
                
                Debug.Log($"Правила сохранены: веса {data.weight_stage1}, {data.weight_stage2}, {data.weight_stage3}, {data.weight_stage4}");
            }
            else
            {
                Debug.LogError($"Ошибка загрузки правил: {request.error}");
            }
        }
    }
    
    // Запуск игры (кнопка EntryButton)
    public void StartGame()
    {
        Debug.Log("Запуск игры...");
        
        if (!dataLoaded && loadDataOnStart)
        {
            Debug.Log("Данные ещё не загружены, ждём...");
            StartCoroutine(WaitForDataAndStart());
        }
        else
        {
            SceneManager.LoadScene("AcceptenceScen");
        }
    }
    
    IEnumerator WaitForDataAndStart()
    {
        yield return new WaitForSeconds(0.5f);
        while (!dataLoaded)
        {
            yield return null;
        }
        SceneManager.LoadScene("AcceptenceScen");
    }
    
    // Выход на сайт (кнопка ExitButton)
    public void ExitToWebsite()
    {
        Debug.Log("Выход на сайт...");
        
        #if UNITY_WEBGL && !UNITY_EDITOR
            Application.ExternalEval("window.location.href = '../website/index.html';");
        #else
            Debug.Log("Возврат на сайт: ../website/index.html");
        #endif
    }
    
    // Классы для парсинга JSON
    [System.Serializable]
    public class RegulationsData
    {
        public int id;
        public string recipe_name;
        public string[] allowed_ingredients;
        public string[] allowed_packaging;
        public float temp_min;
        public float temp_max;
        public int time_min;
        public int time_max;
        public bool label_required;
    }
    
    [System.Serializable]
    public class GameRulesData
    {
        public int id;
        public string rule_name;
        public float weight_stage1;
        public float weight_stage2;
        public float weight_stage3;
        public float weight_stage4;
        public float threshold_good;
        public float threshold_satisfactory;
    }
}