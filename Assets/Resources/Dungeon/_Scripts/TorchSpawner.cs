using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class TorchSpawner : MonoBehaviour
{
    [SerializeField]
    private TilemapVisualizer tilemapVisualizer;
    
    [SerializeField] 
    private GameObject torchPrefab;
    
    [SerializeField] 
    [Range(0f, 1f)] 
    private float spawnChance = 0.5f;
    
    [SerializeField]
    private Tilemap floorTilemap;

    private List<GameObject> spawnedTorches = new List<GameObject>();

    public void SpawnTorchesInRooms(List<BoundsInt> rooms)
    {
        ClearExistingTorches(); // Теперь точно удалит все факелы
        
        foreach (var room in rooms)
        {
            TrySpawnTorchInRoom(room);
        }
    }

    private void TrySpawnTorchInRoom(BoundsInt room)
    {
        if (Random.value > spawnChance) return;
        
        Vector2Int? torchPosition = FindSuitablePosition(room);
        if (torchPosition.HasValue)
        {
            InstantiateTorch(torchPosition.Value);
        }
    }

    private Vector2Int? FindSuitablePosition(BoundsInt room)
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        for (int x = room.x + 1; x < room.x + room.size.x - 1; x++)
        {
            for (int y = room.y + 1; y < room.y + room.size.y - 1; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = floorTilemap.GetTile(tilePos);
            
                // Проверяем, что тайл существует И является именно floorTile (не wall или background)
                if (tile != null && tile == tilemapVisualizer.floorTile)
                {
                    validPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        return validPositions.Count > 0 ? validPositions[Random.Range(0, validPositions.Count)] : (Vector2Int?)null;
    }

    private void InstantiateTorch(Vector2Int position)
    {
        Vector3 worldPosition = floorTilemap.GetCellCenterWorld((Vector3Int)position);
        GameObject torch = Instantiate(torchPrefab, worldPosition, Quaternion.identity, transform);
        spawnedTorches.Add(torch); // Запоминаем созданный факел
    }

    public void ClearExistingTorches()
    {
        // Уничтожаем все запомненные факелы
        foreach (var torch in spawnedTorches)
        {
            if (torch != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(torch);
                }
                else
                {
                    DestroyImmediate(torch);
                }
            }
        }
        spawnedTorches.Clear();

        // Дополнительная очистка дочерних объектов на случай, если что-то пропустили
        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
}