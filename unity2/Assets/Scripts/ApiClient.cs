using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ApiClient : MonoBehaviour
{
    public static ApiClient Instance;
    private const string BASE_URL = "http://localhost:3001";
    
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
    
    // Сохранение результатов игры
    public IEnumerator SaveResult(string sessionId, string grade, string errorReport, 
        int regulationId, int rulesId, System.Action<bool> onComplete)
    {
        ResultData result = new ResultData
        {
            session_id = sessionId,
            regulation_id = regulationId,
            rules_id = rulesId,
            grade = grade,
            error_report = errorReport
        };
        
        string json = JsonUtility.ToJson(result);
        
        using (UnityWebRequest request = new UnityWebRequest(BASE_URL + "/api/results", "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Результаты сохранены на сервере");
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError($"Ошибка сохранения: {request.error}");
                onComplete?.Invoke(false);
            }
        }
    }
    
    // Получение регламента (если нужно)
    public IEnumerator GetRegulations(System.Action<RegulationsData> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(BASE_URL + "/api/regulations"))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                RegulationsData data = JsonUtility.FromJson<RegulationsData>(json);
                onSuccess?.Invoke(data);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
    
    // Получение правил игры (если нужно)
    public IEnumerator GetGameRules(System.Action<GameRulesData> onSuccess, System.Action<string> onError)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(BASE_URL + "/api/rules"))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                GameRulesData data = JsonUtility.FromJson<GameRulesData>(json);
                onSuccess?.Invoke(data);
            }
            else
            {
                onError?.Invoke(request.error);
            }
        }
    }
    
    // Классы для парсинга JSON
    [System.Serializable]
    public class RegulationsData
    {
        public int id;
        public string recipe_name;
        public float temp_min;
        public float temp_max;
        public int time_min;
        public int time_max;
        public string[] allowed_packaging;
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
    }
    
    [System.Serializable]
    public class ResultData
    {
        public string session_id;
        public int regulation_id;
        public int rules_id;
        public string grade;
        public string error_report;
    }
}