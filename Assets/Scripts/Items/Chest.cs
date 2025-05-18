using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    public GameObject inventoryPanel; // Панель для отображения ячеек копии
    public int copyInventorySize = 20; // Количество ячеек в копии инвентаря

    private Inventory mainInventory; // Ссылка на основной инвентарь
    private List<ItemInventory> copyItems = new List<ItemInventory>(); // Список ячеек копии

    public GameObject itemSlotPrefab; // Префаб ячейки для копии

    public void Initialize(Inventory inventory)
    {
        mainInventory = inventory;

        // Создаем ячейки для копии инвентаря
        for (int i = 0; i < copyInventorySize; i++)
        {
            GameObject newSlot = Instantiate(itemSlotPrefab, inventoryPanel.transform);
            newSlot.name = $"CopySlot_{i}";

            ItemInventory ii = new ItemInventory();
            ii.itemGameObj = newSlot;

            RectTransform rt = newSlot.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
            newSlot.GetComponentInChildren<RectTransform>().localScale = Vector3.one;

            copyItems.Add(ii);
        }

        UpdateCopyInventory();
    }

    public void UpdateCopyInventory()
    {
        // Обновляем отображение копии инвентаря, начиная с последней ячейки основного инвентаря
        int startIndex = mainInventory.hotbarSize + mainInventory.maxCount;

        for (int i = 0; i < copyInventorySize; i++)
        {
            if (startIndex + i < mainInventory.items.Count)
            {
                var mainItem = mainInventory.items[startIndex + i];
                var copyItem = copyItems[i];

                var textComponent = copyItem.itemGameObj.GetComponentInChildren<TextMeshProUGUI>();

                if (mainItem.id != 0 && mainItem.count > 1 && !mainInventory.data.items[mainItem.id].istool)
                {
                    textComponent.text = mainItem.count.ToString();
                }
                else
                {
                    textComponent.text = "";
                }

                copyItem.itemGameObj.GetComponentInChildren<Image>().sprite = mainInventory.data.items[mainItem.id].img;
            }
        }
    }
}
