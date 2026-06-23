using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PackagingManager : MonoBehaviour
{
    public UIManager uiManager;
    public Inventory inventory;
    public GameObject emptyBox;
    public GameObject boxWithBottle033;
    public GameObject boxWithBottle05;
    public GameObject boxWithBottle1;
    public GameObject boxWithCan033;
    public GameObject boxWithCan05;
    public GameObject sticker;
    public Button stickerButton;
    public Button nextButton;
    public string nextScene = "ResultScen";
    public string stageName = "Упаковка";
    private bool isPacked = false;
    private bool hasLabel = false;
    private string packedType = "";
    private string expectedPackedType = "Bottle05";

    void Start()
    {
        string correctPackagingFromDB = PlayerPrefs.GetString("CorrectPackaging", "стекло_0.5");
        Debug.Log($"Правильная тара из БД: {correctPackagingFromDB}");

        switch (correctPackagingFromDB)
        {
            case "стекло_0.33":
                expectedPackedType = "Bottle033";
                break;
            case "стекло_0.5":
                expectedPackedType = "Bottle05";
                break;
            case "стекло_1":
                expectedPackedType = "Bottle1";
                break;
            case "металл_0.33":
                expectedPackedType = "Can033";
                break;
            case "металл_0.5":
                expectedPackedType = "Can05";
                break;
            default:
                expectedPackedType = "Bottle05";
                break;
        }
        Debug.Log($"Ожидаемый тип упаковки: {expectedPackedType}");

        if (uiManager == null)
            uiManager = FindAnyObjectByType<UIManager>();

        if (inventory == null)
        {
            inventory = Inventory.Instance;
            if (inventory == null)
                inventory = FindAnyObjectByType<Inventory>();
        }

        UIManager.Instance.FindPanelsOnCurrentScene();

        emptyBox.SetActive(true);
        boxWithBottle033.SetActive(false);
        boxWithBottle05.SetActive(false);
        boxWithBottle1.SetActive(false);
        boxWithCan033.SetActive(false);
        boxWithCan05.SetActive(false);
        sticker.SetActive(false);

        if (stickerButton != null) stickerButton.interactable = true;
        if (nextButton != null) nextButton.interactable = true;

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => CheckStage());
            Debug.Log("кнопка Next привязана к CheckStage");
        }

        if (stickerButton != null)
        {
            stickerButton.onClick.RemoveAllListeners();
            stickerButton.onClick.AddListener(() => ApplySticker());
            Debug.Log("кнопка Sticker привязана к ApplySticker");
        }

        Transform panel = GameObject.Find("InventoryPanel")?.transform;
        if (panel != null && Inventory.Instance != null)
        {
            Inventory.Instance.SetInventoryPanel(panel);
        }

        Debug.Log("PackagingManager Start: inventory = " + (inventory != null ? "найден" : "null"));
    }

    public void OnBoxClick()
    {
        Debug.Log("OnBoxClick вызван");

        if (inventory == null)
        {
            inventory = FindAnyObjectByType<Inventory>();
            if (inventory == null)
            {
                Debug.LogError("Inventory не найден");
                return;
            }
        }

        if (isPacked)
        {
            Debug.Log("уже упаковано");
            return;
        }

        int selectedSlot = inventory.GetSelectedSlot();
        Debug.Log($"выбранный слот: {selectedSlot}");

        if (selectedSlot == -1)
        {
            Debug.Log("сначала выберите тару в инвентаре");
            StartCoroutine(ShowErrorWithDelay("Сначала выберите тару в инвентаре!"));
            return;
        }

        if (selectedSlot >= inventory.items.Count)
        {
            Debug.Log($"слот {selectedSlot} вне диапазона");
            return;
        }

        var item = inventory.items[selectedSlot].itemData;
        if (item == null)
        {
            Debug.Log("в выбранном слоте нет предмета");
            return;
        }

        switch (item.itemType)
        {
            case ItemType.GlassBottle033:
                packedType = "Bottle033";
                emptyBox.SetActive(false);
                boxWithBottle033.SetActive(true);
                break;
            case ItemType.GlassBottle05:
                packedType = "Bottle05";
                emptyBox.SetActive(false);
                boxWithBottle05.SetActive(true);
                break;
            case ItemType.GlassBottle1:
                packedType = "Bottle1";
                emptyBox.SetActive(false);
                boxWithBottle1.SetActive(true);
                break;
            case ItemType.MetalCan033:
                packedType = "Can033";
                emptyBox.SetActive(false);
                boxWithCan033.SetActive(true);
                break;
            case ItemType.MetalCan05:
                packedType = "Can05";
                emptyBox.SetActive(false);
                boxWithCan05.SetActive(true);
                break;
            default:
                Debug.Log("этот предмет нельзя упаковать");
                StartCoroutine(ShowErrorWithDelay("Этот предмет нельзя упаковать!"));
                return;
        }

        inventory.ClearSlot(selectedSlot);
        inventory.SelectSlot(-1);

        isPacked = true;

        Debug.Log($"упакована тара: {item.itemName} (тип: {packedType})");
    }

    public void ApplySticker()
    {
        Debug.Log("ApplySticker вызван");

        if (!isPacked)
        {
            StartCoroutine(ShowErrorWithDelay("Сначала упакуйте товар!"));
            return;
        }

        if (hasLabel) return;

        hasLabel = true;
        sticker.SetActive(true);

        Debug.Log("этикетка наклеена");
    }

    public void CheckStage()
    {
        Debug.Log($"CheckStage");
        Debug.Log($"isPacked={isPacked}, hasLabel={hasLabel}, packedType={packedType}, expectedPackedType={expectedPackedType}");

        if (!isPacked)
        {
            Debug.Log("ОШИБКА: не упаковано");
            if (ErrorManager.Instance != null)
                ErrorManager.Instance.AddError("Упаковка: попытка завершить без упаковки");
            StartCoroutine(ShowErrorWithDelay("Сначала упакуйте товар!"));
            return;
        }
        Debug.Log("упаковано");

        if (!hasLabel)
        {
            Debug.Log("ошибка нет этикетки");
            if (ErrorManager.Instance != null)
                ErrorManager.Instance.AddError("Упаковка: попытка завершить без этикетки");
            StartCoroutine(ShowErrorWithDelay("Сначала наклейте этикетку"));
            return;
        }
        Debug.Log("этикетка есть");

        if (packedType != expectedPackedType)
        {
            Debug.Log($"ошибка неверная тара ({packedType} != {expectedPackedType})");
            if (ErrorManager.Instance != null)
                ErrorManager.Instance.AddError($"Упаковка: выбрана неверная тара ({packedType}). Нужна {expectedPackedType}");
            StartCoroutine(ShowErrorWithDelay($"Упакована неправильная тара! Нужна бутылка 0.5л."));
            return;
        }
        Debug.Log("тара правильная");

        Debug.Log("всё верно, ShowSuccess");

        if (Inventory.Instance != null)
            Inventory.Instance.SaveItems();

        StartCoroutine(ShowSuccessWithDelay());
        Debug.Log("активация панели вручную");
        uiManager.successPanel.SetActive(true);
    }

    IEnumerator ShowSuccessWithDelay()
    {
        yield return new WaitForSeconds(0.2f);

        if (uiManager == null)
            uiManager = FindAnyObjectByType<UIManager>();

        if (uiManager != null)
            uiManager.ShowSuccess("Упаковка пройдена!", nextScene, stageName);
    }

    IEnumerator ShowErrorWithDelay(string message)
    {
        yield return new WaitForSeconds(0.2f);
        if (UIManager.Instance != null)
            UIManager.Instance.ShowError(message, stageName);
    }

    public void NextScene()
    {
        Debug.Log($"NextScene() вызван на этапе {stageName}");

        if (Inventory.Instance != null)
            Inventory.Instance.SaveItems();

        SceneManager.LoadScene(nextScene);
    }
}