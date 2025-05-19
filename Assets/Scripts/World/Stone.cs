using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
    
    private DataBase data;
    
    void Start()
    {
        data = FindObjectOfType<DataBase>();
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
                // Генерируем случайное направление
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                // Слегка увеличиваем вероятность выброса вверх
                randomDirection.y = Mathf.Abs(randomDirection.y) * 0.7f;
            
                // Инициализируем позицию
                itemScript.Initialize(transform.position, randomDirection.y, true);
            
                // Настраиваем предмет
                itemScript.itemID = dropItemID;
                itemScript.count = Random.Range(Mathf.Max(1, dropCount-2), dropCount+2);
                itemScript.istool = isTool;
            
                // Применяем силу броска
                float throwForce = Random.Range(3f, 6f); // Сила броска
                itemScript.ApplyThrowForce(randomDirection, throwForce);
            }
    
            // Масштабируем
            droppedItem.transform.localScale *= 0.7f;
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