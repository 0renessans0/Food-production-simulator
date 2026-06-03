using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager_Mixing : MonoBehaviour
{
    public GameObject successPanel;
    public GameObject errorPanel;
    public TMP_Text successText;
    public TMP_Text errorText;

    private string nextSceneName;

    void Start()
    {
        FindPanels();
        
        if (successPanel != null)
            successPanel.SetActive(false);
        else
            Debug.LogError("SuccessPanel не найден на сцене Замес!");
        
        if (errorPanel != null)
            errorPanel.SetActive(false);
        else
            Debug.LogError("ErrorPanel не найден на сцене Замес!");
    }

    void FindPanels()
    {
        successPanel = GameObject.Find("SuccessPanel");
        if (successPanel != null)
            successText = successPanel.GetComponentInChildren<TMP_Text>();
        
        errorPanel = GameObject.Find("ErrorPanel");
        if (errorPanel != null)
            errorText = errorPanel.GetComponentInChildren<TMP_Text>();
    }

    public void ShowError(string message, string stageName)
    {
        if (errorPanel == null)
            errorPanel = GameObject.Find("ErrorPanel");
        
        if (errorText == null && errorPanel != null)
            errorText = errorPanel.GetComponentInChildren<TMP_Text>();
        
        if (errorPanel == null)
        {
            Debug.LogError("errorPanel = null!");
            return;
        }
        
        if (errorText != null) errorText.text = message;
        errorPanel.SetActive(true);
        Debug.Log($"Ошибка на этапе {stageName}: {message}");
    }

    public void ShowSuccess(string message, string nextScene, string stageName)
    {
        if (successPanel == null)
            successPanel = GameObject.Find("SuccessPanel");
        
        if (successText == null && successPanel != null)
            successText = successPanel.GetComponentInChildren<TMP_Text>();
        
        if (successPanel == null)
        {
            Debug.LogError("successPanel = null!");
            return;
        }
        
        if (successText != null) successText.text = message;
        nextSceneName = nextScene;
        successPanel.SetActive(true);
        Debug.Log($"Этап {stageName} пройден");
    }

    public void OnSuccessNext()
    {
        if (Inventory.Instance != null)
            Inventory.Instance.SaveItems();
        
        successPanel.SetActive(false);
        SceneManager.LoadScene(nextSceneName);
    }

    public void OnErrorRestart()
    {
        errorPanel.SetActive(false);
        
        for (int i = 0; i < 4; i++)
            Inventory.Instance.ClearSlot(i);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnErrorCancel()
    {
        errorPanel.SetActive(false);
    }
}