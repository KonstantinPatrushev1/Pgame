using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public float damageValue;
    public float knockbackDistance = 3f; // Расстояние отлета
    public float knockbackDuration = 0.15f; // Время остановки движения

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Slime slime = other.GetComponent<Slime>();
            if (slime != null)
            {
                slime.Damage(damageValue);

                // Направление отталкивания
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;

                // Отталкиваем слайм
                slime.Knockback(knockbackDirection, knockbackDistance, knockbackDuration);
            }
        }
    }
}