using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public int itemID;
    public int count;
    public bool istool;

    private float amplitude = 0.1f; // Высота колебания
    private float frequency = 2f; // Частота колебания

    public bool startSwaying = false;
    private Vector3 startPosition;
    public Vector3 dropEndPosition;

    private Transform visualObject;

    public Inventory inventory;  // Ссылка на инвентарь

    private bool canBePickedUp = false; // Флаг для проверки, можно ли подобрать предмет
    private bool hasLanded = false; // Флаг, чтобы знать, что предмет упал

    private float timeToBePickedUp = 0f; // Задержка перед тем, как предмет можно будет подобрать (после падения)
    private float pickupTimer = 0f; // Таймер для отсчета времени

    private bool isFirstDrop = true;  // Флаг, чтобы отслеживать первый бросок

    void OnEnable()
    {
        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
        }
        if (visualObject == null)
        {
            visualObject = this.transform;
        }
        startPosition = transform.position;

        // Сразу начинаем отсчитывать время до того, как предмет можно будет подобрать
        pickupTimer = 0f; // Сбрасываем таймер
        hasLanded = false; // Сбрасываем флаг приземления
        canBePickedUp = false; // Предмет не доступен для подбора сразу

        // Устанавливаем размеры коллайдера по размеру текстуры
        AdjustColliderToSprite();
    }

    void Update()
    {
        // Проверка на анимацию колебания
        if (startSwaying)
        {
            float newY = dropEndPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;
            visualObject.position = new Vector3(dropEndPosition.x, newY, dropEndPosition.z);
        }

        // Когда предмет упал на землю, начинаем отсчитывать время для подбора
        if (hasLanded)
        {
            if (isFirstDrop)
            {
                pickupTimer += Time.deltaTime;
                if (pickupTimer >= timeToBePickedUp)
                {
                    canBePickedUp = true;
                    isFirstDrop = false; // После первого броска задержка больше не требуется
                }
            }
            else
            {
                // Если это не первый бросок, разрешаем подбирать сразу
                canBePickedUp = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canBePickedUp)
        {
            if (istool)
            {
                count = 1;
            }
            // Добавляем предмет в инвентарь
            inventory.AddItemToInventory(this);

            // Уничтожаем объект
            Destroy(gameObject);
        }
    }

    // Метод для настройки коллайдера по размеру спрайта
    private void AdjustColliderToSprite()
    {
        // Получаем компонент SpriteRenderer
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            // Получаем размер текстуры (спрайта)
            Vector2 spriteSize = spriteRenderer.size;

            // Получаем BoxCollider2D
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

            if (boxCollider != null)
            {
                // Устанавливаем размер коллайдера по размеру спрайта
                boxCollider.size = spriteSize;
            }
            else
            {
                // Если коллайдера нет, создаем его
                boxCollider = gameObject.AddComponent<BoxCollider2D>();
                boxCollider.size = spriteSize;
            }
        }
        
    }
    

    // Метод, который будет вызываться после анимации падения, чтобы флаг "упал на землю"
    public void MarkAsLanded()
    {
        hasLanded = true;
        pickupTimer = 0f; // Сброс таймера сразу после приземления
        timeToBePickedUp = 0f; // Установите минимальное время задержки для первого падения
    }
}
