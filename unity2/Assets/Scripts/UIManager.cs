using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public GameObject successPanel;
    public GameObject errorPanel;
    public TMP_Text successText;
    public TMP_Text errorText;

    private string nextSceneName;

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
        StartCoroutine(FindPanelsWithDelay());
    }

    System.Collections.IEnumerator FindPanelsWithDelay()
    {
        yield return null;
        FindPanelsOnCurrentScene();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void FindPanelsOnCurrentScene()
    {
        Debug.Log($"поиск панелей на сцене {SceneManager.GetActiveScene().name}");
        
        Canvas[] canvases = FindObjectsByType<Canvas>();
        successPanel = null;
        errorPanel = null;
        
        foreach (Canvas canvas in canvases)
        {
            foreach (Transform child in canvas.transform)
            {
                if (child.name == "SuccessPanel")
                {
                    successPanel = child.gameObject;
                    Debug.Log("success panel найден");
                    
                    Button nextButton = successPanel.transform.Find("NextButton")?.GetComponent<Button>();
                    if (nextButton != null)
                    {
                        nextButton.onClick.RemoveAllListeners();
                        nextButton.onClick.AddListener(() => OnSuccessNext());
                        Debug.Log("кнопка next привязана");
                    }
                }
                
                if (child.name == "ErrorPanel")
                {
                    errorPanel = child.gameObject;
                    Debug.Log("error panel найден");
                    
                    Button restartButton = errorPanel.transform.Find("RestartButton")?.GetComponent<Button>();
                    if (restartButton != null)
                    {
                        restartButton.onClick.RemoveAllListeners();
                        restartButton.onClick.AddListener(() => OnErrorRestart());
                        Debug.Log("кнопка restart привязана");
                    }
                }
            }
        }
        
        if (successPanel != null)
        {
            successText = successPanel.GetComponentInChildren<TMP_Text>();
            successPanel.SetActive(false);
        }
        
        if (errorPanel != null)
        {
            errorText = errorPanel.GetComponentInChildren<TMP_Text>();
            errorPanel.SetActive(false);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"сцена {scene.name} загружена");
        FindPanelsOnCurrentScene();
    }

    public void ShowSuccess(string message, string nextScene, string stageName)
    {
        if (successPanel == null)
        {
            successPanel = GameObject.Find("SuccessPanel");
            if (successPanel != null)
                successText = successPanel.GetComponentInChildren<TMP_Text>();
        }
        
        if (successPanel == null)
        {
            Debug.LogError($"success panel не найден на сцене {stageName}");
            return;
        }
        
        successText.text = message;
        nextSceneName = nextScene;
        successPanel.SetActive(true);
        Debug.Log($"этап {stageName} пройден");
    }

    public void ShowError(string message, string stageName)
    {
        if (errorPanel == null)
        {
            errorPanel = GameObject.Find("ErrorPanel");
            if (errorPanel != null)
                errorText = errorPanel.GetComponentInChildren<TMP_Text>();
        }
        
        if (errorPanel == null)
        {
            Debug.LogError($"error panel не найден на сцене {stageName}");
            return;
        }
        
        errorText.text = message;
        errorPanel.SetActive(true);
        Debug.Log($"ошибка на этапе {stageName}: {message}");
    }

    public void OnSuccessNext()
    {
        Debug.Log("on success next вызван");
        
        if (Inventory.Instance != null)
            Inventory.Instance.SaveItems();
        
        if (successPanel != null)
            successPanel.SetActive(false);
        
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log($"переход на сцену {nextSceneName}");
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void OnErrorRestart()
{
    Debug.Log("on error restart вызван");
    
    if (errorPanel != null)
        errorPanel.SetActive(false);
    
    for (int i = 0; i < 4; i++)
        Inventory.Instance.ClearSlot(i);
    
    
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
    
    System.Collections.IEnumerator RestartSceneWithDelay()
    {
        yield return null;
        Debug.Log($"перезагрузка сцены {SceneManager.GetActiveScene().name}");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}