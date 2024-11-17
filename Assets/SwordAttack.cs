using System.Collections;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public float attackRange = 1f; // Дистанция удара
    public float attackCooldown = 0.5f; // Время между ударами
    private bool canAttack = true;

    public enum Direction { Up, Down, Left, Right }
    public Direction currentDirection = Direction.Right;

    private GameObject swordColliderObject;
    private BoxCollider2D swordCollider;

    void Start()
    {
        // Создаем объект для коллайдера
        swordColliderObject = new GameObject("SwordCollider");
        swordCollider = swordColliderObject.AddComponent<BoxCollider2D>();
        swordCollider.isTrigger = true; // Устанавливаем как Trigger для проверки столкновений
        swordColliderObject.SetActive(false); // Изначально коллайдер выключен

        // Добавляем обработку столкновений
        swordColliderObject.AddComponent<EnemyDeath>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack) // Левая кнопка мыши
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        canAttack = false;

        // Включаем коллайдер
        swordColliderObject.SetActive(true);

        // Позиционируем коллайдер в зависимости от направления
        Vector2 attackPosition = Vector2.zero;
        float squareSize = attackRange;

        switch (currentDirection)
        {
            case Direction.Up:
                attackPosition = new Vector2(0, attackRange);
                break;
            case Direction.Down:
                attackPosition = new Vector2(0, -attackRange);
                break;
            case Direction.Left:
                attackPosition = new Vector2(-attackRange, 0);
                break;
            case Direction.Right:
                attackPosition = new Vector2(attackRange, 0);
                break;
        }

        swordColliderObject.transform.position = (Vector2)transform.position + attackPosition;
        swordCollider.size = new Vector2(squareSize, squareSize);

        yield return new WaitForSeconds(0.1f); // Длительность удара
        swordColliderObject.SetActive(false);

        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}