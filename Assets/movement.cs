using System.Collections;
using UnityEngine;

public class movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 velocity;
    private bool isAttacking = false; 
    private float horizontal;  
    private float vertical;    
    private float lastAttackTime = -1f;  
    public float attackCooldown = 0.5f;  
    private bool isKnockedBack = false;  // Флаг для отслеживания отталкивания
    private Vector2 knockbackDirection;  // Направление отталкивания
    private float knockbackTimeRemaining;  // Время до окончания отталкивания

    public float knockbackDistance = 3f; // Расстояние отлета
    public float knockbackDuration = 0.15f; // Время отталкивания

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isAttacking || isKnockedBack) return;

        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");
        horizontal = animator.GetFloat("Horizontal");
        vertical = animator.GetFloat("Vertical");
        

        velocity = movementInput;

        UpdateAnimation();

        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown)
        {
            StartAttack();
        }
    }

    void FixedUpdate()
    {
        if (!isAttacking)
        {
            rb.velocity = movementInput.normalized * moveSpeed;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
        if (isKnockedBack)
        {
            rb.velocity = knockbackDirection * (moveSpeed * 2f);  // Применяем отталкивание
            knockbackTimeRemaining -= Time.deltaTime;

            if (knockbackTimeRemaining <= 0)
            {
                isKnockedBack = false;  // Окончание отталкивания
            }
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

        animator.SetBool("Attack", isAttacking);
    }

    void StartAttack()
    {
        WeaponManager weaponManager = GetComponent<WeaponManager>();
        if (weaponManager != null && weaponManager.weaponId == -1) return;
        if (isAttacking) return; 

        isAttacking = true; 
        animator.SetTrigger("Attack"); 
        lastAttackTime = Time.time;

        float attackDuration = 0.25f;  
        if (vertical < 0 && horizontal == 0)
        {
            Invoke(nameof(EndAttack), attackDuration - 0.07f);
        }
        else
        {
            Invoke(nameof(EndAttack), attackDuration);
        }
    }

    void EndAttack()
    {
        isAttacking = false;
    }

    private IEnumerator DisableMovement(float duration)
    {
        float originalSpeed = moveSpeed;
        moveSpeed = 0; // Останавливаем движение
        yield return new WaitForSeconds(duration);
        moveSpeed = originalSpeed; // Восстанавливаем движение
    }
    public void ApplyKnockback(Vector2 direction, float distance, float duration)
    {
        isKnockedBack = true;
        knockbackDirection = direction.normalized;
        knockbackTimeRemaining = duration;
    }
}
