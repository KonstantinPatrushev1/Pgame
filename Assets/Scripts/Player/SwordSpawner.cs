using Unity.VisualScripting;
using UnityEngine;

public class SwordSpawner : MonoBehaviour
{
    private SpriteRenderer swordRenderer; // Рендер меча
    public SpriteRenderer characterRenderer;
    private Sprite swordSprite;  // Спрайт меча
    private float rotationSpeed = 500f;  // Скорость вращения меча
    public Vector3 swordScale; // Масштаб меча
    private float damage;
    private GameObject sword;  // Объект меча
    private PolygonCollider2D swordCollider;

    private Animator animator; // Ссылка на Animator
    private float horizontal;  // Параметр Horizontal из Animator
    private float vertical;    // Параметр Vertical из Animator
    private int direction = 3;
    private float angle;
    private float distanceFromCenter = 0.05f; // Расстояние меча от персонажа

    private bool isRotating = false; // Флаг для вращения меча
    private float rotationTimer = 0f; // Таймер для отсчета времени вращения
    private float rotationDuration = 0.3f; // Длительность вращения меча в секундах

    private float attackCooldown = 0.5f; // Задержка между атаками
    private float attackTimer = 0f; // Таймер отсчета задержки

    private float temp = -1;
    public WeaponManager weaponManager;
    public PlayerInfo playerInfo;
    public Inventory inventory;
    public MapController mapController;
    public GameObject PausePanel;
    
    public GameObject StatuePanel;
    public GameObject SavePanel;
    public GameObject TeleportPanel;


    void Start()
    {
        animator = GetComponent<Animator>();

        // Создаем объект меча
        sword = new GameObject("Sword");
        sword.tag = "Sword";

        // Добавляем спрайт к объекту меча
        swordRenderer = sword.AddComponent<SpriteRenderer>();
        swordRenderer.sprite = swordSprite;
        
        swordRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // Устанавливаем order in layer
        swordRenderer.sortingOrder = -2;
        
        // Добавляем PolygonCollider2D
        swordCollider = sword.AddComponent<PolygonCollider2D>();
        swordCollider.isTrigger = true;
        

        // Делаем меч дочерним объектом персонажа
        sword.transform.SetParent(transform);
        
        swordScale = sword.transform.localScale;

        // Устанавливаем начальное положение меча
        sword.transform.localPosition = new Vector3(distanceFromCenter, 0, 0);

        // Скрываем меч
        sword.SetActive(false);
    }

    void Update()
{
    if (WeaponManager.instance.weaponId == 7)
    {
        sword.tag = "Pickaxe";
    }
    else
    {
        sword.tag = "Sword";
    }
    UpdateSwordSortingOrder();
    // Если инвентарь или карта открыты, проверяем состояние анимации
    if (inventory.IsInventoryOpen 
        || mapController.IsMapOpen 
        || PausePanel.activeSelf 
        || StatuePanel.activeSelf 
        || SavePanel.activeSelf 
        || TeleportPanel.activeSelf)
    {
        // Если атака всё еще идет, не блокируем управление
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            // Если атака ещё не завершена, не блокируем управление
            return;
        }

        // Останавливаем вращение меча и скрываем его, если инвентарь открыт
        isRotating = false;
        sword.SetActive(false);  // Скрываем меч

        // Сбрасываем таймер атаки, если инвентарь или карта закрыты
        attackTimer = 0f;

        return;  // Прекращаем выполнение метода
    }

    void UpdateSwordSortingOrder()
    {
        // Основной расчёт sortingOrder на основе Y координаты
        if (sword.transform.position.y > transform.position.y)
        {
            swordRenderer.sortingOrder = -1 * Mathf.RoundToInt(transform.position.y * 100f);
        }
        else
        {
            swordRenderer.sortingOrder = -1 * Mathf.RoundToInt(transform.position.y * 100f) + 2;
        }
    }

    // Логика удара
    horizontal = animator.GetFloat("Horizontal");
    vertical = animator.GetFloat("Vertical");

    // Обновляем таймер атаки
    attackTimer += Time.deltaTime;

