using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0,10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;
    [SerializeField] 
    private TorchSpawner torchSpawner;
    [SerializeField] 
    private RoomObjectSpawner roomObjectSpawner;
    
    
    // Добавляем список центров комнат как поле класса
    private List<Vector2Int> roomCenters = new List<Vector2Int>();

    public void GenerateDungeon()
    {
        RunProceduralGeneration();
    }
    public bool IsGenerationComplete { get; private set; } = true;
    
    public void ClearDungeon()
    {
        // Очищаем тайлы
        tilemapVisualizer.Clear();
    
        // Очищаем данные
        roomCenters.Clear();
    
        // Сбрасываем флаг генерации
        IsGenerationComplete = true;
    }
    protected override void RunProceduralGeneration()
    {
        IsGenerationComplete = false;
    
        // Очищаем перед генерацией
        tilemapVisualizer.Clear();
        roomCenters.Clear();
        roomObjectSpawner.ClearExistingObjects();
        torchSpawner.ClearExistingTorches();
    
        CreateRooms();
        IsGenerationComplete = true;
    }
    
    public Vector2Int? GetRandomRoomCenter()
    {
        if (roomCenters.Count == 0) return null;
        return roomCenters[Random.Range(0, roomCenters.Count)];
    }
    
    

    private void CreateRooms()
    {
        var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
            new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)), 
            minRoomWidth,
            minRoomHeight
        );

        roomCenters.Clear();
        foreach (var room in roomsList)
        {
            Vector2Int center = (Vector2Int)Vector3Int.RoundToInt(room.center);
            roomCenters.Add(center);
        }

        HashSet<Vector2Int> corridors = ConnectRooms(new List<Vector2Int>(roomCenters));
    
        // Генерация пола комнат
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }

        // Объединяем пол комнат и коридоры
        floor.UnionWith(corridors);

        // Сначала рисуем все полы обычным тайлом
        tilemapVisualizer.PaintFloorTiles(floor);
    
        // Затем заменяем тайлы коридоров на специальные
        tilemapVisualizer.ReplaceTiles(corridors, tilemapVisualizer.corridorTile);

        WallGenerator.CreateWalls(floor, tilemapVisualizer);
        torchSpawner.SpawnTorchesInRooms(roomsList);
        roomObjectSpawner.SpawnObjectsInRooms(roomsList, corridors);
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter =
                new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) &&
                    position.y >= (roomBounds.yMin + offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }

        return floor;
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
        }

        return corridors;
    }

    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }

        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }
            else if (destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }

        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }

        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x -offset; col++)
            {
                for (int row = offset; row < room.size.y -offset; row++)
                {
                    Vector2Int position = (Vector2Int) room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }

        return floor;
    }
}