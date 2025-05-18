using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Save;
public class PlayerInfo : MonoBehaviour
{
    public float hp;
    public float maxhp;

    public float stamina;
    public float maxstamina;
    
    private float timer = 0f;
    
    private Storage storage;
    private GameData gameData;
    

    private void Start()
    {
        storage = new Storage();
        gameData = new GameData();
        maxhp = 300;
        maxstamina = 1440f;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f && stamina > 0)
        {
            stamina--;
            timer = 0f;
        }
    }

    public void SetDamage(float newdamage)
    {
        hp -= newdamage;
    }

    public void SetStamina(float newstamina)
    {
        stamina -= newstamina;
    }
    
    public void SavePlayerInfo(Storage storage, GameData gameData, string currentSlotFileName)
    {
        if (gameData == null)
        {
            gameData = new GameData();
        }

        // Обновляем данные игрока в GameData
        gameData.hp = hp;
        gameData.stamina = stamina;

        Debug.Log($"Saving HP: {gameData.hp}, Stamina: {gameData.stamina}");
    }

    public void LoadPlayerInfo(Storage storage, GameData gameData, string currentSlotFileName)
    {
        if (storage == null)
        {
            storage = new Storage();
        }

        // Проверяем, существует ли файл сохранения
        string filePath = Application.persistentDataPath + "/saves/" + currentSlotFileName;
        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"Save file not found: {filePath}. Using default values.");
            return;
        }

        // Загружаем данные
        gameData = (GameData)storage.Load(currentSlotFileName, new GameData());
        if (gameData != null)
        {
            // Загружаем значения HP и Stamina
            hp = gameData.hp;
            stamina = gameData.stamina;

            Debug.Log($"Loaded HP: {gameData.hp}, Stamina: {gameData.stamina} from file {filePath}");
        }
        else
        {
            Debug.LogError("Failed to load player info: GameData is null");
        }
    }


}
