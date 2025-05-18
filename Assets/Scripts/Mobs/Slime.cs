using UnityEngine;
using System.Collections;

public class Slime : MonoBehaviour
{
    public float moveSpeed = 3f; // Нормальная скорость слайма
    public float detectionRadius = 5f; // Радиус обнаружения игрока
    public float changeDirectionInterval = 2f; // Интервал для смены случайного направления
    public float hp; // Здоровье слайма
    public float lastHitTime = 0f; // Время последнего получения урона
    public float damageCooldown = 0.3f; // Время, через которое слайм может получать следующий урон

    // Параметры для отталкивания
    public float knockbackDistance = 3f; // Расстояние отлета
    public float knockbackDuration = 0.15f; // Время остановки движения
    public float knockbackForceMultiplier = 1f; // Множитель силы отталкивания

    private float normalSpeed; // Для хранения нормальной скорости
    private Transform player;
    private Vector2 randomDirection;
    private float timeToChangeDirection;
    public float pushForce = 10f;
    public float enemydamage;

    private Rigidbody2D rb; // Rigidbody2D компонента

    void Start()
    {
        normalSpeed = moveSpeed;
        player = GameObject.FindWithTag("Player").transform; // Находим игрока
        timeToChangeDirection = changeDirectionInterval;
        randomDirection = Random.insideUnitCircle.normalized; // Генерируем случайное направление
        rb = GetComponent<Rigidbody2D>(); // Получаем Rigidbody2D
    }

    void Update()
    {
        if (moveSpeed == 0) return; // Если движение временно отключено, ничего не делаем

        timeToChangeDirection -= Time.deltaTime;

        if (timeToChangeDirection <= 0)
        {
            randomDirection = Random.insideUnitCircle.normalized; // Генерируем новое направление
            timeToChangeDirection = changeDirectionInterval;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            // Если игрок близко, следуем за ним
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Если игрок далеко, двигаемся случайно
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + randomDirection, moveSpeed * Time.deltaTime);
        }
    }
    

    private void DestroySlime()
    {
        Destroy(gameObject); // Уничтожаем объект
    }

    // Универсальный метод для отталкивания
    public void Knockback(Vector2 direction, float distance, float duration)
    {
        if (rb != null)
        {
            float force = rb.mass * distance / duration * knockbackForceMultiplier;

            // Сбрасываем скорость перед применением силы
            rb.velocity = Vector2.zero;
            rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

            // Временно отключаем движение
            StartCoroutine(DisableMovement(duration));
            if (hp <= 0)
            {
                DestroySlime();
            }
        }
    }


    private IEnumerator DisableMovement(float duration)
    {
        moveSpeed = 0; // Останавливаем движение
        yield return new WaitForSeconds(duration); // Ждем окончания "парализации"

        rb.velocity = Vector2.zero; // Сбрасываем физическую скорость
        moveSpeed = normalSpeed; // Восстанавливаем скорость
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем, что объект имеет тег "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Находим направление от объекта к игроку
            Vector2 pushDirection = collision.transform.position - transform.position;

            // Нормализуем вектор и отталкиваем игрока
            movement playerMovement = collision.gameObject.GetComponent<movement>();
            PlayerInfo playerinfo = collision.gameObject.GetComponent<PlayerInfo>();
            if (playerMovement != null)
            {
                playerinfo.SetDamage(enemydamage);
                playerMovement.ApplyKnockback(pushDirection, knockbackDistance, knockbackDuration); // Применяем отталкивание
            }
        }
    }

}
