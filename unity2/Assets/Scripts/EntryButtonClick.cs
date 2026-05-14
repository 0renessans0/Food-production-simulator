using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryButtonClick : MonoBehaviour
{
     public void onButtonClick()
    {
        SceneManager.LoadScene("AcceptenceScen");
    }  
}
