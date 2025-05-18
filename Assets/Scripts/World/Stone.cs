using UnityEngine;

public class Stone : MonoBehaviour
{
    public int maxHP = 3;
    public float damageCooldown = 0.5f;
    public float shakeDuration = 0.2f;
    public float shakeIntensity = 0.1f;
    
    [Header("Drop Settings")]
    public GameObject dropItemPrefab; // Префаб DroppedItem
    public int dropItemID; // ID предмета, который выпадет
    public int dropCount = 1; // Количество выпадающих предметов
    public bool isTool = false; // Является ли предмет инструментом

    private int currentHP;
    private float lastDamageTime;
    private Vector3 originalPosition;
    private bool isShaking = false;
    private float shakeTimer = 0f;
    
    public DataBase data;

    void Start()
    {
        currentHP = maxHP;
        lastDamageTime = -damageCooldown;
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isShaking)
        {
            ShakeEffect();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pickaxe"))
        {
            TryTakeDamage();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Pickaxe"))
        {
            TryTakeDamage();
        }
    }

    void TryTakeDamage()
    {
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            TakeDamage();
            lastDamageTime = Time.time;
        }
    }

    void TakeDamage()
    {
        currentHP--;
        StartShaking();
        
        if (currentHP <= 0)
        {
            DropItem();
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Камень получил урон! Осталось HP: " + currentHP);
        }
    }

    void DropItem()
    {
        if (dropItemPrefab != null)
        {
            // Создаем предмет
            GameObject droppedItem = Instantiate(dropItemPrefab, transform.position, Quaternion.identity);
            
            droppedItem.GetComponent<SpriteRenderer>().sortingOrder = -32766;
        
            // Получаем компоненты
            DroppedItem itemScript = droppedItem.GetComponent<DroppedItem>();
            SpriteRenderer sr = droppedItem.GetComponent<SpriteRenderer>();
        
            // Настраиваем спрайт
            sr.sprite = data.items[dropItemID].img;
        
            if (itemScript != null)
            {
                // Корректируем позицию дропа - немного ниже камня
                Vector3 dropPosition = transform.position;
                dropPosition.y -= 0.2f; // Можно регулировать это значение
            
                itemScript.itemID = dropItemID;
                itemScript.count = dropCount;
                itemScript.istool = isTool;
                itemScript.dropEndPosition = dropPosition; // Используем скорректированную позицию
                itemScript.startSwaying = true;
                itemScript.MarkAsLanded();
            }
        
            // Масштабируем
            droppedItem.transform.localScale *= 0.5f;
        }
    }

    void StartShaking()
    {
        originalPosition = transform.position;
        isShaking = true;
        shakeTimer = shakeDuration;
    }

    void ShakeEffect()
    {
        if (shakeTimer > 0)
        {
            transform.position = originalPosition + Random.insideUnitSphere * shakeIntensity;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            isShaking = false;
            transform.position = originalPosition;
        }
    }
}