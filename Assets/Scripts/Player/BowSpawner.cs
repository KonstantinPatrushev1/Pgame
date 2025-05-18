using UnityEngine;

public class BowSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform bowParent;
    [SerializeField] private string bowPrefabPath = "Weapons/BowPrefab";
    [SerializeField] private string arrowPrefabPath = "Weapons/ArrowPrefab";
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private Animator eyesAnimator;
    [SerializeField] private Animator hatAnimator;
    [SerializeField] private Animator shirtAnimator;
    [SerializeField] private Animator pantsAnimator;
    [SerializeField] private GameObject player;
    [SerializeField] private Inventory inventory;
    [SerializeField] private MapController mapController;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject StatuePanel;
    [SerializeField] private GameObject SavePanel;
    [SerializeField] private GameObject TeleportPanel;

    [Header("Bow Positioning")]
    [SerializeField] private Vector3 rightPosition = new Vector3(0.3f, 0.1f, 0);
    [SerializeField] private Vector3 leftPosition = new Vector3(-0.3f, 0.1f, 0);
    [SerializeField] private Vector3 upPosition = new Vector3(0, 0.3f, 0);
    [SerializeField] private Vector3 downPosition = new Vector3(0, -0.1f, 0);
    
    [Header("Bow Rotation")]
    [SerializeField] private float rightRotation = 0f;
    [SerializeField] private float leftRotation = 180f;
    [SerializeField] private float upRotation = 90f;
    [SerializeField] private float downRotation = -90f;
    
    [Header("Bow Scale")]
    [SerializeField] private Vector3 rightScale = new Vector3(0.5f, 0.5f, 1);
    [SerializeField] private Vector3 leftScale = new Vector3(0.5f, 0.5f, 1);
    [SerializeField] private Vector3 upScale = new Vector3(0.5f, 0.5f, 1);
    [SerializeField] private Vector3 downScale = new Vector3(0.5f, 0.5f, 1);

    [Header("Attack Settings")]
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private float arrowSpeed = 10f;

    private GameObject currentBow;
    private GameObject arrowPrefab;
    private SpriteRenderer bowRenderer;
    private SpriteRenderer playerRenderer;
    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float cooldownTimer = 0f;
    

    private void Start()
    {
        playerRenderer = player.GetComponent<SpriteRenderer>();
        arrowPrefab = Resources.Load<GameObject>(arrowPrefabPath);
        if (currentBow == null)
        {
            SpawnBow();
        }
    }

    private void Update()
    {
        if (IsGamePaused()) return;

        cooldownTimer += Time.deltaTime;

        // Логика атаки
        if (Input.GetMouseButtonDown(0))
        {
            TryStartAttack();
        }

        // Управление анимацией атаки
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                EndAttack();
            }
        }
    }

    private bool IsGamePaused()
    {
        return inventory.IsInventoryOpen || mapController.IsMapOpen || 
               PausePanel.activeSelf || StatuePanel.activeSelf || 
               SavePanel.activeSelf || TeleportPanel.activeSelf;
    }

    private void TryStartAttack()
    {
        // Проверяем условия для атаки
        if (cooldownTimer < attackCooldown ||
            WeaponManager.instance.weaponId != 6 ||
            isAttacking)
        {
            characterAnimator.SetBool("Bow", false);
            eyesAnimator.SetBool("Bow", false); 
            hatAnimator.SetBool("Bow", false);
            shirtAnimator.SetBool("Bow", false);
            pantsAnimator.SetBool("Bow", false);
            return;
        }
        
        characterAnimator.SetBool("Bow", true);
        eyesAnimator.SetBool("Bow", true);
        hatAnimator.SetBool("Bow", true);
        shirtAnimator.SetBool("Bow", true);
        pantsAnimator.SetBool("Bow", true);
        StartAttack();
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = 0f;
        cooldownTimer = 0f;

        if (currentBow == null)
        {
            SpawnBow();
        }
        else
        {
            currentBow.SetActive(true);
        }

        characterAnimator.SetBool("BowShoot", true);
        eyesAnimator.SetBool("BowShoot", true);
        hatAnimator.SetBool("BowShoot", true);
        shirtAnimator.SetBool("BowShoot", true);
        pantsAnimator.SetBool("BowShoot", true);
        UpdateBowPosition();
        
        // Создаем стрелу
        ShootArrow();
    }

    private void ShootArrow()
    {
        if (arrowPrefab == null) return;

        float horizontal = characterAnimator.GetFloat("Horizontal");
        float vertical = characterAnimator.GetFloat("Vertical");

        Vector2 shootDirection = Vector2.zero;
        Vector3 spawnOffset = Vector3.zero;

        // Приоритет горизонтальному направлению, если нажато вбок (даже немного)
        if (Mathf.Abs(horizontal) > 0.1f) // Если есть хоть какое-то горизонтальное нажатие
        {
            shootDirection = horizontal > 0 ? Vector2.right : Vector2.left;
            spawnOffset = horizontal > 0 ? new Vector3(0.5f, 0.1f, 0) : new Vector3(-0.5f, 0.1f, 0);
        }
        else // Если горизонтальное нажатие отсутствует, стреляем вертикально
        {
            shootDirection = vertical > 0 ? Vector2.up : Vector2.down;
            spawnOffset = vertical > 0 ? new Vector3(0, 0.5f, 0) : new Vector3(0, -0.5f, 0);
        }

        // Позиция спавна с учетом смещения
        Vector3 spawnPosition = player.transform.position + spawnOffset;

        GameObject arrow = Instantiate(arrowPrefab, spawnPosition, Quaternion.identity);
        Arrow arrowComponent = arrow.GetComponent<Arrow>();
        if (arrowComponent != null)
        {
            arrowComponent.Initialize(shootDirection, player, currentBow);
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        if (currentBow != null)
        {
            currentBow.SetActive(false);
            characterAnimator.SetBool("BowShoot", false);
            eyesAnimator.SetBool("BowShoot", false);
            hatAnimator.SetBool("BowShoot", false);
            shirtAnimator.SetBool("BowShoot", false);
            pantsAnimator.SetBool("BowShoot", false);
        }
    }

    private void SpawnBow()
    {
        GameObject bowPrefab = Resources.Load<GameObject>(bowPrefabPath);
        if (bowPrefab != null)
        {
            currentBow = Instantiate(bowPrefab, bowParent);
            bowRenderer = currentBow.GetComponent<SpriteRenderer>();
            currentBow.SetActive(false); // Скрываем до атаки
        }
        else
        {
            Debug.LogError("Bow prefab not found at path: " + bowPrefabPath);
        }
    }

    private void UpdateBowPosition()
    {
        if (currentBow == null) return;

        float horizontal = characterAnimator.GetFloat("Horizontal");
        float vertical = characterAnimator.GetFloat("Vertical");

        bool isFacingRight = horizontal > 0.5f;
        bool isFacingLeft = horizontal < -0.5f;
        bool isFacingUp = vertical > 0.5f;
        bool isFacingDown = vertical < -0.5f || (Mathf.Abs(horizontal) <= 0.5f && Mathf.Abs(vertical) <= 0.5f);

        if (isFacingRight)
        {
            SetBowTransform(rightPosition, rightRotation, rightScale);
            SetBowSortingOrder(false);
        }
        else if (isFacingLeft)
        {
            SetBowTransform(leftPosition, leftRotation, leftScale);
            SetBowSortingOrder(false);
        }
        else if (isFacingUp)
        {
            SetBowTransform(upPosition, upRotation, upScale);
            SetBowSortingOrder(false);
        }
        else if (isFacingDown)
        {
            SetBowTransform(downPosition, downRotation, downScale);
            SetBowSortingOrder(true);
        }
    }

    private void SetBowTransform(Vector3 position, float rotationZ, Vector3 scale)
    {
        currentBow.transform.localPosition = position;
        currentBow.transform.localEulerAngles = new Vector3(0, 0, rotationZ);
        currentBow.transform.localScale = scale;
    }

    private void SetBowSortingOrder(bool isFacingDown)
    {
        if (bowRenderer != null && playerRenderer != null)
        {
            bowRenderer.sortingOrder = isFacingDown ? 
                playerRenderer.sortingOrder + 1 : 
                playerRenderer.sortingOrder - 1;
        }
    }

    private void OnDestroy()
    {
        if (currentBow != null)
        {
            Destroy(currentBow);
        }
    }
}