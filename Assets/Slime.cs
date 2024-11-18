using UnityEngine;
using System.Collections;

public class Slime : MonoBehaviour
{
    public float moveSpeed = 3f; // Нормальная скорость слайма
    public float detectionRadius = 5f; // Радиус обнаружения игрока
    public float changeDirectionInterval = 2f; // Интервал для смены случайного направления
    public float hp; // Здоровье слайма

    private float normalSpeed; // Для хранения нормальной скорости
    private Transform player;
    private Vector2 randomDirection;
    private float timeToChangeDirection;

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

    public void Damage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Invoke("DestroySlime", 0.5f); // Уничтожаем слайма с задержкой
        }
    }

    private void DestroySlime()
    {
        Destroy(gameObject); // Уничтожаем объект
    }

    public void Knockback(Vector2 direction, float distance, float duration)
    {
        if (rb != null)
        {
            float force = rb.mass * distance / duration;

            // Сбрасываем скорость перед применением силы
            rb.velocity = Vector2.zero;
            rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);

            // Временно отключаем движение
            StartCoroutine(DisableMovement(duration));
        }
    }

    private IEnumerator DisableMovement(float duration)
    {
        moveSpeed = 0; // Останавливаем движение
        yield return new WaitForSeconds(duration); // Ждем окончания "парализации"

        rb.velocity = Vector2.zero; // Сбрасываем физическую скорость
        moveSpeed = normalSpeed; // Восстанавливаем скорость
    }
}