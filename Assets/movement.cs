using UnityEngine;

public class movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movementt;
    private Animator animator;
    private SwordAttack swordAttack; // Ссылка на объект SwordAttack

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        swordAttack = GetComponent<SwordAttack>(); // Получаем ссылку на компонент SwordAttack
    }

    void Update()
    {
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
    }

    void FixedUpdate()
    {
        rb.velocity = movementt.normalized * moveSpeed;
    }
}
