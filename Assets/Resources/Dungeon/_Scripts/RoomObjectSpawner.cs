using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[System.Serializable]
public class SpawnableObject
{
    public GameObject prefab;
    [Range(0f, 1f)] 
    public float spawnChance = 0.5f;
    [Min(1)]
    public int requiredFreeTiles = 1;
    public int maxPerRoom = 1;
    public bool allowInCorridors = false;
    public int minRoomSize = 4;
}

public class RoomObjectSpawner : MonoBehaviour
{
    [SerializeField] private TilemapVisualizer tilemapVisualizer;
    [SerializeField] private Tilemap floorTilemap;
    [SerializeField] private List<SpawnableObject> objectsToSpawn = new List<SpawnableObject>();

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>(); // Новое: храним занятые позиции

    public void SpawnObjectsInRooms(List<BoundsInt> rooms, HashSet<Vector2Int> corridorPositions = null)
    {
        ClearExistingObjects();
        occupiedPositions.Clear(); // Очищаем занятые позиции перед новой генерацией

        foreach (var room in rooms)
        {
            // Сначала собираем все валидные позиции в комнате
            List<Vector2Int> allValidPositions = GetAllValidPositionsInRoom(room, corridorPositions);

            // Перемешиваем позиции для случайного порядка
            ShufflePositions(allValidPositions);

            foreach (var spawnable in objectsToSpawn)
            {
                if (spawnable.prefab == null) continue;
                if (room.size.x < spawnable.minRoomSize || room.size.y < spawnable.minRoomSize) continue;

                int spawnCount = 0;
                while (spawnCount < spawnable.maxPerRoom && Random.value <= spawnable.spawnChance)
                {
                    Vector2Int? spawnPosition = FindSuitablePosition(allValidPositions, spawnable.requiredFreeTiles, 
                                                                   spawnable.allowInCorridors, corridorPositions);
                    if (spawnPosition.HasValue)
                    {
                        InstantiateObject(spawnable.prefab, spawnPosition.Value);
                        MarkPositionAsOccupied(spawnPosition.Value, spawnable.requiredFreeTiles);
                        spawnCount++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    private List<Vector2Int> GetAllValidPositionsInRoom(BoundsInt room, HashSet<Vector2Int> corridorPositions)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        for (int x = room.x + 1; x < room.x + room.size.x - 1; x++)
        {
            for (int y = room.y + 1; y < room.y + room.size.y - 1; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = floorTilemap.GetTile(tilePos);
                
                if (tile != null && tile == tilemapVisualizer.floorTile)
                {
                    validPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        return validPositions;
    }

    private void ShufflePositions(List<Vector2Int> positions)
    {
        for (int i = 0; i < positions.Count; i++)
        {
            Vector2Int temp = positions[i];
            int randomIndex = Random.Range(i, positions.Count);
            positions[i] = positions[randomIndex];
            positions[randomIndex] = temp;
        }
    }

    private Vector2Int? FindSuitablePosition(List<Vector2Int> validPositions, int requiredFreeTiles, 
                                           bool allowInCorridors, HashSet<Vector2Int> corridorPositions)
    {
        foreach (var position in validPositions)
        {
            // Проверяем, не занята ли уже эта позиция
            if (occupiedPositions.Contains(position)) continue;

            // Проверяем, разрешены ли коридоры для этого объекта
            Vector3Int tilePos = (Vector3Int)position;
            TileBase tile = floorTilemap.GetTile(tilePos);
            bool isCorridor = corridorPositions != null && corridorPositions.Contains(position);
            
            if (tile == tilemapVisualizer.floorTile || (allowInCorridors && isCorridor))
            {
                if (HasEnoughFreeSpace(position, requiredFreeTiles))
                {
                    return position;
                }
            }
        }
        return null;
    }

    private void MarkPositionAsOccupied(Vector2Int centerPosition, int radius)
    {
        // Помечаем центральную позицию как занятую
        occupiedPositions.Add(centerPosition);

        // Помечаем соседние позиции, если требуется больше 1 тайла
        if (radius > 1)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    Vector2Int neighborPos = centerPosition + new Vector2Int(x, y);
                    occupiedPositions.Add(neighborPos);
                }
            }
        }
    }

    private bool HasEnoughFreeSpace(Vector2Int centerPosition, int requiredFreeTiles)
    {
        if (requiredFreeTiles <= 1) return true;

        int freeTilesFound = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                
                Vector2Int checkPos = centerPosition + new Vector2Int(x, y);
                if (!occupiedPositions.Contains(checkPos))
                {
                    freeTilesFound++;
                    if (freeTilesFound >= requiredFreeTiles - 1)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void InstantiateObject(GameObject prefab, Vector2Int position)
    {
        Vector3 worldPosition = floorTilemap.GetCellCenterWorld((Vector3Int)position);
        GameObject obj = Instantiate(prefab, worldPosition, Quaternion.identity, transform);
        spawnedObjects.Add(obj);
    }

    private void ClearExistingObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                if (Application.isPlaying) Destroy(obj);
                else DestroyImmediate(obj);
            }
        }
        spawnedObjects.Clear();

        foreach (Transform child in transform)
        {
            if (Application.isPlaying) Destroy(child.gameObject);
            else DestroyImmediate(child.gameObject);
        }
    }
}