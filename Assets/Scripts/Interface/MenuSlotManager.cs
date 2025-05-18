using UnityEngine;
using UnityEngine.UI;

public class MenuSlotManager : MonoBehaviour
{
    public Button[] slotButtons;

    void Start()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slotIndex = i + 1;
            slotButtons[i].onClick.RemoveAllListeners();
            slotButtons[i].onClick.AddListener(() => OnSlotSelected(slotIndex));

            // Проверяем наличие файла сохранения
            string slotFileName = $"Slot{slotIndex}.save";
            if (!DoesSaveFileExist(slotFileName))
            {
                slotButtons[i].interactable = false;
            }
        }
    }

    private void OnSlotSelected(int slotIndex)
    {
        // Формируем имя файла слота
        string slotFileName = $"Slot{slotIndex}.save";

        // Сохраняем выбранный слот в PlayerPrefs
        PlayerPrefs.SetString("SelectedSlot", slotFileName);
        PlayerPrefs.Save();

        Debug.Log($"Slot {slotIndex} selected, switching to scene with {slotFileName}");

        // Загружаем новую сцену
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }


    private bool DoesSaveFileExist(string slotFileName)
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "saves", slotFileName);
        return System.IO.File.Exists(path);
    }
}