using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public float pushForce = 10f; // Сила отталкивания

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем, что столкновение произошло с объектом с тегом "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Получаем направление от объекта врага
            Vector2 direction = (transform.position - collision.transform.position).normalized;
            
            // Получаем компонент Rigidbody2D игрока
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            
            // Применяем силу отталкивания в противоположную сторону
            rb.AddForce(direction * pushForce, ForceMode2D.Impulse);
        }
    }
}