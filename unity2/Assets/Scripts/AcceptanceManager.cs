using UnityEngine;
using UnityEngine.SceneManagement; 

public class AcceptanceManager : MonoBehaviour
{
    public UIManager uiManager;
    public Inventory inventory;
    public string nextScene = "MixingScene";
    public string stageName = "Приёмка";

    public void CheckStage()
    {
        Debug.Log("🔍 CheckStage ВЫЗВАН!");
        
        bool allGood = true;
        string errorMsg = "";

        for (int i = 0; i < 9; i++)
        {
            if (i >= inventory.items.Count) break;
            
            var item = inventory.items[i];
            if (item == null || item.itemData == null || item.count == 0)
            {
                allGood = false;
                errorMsg += $"Не хватает ингредиента в слоте {i}. ";
            }
        }

        if (allGood)
        {
            uiManager.ShowSuccess("Приёмка пройдена!", nextScene, stageName);
        }
        else
        {
            uiManager.ShowError($"Ошибка: {errorMsg}", stageName);
        }
    }

    public void NextScene()
{
    Inventory.Instance.SaveItems();
    SceneManager.LoadScene("MixingScene");
}
}