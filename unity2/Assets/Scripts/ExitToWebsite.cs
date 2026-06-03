using UnityEngine;

public class ExitToWebsite : MonoBehaviour
{
    public void ExitToSite()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
            Application.ExternalEval("window.location.href = '../website/index.html';");
        #else
            Debug.Log("Возврат на сайт: ../website/index.html");
        #endif
    }
}