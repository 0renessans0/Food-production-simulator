using UnityEngine;

public class KettleClick : MonoBehaviour
{
    public MixingManager mixingManager;
    public Inventory inventory;

    private void OnMouseDown()
    {
        int selectedSlot = inventory.GetSelectedSlot();
        
        if (selectedSlot == -1)
        {
            Debug.Log("сначала выберите ингредиент в инвентаре!");
            return;
        }

        if (selectedSlot >= 4) 
        {
            Debug.Log("это не ингредиент для замеса!");
            return;
        }

        if (inventory.items[selectedSlot].itemData == null || inventory.items[selectedSlot].count == 0)
        {
            Debug.Log("выбранном слоте нет ингредиента!");
            return;
        }

        mixingManager.AddIngredientFromSlot(selectedSlot);
        
        inventory.SelectSlot(-1);
    }
}