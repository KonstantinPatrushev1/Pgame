using UnityEngine;
using UnityEngine.UI;

public class SlotManager : MonoBehaviour
{
    public Button[] slotButtons; // Кнопки для выбора слотов
    public SaveLoadManager saveLoadManager; // Ссылка на SaveLoadManager

    void Start()
    {
        saveLoadManager = FindObjectOfType<SaveLoadManager>();

        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slotIndex = i + 1;
            slotButtons[i].onClick.AddListener(() =>
            {
                OnSlotSelected(slotIndex);
            });
        }

        UpdateSlotButtonTexts(); // Инициализируем тексты кнопок
    }

    private void OnSlotSelected(int slotIndex)
    {
        // Устанавливаем текущий слот
        saveLoadManager.SetSlot(slotIndex);

        // Сохраняем сразу после выбора слота
        saveLoadManager.Save();

        // Обновляем текст кнопок
        UpdateSlotButtonTexts();

        // Лог
        Debug.Log($"Slot {slotIndex} selected, file saved to {saveLoadManager.currentSlotFileName}");
    }

    private void UpdateSlotButtonTexts()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slotIndex = i + 1;
            var buttonText = slotButtons[i].GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = slotIndex == saveLoadManager.GetCurrentSlotIndex()
                    ? $"Slot {slotIndex} (Current)"
                    : $"Slot {slotIndex}";
            }
        }
    }
}