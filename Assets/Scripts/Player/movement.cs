using System.Collections;
using Save;
using UnityEngine;

public class movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Animator animator;
    public Animator eyesAnimator;
    public Animator hatAnimator;
    public Animator pantsAnimator;
    public Animator shirtAnimator;
    private Rigidbody2D rb;
    public Vector2 movementInput;
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
    public Inventory inventory;
    public MapController mapController;

    private ChestZones chestZones;
    private Storage storage;
    public GameData gameData;
    public GameObject PausePanel;
    
    public GameObject StatuePanel;
    public GameObject SavePanel;
    public GameObject TeleportPanel;

    void Start()
    {
        storage = new Storage();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
{
    // Если инвентарь открыт
    if (inventory.IsInventoryOpen 
        || PausePanel.activeSelf
        || StatuePanel.activeSelf 
        || SavePanel.activeSelf 
        || TeleportPanel.activeSelf)  
    {
        // Останавливаем анимацию атаки
        if (isAttacking)
        {
            animator.SetBool("Attack", false);
            eyesAnimator.SetBool("Attack", false);
            hatAnimator.SetBool("Attack", false);
            pantsAnimator.SetBool("Attack", false);
            shirtAnimator.SetBool("Attack", false);
            isAttacking = false;  // Останавливаем атаку
        }

        // Останавливаем анимацию бега и не обновляем скорость
        animator.SetBool("Walking", false);
        eyesAnimator.SetBool("Walking", false);
        hatAnimator.SetBool("Walking", false);
        pantsAnimator.SetBool("Walking", false);
        shirtAnimator.SetBool("Walking", false);
        return;  // Прекращаем выполнение, чтобы не было лишней логики
    }

    // Если персонаж атакует или находится в отталкивании, ничего не делаем
    if (isAttacking || isKnockedBack) return;

    movementInput.x = Input.GetAxisRaw("Horizontal");
    movementInput.y = Input.GetAxisRaw("Vertical");
    horizontal = animator.GetFloat("Horizontal");
    vertical = animator.GetFloat("Vertical");

    velocity = movementInput;

    UpdateAnimation();

    // Проверка на атаку, если карта не открыта
    if (!mapController.IsMapOpen && Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown)
    {
        StartAttack();
    }
}

void FixedUpdate()
{
    if (isKnockedBack)  // Если отталкивание активно
    {
        rb.velocity = knockbackDirection * (moveSpeed * 2f);  // Применяем отталкивание
        knockbackTimeRemaining -= Time.deltaTime;

        if (knockbackTimeRemaining <= 0)
        {
            isKnockedBack = false;  // Завершаем отталкивание
        }
        return;  // Прекращаем дальнейшую обработку
    }

    if (inventory.IsInventoryOpen)  // Если инвентарь открыт
    {
        rb.velocity = Vector2.zero;  // Останавливаем движение
        return;  // Не продолжаем обработку движения
    }

    if (!isAttacking)
    {
        rb.velocity = movementInput.normalized * moveSpeed;
    }
    else
    {
        rb.velocity = Vector2.zero;
    }
}

public void UpdateAnimation()
{
    if (velocity != Vector2.zero && !isAttacking && !inventory.IsInventoryOpen)
    {
        animator.SetBool("Walking", true);
        eyesAnimator.SetBool("Walking", true);
        hatAnimator.SetBool("Walking", true);
        pantsAnimator.SetBool("Walking", true);
        shirtAnimator.SetBool("Walking", true);
        animator.SetFloat("Horizontal", velocity.x);
        animator.SetFloat("Vertical", velocity.y);
        eyesAnimator.SetFloat("Horizontal", velocity.x);
        eyesAnimator.SetFloat("Vertical", velocity.y);
        hatAnimator.SetFloat("Horizontal", velocity.x);
        hatAnimator.SetFloat("Vertical", velocity.y);
        pantsAnimator.SetFloat("Horizontal", velocity.x);
        pantsAnimator.SetFloat("Vertical", velocity.y);
        shirtAnimator.SetFloat("Horizontal", velocity.x);
        shirtAnimator.SetFloat("Vertical", velocity.y);
    }
    else
    {
        animator.SetBool("Walking", false);
        eyesAnimator.SetBool("Walking", false);
        hatAnimator.SetBool("Walking", false);
        pantsAnimator.SetBool("Walking", false);
        shirtAnimator.SetBool("Walking", false);
    }

    animator.SetBool("Attack", isAttacking);
    eyesAnimator.SetBool("Attack", isAttacking);
    hatAnimator.SetBool("Attack", isAttacking);
    pantsAnimator.SetBool("Attack", isAttacking);
    shirtAnimator.SetBool("Attack", isAttacking);

}

void StartAttack()
{
    WeaponManager weaponManager = GetComponent<WeaponManager>();
    if (weaponManager != null && weaponManager.weaponId == -1) return;
    if (isAttacking) return; 

    isAttacking = true; 
    animator.SetTrigger("Attack");
    eyesAnimator.SetTrigger("Attack");// Запуск анимации удара
    hatAnimator.SetTrigger("Attack");
    pantsAnimator.SetTrigger("Attack");
    shirtAnimator.SetTrigger("Attack");
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
