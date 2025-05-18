using UnityEngine;

public class EyesSorting : MonoBehaviour
{
    private SpriteRenderer objectRenderer;

    public float sortingMultiplier = 100f; // Множитель для точности сортировки

    void Start()
    {
        // Получаем компонент SpriteRenderer объекта
        objectRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Устанавливаем sortingOrder на основе позиции по оси Y
        // Умножаем Y на множитель, чтобы получить уникальное значение для каждого объекта
        objectRenderer.sortingOrder = (-1 * Mathf.RoundToInt(transform.position.y * sortingMultiplier)) + 1;
    }
}