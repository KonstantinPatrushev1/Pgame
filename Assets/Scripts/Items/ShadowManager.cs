using UnityEngine;
using System.Collections.Generic;

public class ShadowManager : MonoBehaviour
{
    public static ShadowManager Instance;
    private Dictionary<Vector2Int, GameObject> fixedShadows = new Dictionary<Vector2Int, GameObject>();
    public float gridSize = 0.5f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterFixedShadow(Vector3 position, GameObject shadow)
    {
        Vector2Int gridPos = PositionToGrid(position);
        
        if (fixedShadows.ContainsKey(gridPos))
        {
            // Если тень уже есть в этой позиции - уничтожаем новую
            Destroy(shadow);
        }
        else
        {
            // Регистрируем новую фиксированную тень
            fixedShadows.Add(gridPos, shadow);
        }
    }

    public Vector2Int PositionToGrid(Vector3 position) => new Vector2Int(
        Mathf.RoundToInt(position.x / gridSize),
        Mathf.RoundToInt(position.y / gridSize)
    );
    
    
    public void UnregisterShadow(GameObject shadow)
    {
        List<Vector2Int> keysToRemove = new List<Vector2Int>();
        
        foreach (var pair in fixedShadows)
        {
            if (pair.Value == shadow)
            {
                keysToRemove.Add(pair.Key);
            }
        }
        
        foreach (var key in keysToRemove)
        {
            fixedShadows.Remove(key);
        }
    }
}