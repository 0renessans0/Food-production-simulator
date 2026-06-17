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

                    BindSuccessPanelButton(successPanel);
                }

                if (child.name == "ErrorPanel")
                {
                    errorPanel = child.gameObject;
                    Debug.Log("error panel найден");
                    BindErrorPanelButton(errorPanel);
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

        BindSuccessPanelButton(successPanel);
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

        BindErrorPanelButton(errorPanel);
        errorText.text = message;
        errorPanel.SetActive(true);
        Debug.Log($"ошибка на этапе {stageName}: {message}");
    }

    Button FindChildButton(Transform parent, params string[] names)
    {
        foreach (string name in names)
        {
            Transform child = parent.Find(name);
            if (child == null) continue;

            Button button = child.GetComponent<Button>();
            if (button != null)
                return button;
        }

        return null;
    }

    void BindSuccessPanelButton(GameObject panel)
    {
        Button nextButton = FindChildButton(panel.transform, "NextButton", "Button");
        if (nextButton == null)
        {
            Debug.LogWarning("кнопка next не найдена в SuccessPanel");
            return;
        }

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(OnSuccessNext);
        Debug.Log("кнопка next привязана");
    }

    void BindErrorPanelButton(GameObject panel)
    {
        Button restartButton = FindChildButton(panel.transform, "RestartButton", "Button");
        if (restartButton == null)
        {
            Debug.LogWarning("кнопка restart не найдена в ErrorPanel");
            return;
        }

        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(OnErrorRestart);
        Debug.Log("кнопка restart привязана");
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
        if (Inventory.Instance != null)
        
        {
            for (int i = 0; i < Inventory.Instance.items.Count; i++)
            Inventory.Instance.ClearSlot(i);

            for (int i = 0; i < Inventory.Instance.savedItems.Count && i < Inventory.Instance.items.Count; i++)
            {
                var saved = Inventory.Instance.savedItems[i];
                if (saved.itemData != null)
                Inventory.Instance.AddItem(i, saved.itemData, saved.count);
            }
        }
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"перезагрузка сцены {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
