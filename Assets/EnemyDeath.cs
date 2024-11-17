using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, является ли объект врагом
        if (other.CompareTag("Enemy"))
        {
            // Получаем компонент Slime и вызываем метод Die
            Slime slime = other.GetComponent<Slime>();
            if (slime != null)
            {
                slime.Die();
            }
        }
    }
}