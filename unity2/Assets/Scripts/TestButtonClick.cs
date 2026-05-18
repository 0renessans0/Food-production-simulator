using UnityEngine;

public class TestButtonClick : MonoBehaviour
{
    public Inventory inventory;

    public void AddBarley()
    {
        Debug.Log("🔘 Тестовая кнопка НАЖАТА!");

        if (inventory == null)
        {
            Debug.LogError("❌ Inventory не назначен в инспекторе!");
            return;
        }

        Debug.Log("✅ Inventory найден, вызываем AddItem");
        inventory.AddItem(0, null, 1);
        Debug.Log("✅ AddItem выполнен");
    }
}