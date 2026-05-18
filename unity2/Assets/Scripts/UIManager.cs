using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject successPanel;
    public GameObject errorPanel;

    private string nextSceneName;
    private string currentStageName;
    private string sessionId;

    void Start()
    {
        successPanel.SetActive(false);
        errorPanel.SetActive(false);
        sessionId = "temp_session_" + System.DateTime.Now.Ticks;
    }

    public void ShowSuccess(string message, string nextScene, string stageName)
    {
        nextSceneName = nextScene;
        currentStageName = stageName;
        successPanel.SetActive(true);
        Debug.Log($"✅ Запись в БД: {stageName} пройден, сессия {sessionId}");
    }

    public void ShowError(string message, string stageName)
    {
        currentStageName = stageName;
        errorPanel.SetActive(true);
        Debug.Log($"❌ Запись в БД: Ошибка на этапе {stageName}: {message}, сессия {sessionId}");
    }

    public void OnSuccessNext()
    {
        successPanel.SetActive(false);
        SceneManager.LoadScene(nextSceneName);
    }

    public void OnErrorRestart()
    {
        errorPanel.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnErrorCancel()
    {
        errorPanel.SetActive(false);
    }
}