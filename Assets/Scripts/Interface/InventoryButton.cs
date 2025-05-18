using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryButton : MonoBehaviour, IPointerDownHandler
{
    public Inventory inventory; // Ссылка на инвентарь

    public void OnPointerDown(PointerEventData eventData)
    {
        if (inventory != null)
        {
            inventory.SelectObject(); // Вызываем метод SelectObject
        }
    }
}