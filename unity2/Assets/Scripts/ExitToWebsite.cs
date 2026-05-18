using UnityEngine;

public class ExitToWebsite : MonoBehaviour
{
    public void ExitToSite()
    {
        // Для WebGL сборки
        #if UNITY_WEBGL && !UNITY_EDITOR
            Application.ExternalEval("window.location.href = '../website/index.html';");
        #else
            // Для тестирования в редакторе Unity
            Debug.Log("Возврат на сайт: ../website/index.html");
        #endif
    }
}