using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Save;

public class ChestZones : MonoBehaviour
{
    public static bool IsAnyChestOpen = false;
    public int chestId;
    public Dictionary<int, List<ItemInventory>> chestStorage = new Dictionary<int, List<ItemInventory>>();
    public float zoneSize = 0f; // Расстояние от центра сундука до зоны
    public Vector2 zoneDimensions = new Vector2(1f, 1f); // Размер зоны (ширина, высота)
    public Transform player; // Ссылка на объект игрока
    public Animator playerAnimator; // Ссылка на Animator игрока
    public Inventory inventory;
    public MapController mapController;
    
    public GameObject chestPanel;

    public CursorManager cursormanager;
    private GameData gameData;
    private Storage storage;
    
    void Start()
    {
        storage = new Storage();
        if (!chestStorage.ContainsKey(chestId))
        {
            chestStorage[chestId] = new List<ItemInventory>();
            for (int i = 0; i < 20; i++)
            {
                chestStorage[chestId].Add(new ItemInventory());
            }
        }
        // Позиции зон относительно сундука
        Vector2[] zoneOffsets = new Vector2[] {
            new Vector2(-zoneSize, 0), // Лево
            new Vector2(zoneSize, 0),  // Право
            new Vector2(0, zoneSize),  // Вверх
            new Vector2(0, -zoneSize)  // Вниз
        };

        // Создаем зоны
        foreach (Vector2 offset in zoneOffsets)
        {
            GameObject zone = new GameObject("Zone");
            zone.tag = "ChestZone";
            zone.transform.parent = transform; // Делаем объект дочерним сундуку
            zone.transform.localPosition = offset; // Устанавливаем позицию относительно центра сундука

            // Добавляем BoxCollider2D
            BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
            collider.isTrigger = true; // Устанавливаем на триггер
            collider.size = zoneDimensions; // Устанавливаем размер зоны

            // Добавляем компонент для обработки OnTriggerEnter2D
            ZoneTrigger zoneTrigger = zone.AddComponent<ZoneTrigger>();
            zoneTrigger.chest = this; // Передаем ссылку на сундук
            zoneTrigger.mapController = mapController; // Передаем ссылку на MapController
            zoneTrigger.cursormanager = cursormanager;
        }
    }

    private void Update()
    {
        
    }

    public bool IsPlayerLookingAtChest()
    {
        // Получаем направление взгляда персонажа
        float horizontal = playerAnimator.GetFloat("Horizontal");
        float vertical = playerAnimator.GetFloat("Vertical");

        // Находим направление от игрока к сундуку
        Vector2 directionToChest = (transform.position - player.position).normalized;

        // Проверка, смотрит ли игрок в сторону сундука
        if (horizontal > 0 && directionToChest.x > 0) return true;
        if (horizontal < 0 && directionToChest.x < 0) return true;
        if (vertical > 0 && directionToChest.y > 0) return true;
        if (vertical < 0 && directionToChest.y < 0) return true;

        return false;
    }
    
    public void SaveChestContents(int chestId, string currentSlotFileName, Storage storage, GameData gameData)
    {
        if (!chestStorage.ContainsKey(chestId)) return;

        var chestItems = chestStorage[chestId];
        var chestData = new List<ItemInventoryData>();

        foreach (var item in chestItems)
        {
            chestData.Add(new ItemInventoryData(item));
        }

        // Сохраняем содержимое сундука в текущие данные игры
        if (!gameData.chests.ContainsKey(chestId))
        {
            gameData.chests[chestId] = new List<ItemInventoryData>();
        }

        gameData.chests[chestId] = chestData;
        Debug.Log($"Chest {chestId} contents saved.");
    }

    public void LoadChestContents(int chestId, string currentSlotFileName, Storage storage, GameData gameData)
    {
        
        gameData = (GameData)storage.Load(currentSlotFileName, new GameData());

        if (gameData.chests.ContainsKey(chestId))
        {
            var chestData = gameData.chests[chestId];

            // Очищаем сундук и загружаем предметы
            if (!chestStorage.ContainsKey(chestId))
            {
                chestStorage[chestId] = new List<ItemInventory>();
            }
            chestStorage[chestId].Clear();

            foreach (var itemData in chestData)
            {
                chestStorage[chestId].Add(new ItemInventory(itemData));
            }

            Debug.Log($"Chest {chestId} contents loaded.");
        }
        else
        {
            Debug.LogWarning($"No data found for chest {chestId} in save file.");
        }
    }

}