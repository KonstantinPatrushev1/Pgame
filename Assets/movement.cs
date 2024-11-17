using UnityEngine;

public class movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;
    private Rigidbody2D rb;
    private Vector2 movementt;
    private SwordAttack swordAttack; // Ссылка на объект SwordAttack
    private Vector2 velocity;
    private bool isAttacking = false; // Флаг для проверки, атакует ли персонаж

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        swordAttack = GetComponent<SwordAttack>(); // Получаем ссылку на компонент SwordAttack
    }

    void Update()
    {
        // Если персонаж атакует, движение блокируется
        if (isAttacking) return;

        movementt.x = Input.GetAxisRaw("Horizontal");
        movementt.y = Input.GetAxisRaw("Vertical");

        // Обновляем направление персонажа в SwordAttack
        if (movementt.x > 0)
            swordAttack.currentDirection = SwordAttack.Direction.Right;
        else if (movementt.x < 0)
            swordAttack.currentDirection = SwordAttack.Direction.Left;
        else if (movementt.y > 0)
            swordAttack.currentDirection = SwordAttack.Direction.Up;
        else if (movementt.y < 0)
            swordAttack.currentDirection = SwordAttack.Direction.Down;

        // Обновляем velocity с movementt
        velocity = movementt;

        UpdateAnimation();

        // Проверка на нажатие ЛКМ для атаки
        if (Input.GetMouseButtonDown(0) && !isAttacking) // Атака только если не атакует
        {
            StartAttack();
        }
    }

    void FixedUpdate()
    {
        // Если не атакует, то персонаж двигается
        if (!isAttacking)
        {
            rb.velocity = movementt.normalized * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero; // Прекращаем движение при атаке
        }
    }

    void UpdateAnimation()
    {
        if (velocity != Vector2.zero && !isAttacking)
        {
            animator.SetBool("Walking", true);
            animator.SetFloat("Horizontal", velocity.x);
            animator.SetFloat("Vertical", velocity.y);
        }
        else
        {
            animator.SetBool("Walking", false);
        }

        if (isAttacking)
        {
            animator.SetBool("Attack", true);
        }
        else
        {
            animator.SetBool("Attack", false);
        }
    }

    void StartAttack()
    {
        if (isAttacking) return; // Если уже атакует, не запускаем атаку

        isAttacking = true; // Включаем флаг атаки
        animator.SetTrigger("Attack"); // Воспроизведение анимации удара
        Invoke("EndAttack", 0.5f); // Завершаем атаку через 1 секунду (время длины анимации)
    }

    void EndAttack()
    {
        isAttacking = false; // Завершаем атаку, разрешаем движение и следующую атаку
    }
}
