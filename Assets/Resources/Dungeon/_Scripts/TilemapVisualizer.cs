using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Tilemaps;

public class TilemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    private TileBase wallTop, wallSideRight, wallSideLeft, wallBottom, wallFull, wallInnerCornerDownLeft, wallInnerCornerDownRight, wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;
    [SerializeField] 
    public TileBase floorTile;
    [SerializeField]
    private TileBase backgroundTile;
    [SerializeField] 
    private PhysicsMaterial2D wallPhysicsMaterial;
    [SerializeField]
    public TileBase corridorTile;
    public void PaintFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PaintTiles(floorPositions, floorTilemap, floorTile);
    }
    
    public void ReplaceTiles(HashSet<Vector2Int> positions, TileBase newTile)
    {
        foreach (var position in positions)
        {
            var tilePosition = floorTilemap.WorldToCell((Vector3Int)position);
            if (floorTilemap.GetTile(tilePosition) != null) // Если тайл существует
            {
                floorTilemap.SetTile(tilePosition, newTile);
            }
        }
    }

    private void PaintTiles(IEnumerable<Vector2Int> positions, Tilemap tilemap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTile(tilemap, tile, position);
        }
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int) position);
        tilemap.SetTile(tilePosition, tile);
    }

    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    internal void PaintSingleBasicWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tile = wallTop;
        }else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = wallSideRight;
        }else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = wallSideLeft;
        }else if (WallTypesHelper.wallBottm.Contains(typeAsInt))
        {
            tile = wallBottom;
        }else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        if(tile!=null)
            PaintSingleTile(wallTilemap, tile, position);
    }

    public void PaintSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typeAsInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
        {
            tile = wallBottom;
        }
        
        if(tile!=null)
            PaintSingleTile(wallTilemap, tile, position);
    }
    
    public void AddBoxCollidersToWalls()
    {
        if (wallTilemap == null)
        {
            Debug.LogError("wallTilemap is not assigned!");
            return;
        }

        // Удаляем все дочерние объекты с коллайдерами
        RemoveOldWallColliders();

        // Получаем все занятые позиции тайлов
        BoundsInt bounds = wallTilemap.cellBounds;
        TileBase[] allTiles = wallTilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    // Создаем коллайдер для каждого тайла стены
                    Vector3Int tilePosition = new Vector3Int(x + bounds.xMin, y + bounds.yMin, 0);
                    CreateWallCollider(tilePosition);
                }
            }
        }
    }
    
    private void RemoveOldWallColliders()
    {
        // Удаляем все дочерние объекты, которые содержат коллайдеры стен
        // Используем обратный цикл, так как мы удаляем элементы
        for (int i = wallTilemap.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = wallTilemap.transform.GetChild(i);
            if (child.name.StartsWith("WallCollider_") || child.GetComponent<Collider2D>() != null)
            {
                // Уничтожаем в редакторе или во время игры
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
    
    private void CreateWallCollider(Vector3Int tilePosition)
    {
        GameObject colliderObj = new GameObject("WallCollider_" + tilePosition.x + "_" + tilePosition.y);
        colliderObj.transform.SetParent(wallTilemap.transform);
        colliderObj.transform.position = wallTilemap.CellToWorld(tilePosition) + wallTilemap.cellSize / 2;

        // 1. Добавляем коллайдер
        BoxCollider2D boxCollider = colliderObj.AddComponent<BoxCollider2D>();
        boxCollider.size = wallTilemap.cellSize;

        // 2. Применяем физический материал (если есть)
        if (wallPhysicsMaterial != null)
            boxCollider.sharedMaterial = wallPhysicsMaterial;

        // 3. Добавляем Shadow Caster 2D
        ShadowCaster2D shadowCaster = colliderObj.AddComponent<ShadowCaster2D>();
    
        // (Опционально) Настраиваем форму тени (если нужно что-то сложнее прямоугольника)
        shadowCaster.useRendererSilhouette = false; // Используем форму коллайдера
        
        
        shadowCaster.selfShadows = true; // Включаем самозатенение

        colliderObj.tag = "Wall";
        colliderObj.layer = LayerMask.NameToLayer("Walls");
    }

    private T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        var component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }
        return component;
    }
    
    public void FillBackgroundAroundDungeon(HashSet<Vector2Int> floorPositions, int padding = 15)
    {
        if (floorPositions.Count == 0) return;

        // Находим границы данжа
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var position in floorPositions)
        {
            minX = Mathf.Min(minX, position.x);
            maxX = Mathf.Max(maxX, position.x);
            minY = Mathf.Min(minY, position.y);
            maxY = Mathf.Max(maxY, position.y);
        }

        // Расширяем границы с учетом отступа
        minX -= padding;
        maxX += padding;
        minY -= padding;
        maxY += padding;

        // Заполняем все пространство фоновым тайлом
        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                var position = new Vector2Int(x, y);
                // Если это не пол и не стена, заливаем фоном
                if (!floorPositions.Contains(position) && 
                    !wallTilemap.HasTile((Vector3Int)position))
                {
                    PaintSingleTile(floorTilemap, backgroundTile, position);
                }
            }
        }
    }
    
    
}