    // Обрабатываем нажатие ЛКМ
    if (Input.GetMouseButtonDown(0) && !isRotating && attackTimer >= attackCooldown)
    {
        // Если оружие выбрано
        if (HasWeaponEquipped())
        {
            // Сбрасываем таймер задержки атаки
            attackTimer = 0f;

            // Показываем меч
            sword.SetActive(true);

            // Определяем направление
            SetDirection();

            // Устанавливаем начальное положение и вращение меча
            SetSwordPositionAndRotation();

            // Активируем вращение
            isRotating = true;
            rotationTimer = 0f; // Сбрасываем таймер
            if (isRotating) 
            {
                playerInfo.SetStamina(10);  // Тратим стамину только если оружие выбрано и вращение активно
            }
        }
    }
    
    // Выполняем вращение, если активен флаг
    if (isRotating)
    {
        rotationTimer += Time.deltaTime;

        if (rotationTimer < rotationDuration)
        {
            // Вращаем меч
            sword.transform.RotateAround(transform.position, Vector3.forward, temp * rotationSpeed * Time.deltaTime);
        }
        else
        {
            // Останавливаем вращение и скрываем меч
            isRotating = false;
            sword.SetActive(false);
        }
    }
    
}
    private bool HasWeaponEquipped()
    {
        return WeaponManager.instance.weaponId != -1 && WeaponManager.instance.weaponId != 6;
    }





    public void SetSwordSprite(Sprite newSprite)
    {
        if (swordRenderer != null)
        {
            swordRenderer.sprite = newSprite;
        }
    }
    public void UpdateCollider(Vector2[] newPath)
    {
        if (swordCollider != null)
        {
            swordCollider.SetPath(0, newPath);
        }
    }
    public void UpdateGamage(float newgamage)
    {

        damage = newgamage;
    }
    
    private void SetDirection()
    {
        sword.transform.localScale = swordScale;
        if (horizontal < 0)
        {
            direction = 0; // Влево
            temp = 1;

            // Отзеркаливаем меч по оси X
            sword.transform.localScale = new Vector3(sword.transform.localScale.x, -Mathf.Abs(sword.transform.localScale.y), sword.transform.localScale.z);
        }
        else if (horizontal > 0)
        {
            direction = 3; // Вправо
            temp = -1;
        }
        else if (vertical > 0)
        {
            direction = 2; // Вверх
            temp = -1;
        }
        else if (vertical < 0)
        {
            direction = 1; // Вниз
            temp = -1;
        }
    }

    private void SetSwordPositionAndRotation()
    {
        ResetSwordPosition();

        switch (direction)
        {
            case 0: // Влево
                angle = -30 * 7.2f;
                break;
            case 1: // Вниз
                angle = -6 * 7.2f;
                break;
            case 2: // Вверх
                angle = -31 * 7.2f;
                break;
            case 3: // Вправо
                angle = -45 * 7.2f;
                break;
        }

        sword.transform.RotateAround(transform.position, Vector3.forward, angle);
    }

    private void ResetSwordPosition()
    {
        sword.transform.localPosition = new Vector3(distanceFromCenter, 0, 0); // Начальное положение
        sword.transform.localRotation = Quaternion.identity; // Сбрасываем вращение
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, что объект с которым столкнулся меч имеет тег "Enemy"
        if (collision.CompareTag("Enemy"))
        {
            // Получаем компонент Slime у объекта, с которым произошло столкновение
            Slime slime = collision.GetComponent<Slime>();
            if (slime != null)
            {
                // Проверяем, прошло ли достаточно времени для следующего удара
                if (Time.time - slime.lastHitTime >= slime.damageCooldown)
                {
                    // Уменьшаем здоровье слайма
                    slime.hp -= damage;

                    // Направление от меча к слайму для отталкивания
                    Vector2 knockbackDirection = (collision.transform.position - sword.transform.position).normalized;

                    // Отталкиваем слайма
                    slime.Knockback(knockbackDirection, 3f, 0.15f);

                    // Обновляем время последнего удара
                    slime.lastHitTime = Time.time;
                }
            }
        }
    }


}

