using UnityEngine;

public class TransparentObject : MonoBehaviour
{
    public GameObject player; // Персонаж
    private SpriteRenderer spriteRenderer;
    private Color originalColor; // Оригинальный цвет спрайта
    private PolygonCollider2D treeCollider; // Коллайдер дерева

    public float transparencyFactor = 0.5f; // Уровень прозрачности (0 - полностью прозрачный, 1 - непрозрачный)

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        treeCollider = GetComponent<PolygonCollider2D>(); // Получаем PolygonCollider2D
    }

    private void Update()
    {
        // Проверяем, находится ли персонаж выше дерева по Y-координате
        if (player.transform.position.y > transform.position.y)
        {
            // Проверяем, находится ли персонаж внутри формы коллайдера дерева
            if (treeCollider.OverlapPoint(player.transform.position))
            {
                // Если персонаж заходит за объект, делаем его полупрозрачным
                Color transparentColor = originalColor;
                transparentColor.a = transparencyFactor;
                spriteRenderer.color = transparentColor;
            }
            else
            {
                // Восстанавливаем оригинальный цвет
                spriteRenderer.color = originalColor;
            }
        }
        else
        {
            // Если персонаж не выше объекта, восстанавливаем исходную непрозрачность
            spriteRenderer.color = originalColor;
        }
    }
}