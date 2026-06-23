using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class MixingManager : MonoBehaviour
{
    public UIManager uiManager;
    public Inventory inventory;
    public Slider progressBar;
    public string nextScene = "FermentationScen";
    public string stageName = "Замес";
    private int ingredientsAdded = 0;
    private int requiredIngredients = 4;
    private bool[] ingredientUsed = new bool[4];
    private int wrongIngredientsAdded = 0;

    void Start()
    {
        Debug.Log("сцена замеса");
        UIManager.Instance.FindPanelsOnCurrentScene();

        if (uiManager == null)
            uiManager = FindAnyObjectByType<UIManager>();

        if (inventory == null)
        {
            inventory = Inventory.Instance;
            if (inventory == null)
                inventory = FindAnyObjectByType<Inventory>();
        }

        Transform panel = GameObject.Find("InventoryPanel")?.transform;
        if (panel != null && Inventory.Instance != null)
        {
            Inventory.Instance.SetInventoryPanel(panel);

            var saved = Inventory.Instance.savedItems;
            if (saved != null && saved.Count > 0)
            {
                for (int i = 0; i < saved.Count && i < Inventory.Instance.items.Count; i++)
                {
                    if (saved[i].itemData != null)
                    {
                        Inventory.Instance.items[i].itemData = saved[i].itemData;
                        Inventory.Instance.items[i].count = saved[i].count;
                        Inventory.Instance.UpdateSlotUI(i);
                    }
                }
            }
        }

        if (progressBar != null)
        {
            progressBar.maxValue = requiredIngredients;
            progressBar.value = 0;
            UpdateProgressBarColor(0);
        }

        Button nextButton = GameObject.Find("Button_ToFermentation")?.GetComponent<Button>();
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => CheckStage());
            Debug.Log("кнопка привязана к CheckStage");
        }
    }

    void UpdateProgressBarColor(float value)
    {
        if (progressBar == null) return;

        Image fillImage = progressBar.fillRect?.GetComponent<Image>();
        if (fillImage == null) return;

        float percent = value / requiredIngredients;
        fillImage.color = Color.Lerp(Color.red, Color.green, percent);
    }

    public void AddIngredientFromSlot(int slotIndex)
    {
        if (ingredientsAdded >= requiredIngredients)
        {
            Debug.Log("все ингредиенты добавлены");
            return;
        }

        if (inventory == null)
        {
            inventory = FindAnyObjectByType<Inventory>();
            if (inventory == null) return;
        }

        if (slotIndex >= inventory.items.Count) return;

        var item = inventory.items[slotIndex].itemData;
        if (item == null || inventory.items[slotIndex].count == 0)
        {
            Debug.Log($"в слоте {slotIndex} нет ингредиента");
            return;
        }

        if (slotIndex >= 4)
        {
            wrongIngredientsAdded++;
            inventory.ClearSlot(slotIndex);
            Debug.Log($"добавлен не тот ингредиент: {item.itemName}");

            if (ErrorManager.Instance != null)
                ErrorManager.Instance.AddError($"Замес: добавлен {item.itemName} не ингредиент");

            StartCoroutine(ShowErrorWithDelay($"Нельзя добавить {item.itemName} в котел!"));
            return;
        }

        if (ingredientUsed[slotIndex])
        {
            Debug.Log($"ингредиент {item.itemName} уже добавлялся");
            if (ErrorManager.Instance != null)
                ErrorManager.Instance.AddError($"Замес: повторное добавление {item.itemName}");

            StartCoroutine(ShowErrorWithDelay($"Ингредиент {item.itemName} уже добавлен!"));
            return;
        }

        ingredientUsed[slotIndex] = true;
        ingredientsAdded++;

        if (progressBar != null)
        {
            progressBar.value = ingredientsAdded;
            UpdateProgressBarColor(ingredientsAdded);
        }

        inventory.ClearSlot(slotIndex);

        Debug.Log($"добавлен {item.itemName} в чан {ingredientsAdded} из {requiredIngredients}");
    }

    public void CheckStage()
    {
        Debug.Log($"проверка замеса правильных={ingredientsAdded} неправильных={wrongIngredientsAdded} требуется={requiredIngredients}");

        if (uiManager == null)
        {
            uiManager = FindAnyObjectByType<UIManager>();
            if (uiManager == null)
            {
                Debug.LogError("uiManager равен null");
                return;
            }
        }

        bool hasError = false;
        string errorMessage = "";

        if (wrongIngredientsAdded > 0)
        {
            hasError = true;
            errorMessage = $"добавлено {wrongIngredientsAdded} неправильных ингредиентов ";

            if (ErrorManager.Instance != null)
                ErrorManager.Instance.AddError($"Замес: {errorMessage}");
        }

        if (ingredientsAdded < requiredIngredients)
        {
            hasError = true;
            errorMessage += $"добавлено только {ingredientsAdded} из {requiredIngredients} правильных ингредиентов ";

            if (ErrorManager.Instance != null)
                ErrorManager.Instance.AddError($"Замес: {errorMessage}");
        }

        if (hasError)
        {
            StartCoroutine(ShowErrorWithDelay($"Ошибка замеса: {errorMessage}"));
        }
        else
        {
            Debug.Log("замес пройден успешно");

            if (Inventory.Instance != null)
                Inventory.Instance.SaveItems();

            StartCoroutine(ShowSuccessWithDelay("Замес пройден!"));
        }
    }

    IEnumerator ShowSuccessWithDelay(string message)
    {
        yield return new WaitForSeconds(0.2f);
        if (UIManager.Instance != null)
            UIManager.Instance.ShowSuccess(message, nextScene, stageName);
        else
            Debug.LogError("UIManager.Instance = null! Не удалось показать панель успеха.");
    }

    IEnumerator ShowErrorWithDelay(string message)
    {
        yield return new WaitForSeconds(0.2f);
        if (UIManager.Instance != null)
            UIManager.Instance.ShowError(message, stageName);
        else
            Debug.LogError("UIManager.Instance = null! Не удалось показать панель ошибки.");
    }

    public void NextScene()
    {
        Debug.Log($"переход на следующую сцену ingredientsAdded={ingredientsAdded} required={requiredIngredients}");

        if (ingredientsAdded >= requiredIngredients && wrongIngredientsAdded == 0)
        {
            if (Inventory.Instance != null)
                Inventory.Instance.SaveItems();

            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log($"не хватает ингредиентов или есть ошибки {ingredientsAdded} из {requiredIngredients}");
        }
    }
}