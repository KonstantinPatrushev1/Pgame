using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Save;

public class SaveLoadManager : MonoBehaviour
{
    public List<ChestZones> allChests;
    public Inventory inventory;
    private Storage storage;
    private GameData gameData;
    public GameObject player;
    public PlayerInfo playerInfo;
    public string currentSlotFileName = "Slot1.save";
    public SkinColorChanger skinColorChanger;

    void Start()
    {
        storage = new Storage();

        // Проверяем, есть ли сохраненные файлы
        bool hasSave = CheckForExistingSaves();

        if (!hasSave)
        {
            // Если сохранений нет, создаем первое сохранение в "Slot1.save"
            currentSlotFileName = "Slot1.save";
            PlayerPrefs.SetString("SelectedSlot", currentSlotFileName);
            PlayerPrefs.Save();
        
            Debug.Log("No existing saves found. Creating default save in Slot1.");
            storage.Save(currentSlotFileName, new GameData());
        }
        else
        {
            // Загружаем выбранный слот
            string savedSlot = PlayerPrefs.GetString("SelectedSlot", "Slot1.save");
            currentSlotFileName = savedSlot;
        }

        Debug.Log($"Loading game from: {currentSlotFileName}");
        gameData = (GameData)storage.Load(currentSlotFileName, new GameData());
        Load();
    }
    
    private bool CheckForExistingSaves()
    {
        string saveDirectory = Application.persistentDataPath + "/saves";
        if (!Directory.Exists(saveDirectory)) return false;

        string[] saveFiles = Directory.GetFiles(saveDirectory, "Slot*.save");
        return saveFiles.Length > 0;
    }


    public void Save()
    {
        Debug.Log($"Saving game to: {currentSlotFileName}");

        // Сохраняем данные игрока
        playerInfo.SavePlayerInfo(storage, gameData, currentSlotFileName);

        // Сохраняем другие данные
        gameData.position = player.transform.position;
        inventory.SaveInventory(storage, gameData, currentSlotFileName);
        SaveAllChests();
        skinColorChanger.SaveCustomization(storage, gameData, currentSlotFileName);

        // Финальное сохранение в файл
        storage.Save(currentSlotFileName, gameData);

        Debug.Log($"Game successfully saved to: {currentSlotFileName}");
    }




    public void Load()
    {
        Debug.Log($"Loading game from: {currentSlotFileName}");

        // Загружаем данные
        gameData = (GameData)storage.Load(currentSlotFileName, new GameData());
        if (gameData != null)
        {
            // Загружаем данные игрока
            playerInfo.LoadPlayerInfo(storage, gameData, currentSlotFileName);

            // Загружаем другие данные
            player.transform.position = gameData.position;
            inventory.LoadInventory(storage, gameData, currentSlotFileName);
            LoadAllChests();
            skinColorChanger.LoadCustomization(storage, gameData, currentSlotFileName);

            Debug.Log($"Game successfully loaded from: {currentSlotFileName}");
        }
        else
        {
            Debug.LogError($"Failed to load game data from: {currentSlotFileName}");
        }
    }
    
    private bool isSwitchingSlot = false;
    public void SetSlot(int slotIndex)
    {
        isSwitchingSlot = true; // Устанавливаем флаг переключения слота
        currentSlotFileName = $"Slot{slotIndex}.save";
        PlayerPrefs.SetString("SelectedSlot", currentSlotFileName);
        PlayerPrefs.Save();
        Debug.Log($"Switched to slot {slotIndex}: {currentSlotFileName}");
        isSwitchingSlot = false; // Сбрасываем флаг после переключения
    }


    
    public void SaveAllChests()
    {
        foreach (var chest in allChests)
        {
            chest.SaveChestContents(chest.chestId, currentSlotFileName, storage, gameData);
        }
        Debug.Log("All chests saved.");
    }

    public void LoadAllChests()
    {
        foreach (var chest in allChests)
        {
            chest.LoadChestContents(chest.chestId, currentSlotFileName, storage, gameData);
        }
        Debug.Log("All chests loaded.");
    }
    

    public int GetCurrentSlotIndex()
    {
        // Извлекаем номер слота из имени файла
        if (currentSlotFileName.StartsWith("Slot") && currentSlotFileName.EndsWith(".save"))
        {
            string indexStr = currentSlotFileName.Substring(4, currentSlotFileName.Length - 9);
            if (int.TryParse(indexStr, out int slotIndex))
            {
                return slotIndex;
            }
        }
        return 1; // По умолчанию Slot1
    }

}