using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    public int itemID;
    public int count;
    public bool istool;

    private float amplitude = 0.002f;
    private float frequency = 2f;

    public bool startSwaying = false;
    private Vector3 startPosition;
    public Vector3 dropDirection;

    private Transform visualObject;
    public Inventory inventory;
    private bool canBePickedUp = false;
    private bool hasLanded = false;
    private float pickupTimer = 0f;
    private bool isFirstDrop = true;

    private Rigidbody2D rb;
    private bool physicsApplied = false;
    private float minHeight;
    private float maxHeight;
    public float defaultHeightRange = 0.5f;
    public float verticalThrowExtraRange = 1.5f;
    private float heightRange = 1f;
    private float drag = 4f;
    private float angularDrag = 2f;
    private bool isStopping = false;
    
    private Collider2D itemCollider;
    private Collider2D playerCollider;
    private Collider2D attractTargetCollider;
    
    // Новые переменные для притягивания
    private bool isAttracting = false;
    private Transform attractTarget;
    public float attractSpeed = 5f;
    public float attractStartDistance = 1f;
    
    [Header("Shadow Settings")]
    public GameObject shadowPrefab;
    public float shadowScale = 0.8f;
    public float shadowVerticalOffset = 0.2f;
    private GameObject dynamicShadow;
    private bool shadowIsFixed = false;
    private Vector3 fixedShadowPosition;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        itemCollider = GetComponent<Collider2D>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.drag = drag;
            rb.angularDrag = angularDrag;
        }
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        
        if (shadowPrefab != null)
        {
            dynamicShadow = Instantiate(shadowPrefab, transform.position, Quaternion.identity);
            dynamicShadow.transform.localScale = Vector3.one * shadowScale;
        }
    }

    public void Initialize(Vector3 startPosition, float throwDirectionY, bool isDropObject)
    {
        float verticalFactor = Mathf.Abs(throwDirectionY);
        if (isDropObject)
        {
            heightRange = 0.5f;
        }
        else
        {
            heightRange = defaultHeightRange + (verticalThrowExtraRange * verticalFactor);
        }
        minHeight = startPosition.y - heightRange;
        maxHeight = startPosition.y + heightRange;
    }

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

        // Находим коллайдеры Player и AttractTarget
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject attractTargetObj = GameObject.FindGameObjectWithTag("AttractTarget");
        
        if (player != null) playerCollider = player.GetComponent<Collider2D>();
        if (attractTargetObj != null) attractTargetCollider = attractTargetObj.GetComponent<Collider2D>();

        // Игнорируем коллизии с Player и AttractTarget до приземления
        if (itemCollider != null)
        {
            if (playerCollider != null) Physics2D.IgnoreCollision(playerCollider, itemCollider, true);
            if (attractTargetCollider != null) Physics2D.IgnoreCollision(attractTargetCollider, itemCollider, true);
        }

        // Игнорируем другие коллизии как раньше
        string[] tagsToIgnore = { "Sword", "Arrow", "Enemy" };
        foreach (string tag in tagsToIgnore)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject obj in objects)
            {
                Collider2D objCollider = obj.GetComponent<Collider2D>();
                if (objCollider != null && itemCollider != null)
                {
                    Physics2D.IgnoreCollision(objCollider, itemCollider, true);
                }
            }
        }

        startPosition = transform.position;
        pickupTimer = 0f;
        hasLanded = false;
        canBePickedUp = false;
        isAttracting = false;
        AdjustColliderToSprite();
    }

    public void ApplyThrowForce(Vector3 direction, float force)
    {
        if (rb != null && !physicsApplied)
        {
            rb.gravityScale = 1;
            rb.drag = drag;
        
            Vector2 forceDirection = new Vector2(
                direction.x,
                Mathf.Clamp(direction.y, -0.5f, 0.5f)
            ).normalized;
        
            rb.AddForce(forceDirection * force, ForceMode2D.Impulse);
            physicsApplied = true;
            isStopping = false;
        }
    }

    void FixedUpdate()
    {
        if (physicsApplied && !hasLanded)
        {
            float clampedY = Mathf.Clamp(rb.position.y, minHeight, maxHeight);
            if (!Mathf.Approximately(clampedY, rb.position.y))
            {
                isStopping = true;
            }
            
            if ((rb.velocity.magnitude < 0.3f || isStopping) && !hasLanded)
            {
                StopPhysics();
            }
        }
        
        // Логика притягивания к AttractTarget
        if (isAttracting && attractTarget != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, attractTarget.position, attractSpeed * Time.deltaTime);
            
            if (Vector2.Distance(transform.position, attractTarget.position) < 0.1f)
            {
                TryPickup();
            }
        }
    }

    void Update()
    {
        if (dynamicShadow != null)
        {
            if (!shadowIsFixed)
            {
                // Обновляем позицию динамической тени
                Vector3 shadowPos = transform.position;
                shadowPos.y -= shadowVerticalOffset;
                dynamicShadow.transform.position = shadowPos;
            }
            // Фиксированная тень остается на месте (не нужно обновлять)
        }
        
        if (startSwaying)
        {
            float newY = transform.position.y + Mathf.Sin(Time.time * frequency) * amplitude;
            visualObject.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        if (hasLanded)
        {
            pickupTimer += Time.deltaTime;
            if (isFirstDrop && pickupTimer >= 0.5f)
            {
                canBePickedUp = true;
                isFirstDrop = false;
            }
            else if (!isFirstDrop)
            {
                canBePickedUp = true;
            }
        }
    }
    
    private void StopPhysics()
    {
        if (rb == null || hasLanded) return;

        Vector3 finalPosition = new Vector3(
            transform.position.x,
            Mathf.Clamp(transform.position.y, minHeight, maxHeight),
            0
        );

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0;
        rb.drag = 0;
        rb.position = finalPosition;
        
        if (dynamicShadow != null)
        {
            // Фиксируем позицию тени
            shadowIsFixed = true;
            fixedShadowPosition = dynamicShadow.transform.position;
        
            // Регистрируем тень в менеджере
            ShadowManager.Instance.RegisterFixedShadow(fixedShadowPosition, dynamicShadow);
        }

        hasLanded = true;
        startSwaying = true;
        physicsApplied = false;
        isStopping = false;

        transform.position = finalPosition;

        // Включаем коллизии с Player и AttractTarget после приземления
        if (itemCollider != null)
        {
            if (playerCollider != null) Physics2D.IgnoreCollision(playerCollider, itemCollider, false);
            if (attractTargetCollider != null) Physics2D.IgnoreCollision(attractTargetCollider, itemCollider, false);
            
            itemCollider.isTrigger = true;
        }

        // Находим AttractTarget для возможного притягивания
        GameObject attractTargetObj = GameObject.FindGameObjectWithTag("AttractTarget");
        if (attractTargetObj != null)
        {
            attractTarget = attractTargetObj.transform;
        }
 
        canBePickedUp = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hasLanded && canBePickedUp && !isAttracting)
        {
            // Удаляем тень перед притягиванием
            if (dynamicShadow != null)
            {
                ShadowManager.Instance.UnregisterShadow(dynamicShadow);
                Destroy(dynamicShadow);
                dynamicShadow = null;
            }
        
            if (attractTarget != null)
            {
                isAttracting = true;
                startSwaying = false;
            }
        }
        else if (other.CompareTag("AttractTarget") && hasLanded && canBePickedUp)
        {
            TryPickup();
        }
    }

    private void TryPickup()
    {
        // Удаляем тень перед подбором
        if (dynamicShadow != null)
        {
            ShadowManager.Instance.UnregisterShadow(dynamicShadow);
            Destroy(dynamicShadow);
        }

        if (istool)
        {
            count = 1;
        }
        inventory.AddItemToInventory(this);
        Destroy(gameObject);
    }

    private void AdjustColliderToSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Vector2 spriteSize = spriteRenderer.size;
            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                boxCollider.size = spriteSize;
            }
            else
            {
                boxCollider = gameObject.AddComponent<BoxCollider2D>();
                boxCollider.size = spriteSize;
            }
        }
    }
    

    
    void OnDestroy()
    {
        // Уничтожаем только динамические тени
        if (dynamicShadow != null && !shadowIsFixed)
        {
            Destroy(dynamicShadow);
        }
    }
}