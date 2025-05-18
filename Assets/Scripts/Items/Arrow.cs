using UnityEngine;
using UnityEngine.Tilemaps;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;
    
    private Vector2 direction;
    private float currentLifetime;
    private GameObject owner;
    private GameObject bow;
    public float damage = 30;

    public void Initialize(Vector2 shootDirection, GameObject owner, GameObject bow)
    {
        direction = shootDirection.normalized;
        this.owner = owner;
        this.bow = bow;
        currentLifetime = lifetime;
        
        // Поворачиваем стрелу в направлении полета
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        // Движение стрелы с постоянной скоростью
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        
        currentLifetime -= Time.deltaTime;
        if (currentLifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null || other.gameObject == null)
            return;

        // Если это НЕ BoxCollider2D — игнорируем
        if (!(other is BoxCollider2D) && !(other is TilemapCollider2D)) // или other.GetType() != typeof(BoxCollider2D)
            return;

        // Если попали во врага (Enemy) или не в игрока/лук/сундук — уничтожаем стрелу
        if (other.CompareTag("Enemy") || 
            (!other.CompareTag("Player") && !other.CompareTag("Bow") && !other.CompareTag("ChestZone")))
        {
            Debug.Log($"Стрела попала в BoxCollider2D: {other.gameObject.name}");
            Destroy(gameObject);
        }
        
        if (other.CompareTag("Enemy"))
        {
            // Получаем компонент Slime у объекта, с которым произошло столкновение
            Slime slime = other.GetComponent<Slime>();
            if (slime != null)
            {
                // Проверяем, прошло ли достаточно времени для следующего удара
                if (Time.time - slime.lastHitTime >= slime.damageCooldown)
                {
                    // Уменьшаем здоровье слайма
                    slime.hp -= damage;

                    // Направление от меча к слайму для отталкивания
                    Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;

                    // Отталкиваем слайма
                    slime.Knockback(knockbackDirection, 3f, 0.15f);

                    // Обновляем время последнего удара
                    slime.lastHitTime = Time.time;
                }
            }
        }
    }
}