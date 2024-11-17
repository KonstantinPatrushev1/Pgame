using UnityEngine;

public class Slime : MonoBehaviour
{
    public float moveSpeed = 3f; // Скорость движения слайма
    public float detectionRadius = 5f; // Радиус, на котором слайм начнет двигаться к игроку
    public float changeDirectionInterval = 2f; // Интервал для смены случайного направления

    private Transform player;
    private Vector2 randomDirection;
    private float timeToChangeDirection;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform; // Находим игрока по тегу
        timeToChangeDirection = changeDirectionInterval;
        randomDirection = Random.insideUnitCircle.normalized; // Случайное направление
    }

    void Update()
    {
        timeToChangeDirection -= Time.deltaTime;

        if (timeToChangeDirection <= 0)
        {
            randomDirection = Random.insideUnitCircle.normalized; // Генерируем новое случайное направление
            timeToChangeDirection = changeDirectionInterval;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position); // Расстояние до игрока

        if (distanceToPlayer <= detectionRadius)
        {
            // Если игрок близко, слайм движется к нему
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            // Если игрок далеко, слайм двигается в случайном направлении
            transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + randomDirection, moveSpeed * Time.deltaTime);
        }
    }
    
    public void Die()
    {
        Debug.Log("Slime is dying!"); // Выводим сообщение в консоль
        Destroy(gameObject); // Уничтожаем объект слайма
    }
}
