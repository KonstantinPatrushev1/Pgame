using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Save;

public class StatueZones : MonoBehaviour
{
    public Transform player; // Ссылка на объект игрока
    public Animator playerAnimator; // Ссылка на Animator игрока
    public GameObject savePanel; // Панель для сохранения
    public float zoneSize = 1f; // Размер зоны вокруг статуи
    public Vector2 zoneDimensions = new Vector2(2f, 2f); // Размер зоны (ширина, высота)
    public int statueId; // Уникальный ID статуи
    public movement movement;

    public bool isPlayerInZone = false; // Состояние нахождения игрока в зоне статуи
    private bool interactionCooldown = false; // Ожидание для предотвращения спама
    public CursorManager cursorManager;
    public MapController mapController;
    public Inventory inventory;

    public GameObject fadeImageObject; // Объект с изображением для затемнения
    private Image fadeImage; // Компонент Image на объекте для затемнения
    public float fadeDuration = 1f; // Время на затемнение и восстановление
    public float teleportDelay = 1f; // Задержка перед телепортацией после того, как экран стал черным

    void Start()
    {
        // Получаем компонент Image с объекта fadeImageObject
        fadeImage = fadeImageObject.GetComponent<Image>();

        // Убедитесь, что объект с изображением изначально не активен
        fadeImageObject.SetActive(false);

        // Позиции зон относительно статуи
        Vector2[] zoneOffsets = new Vector2[] {
            new Vector2(-zoneSize, 0), // Лево
            new Vector2(zoneSize, 0),  // Право
            new Vector2(0, zoneSize),  // Вверх
            new Vector2(0, -zoneSize)  // Вниз
        };

        // Создаем зоны вокруг статуи
        foreach (Vector2 offset in zoneOffsets)
        {
            GameObject zone = new GameObject("Zone");
            zone.tag = "ChestZone";
            zone.transform.parent = transform; // Делаем объект дочерним статуе
            zone.transform.localPosition = offset; // Устанавливаем позицию относительно центра статуи

            // Добавляем BoxCollider2D
            BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
            collider.isTrigger = true; // Устанавливаем на триггер
            collider.size = zoneDimensions; // Устанавливаем размер зоны

            // Добавляем компонент для обработки OnTriggerEnter2D
            ZoneTriggerStatue zoneTriggerStatue = zone.AddComponent<ZoneTriggerStatue>();
            zoneTriggerStatue.statue = this; // Передаем ссылку на статую
        }
    }
    private bool ispanelopen;
    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E) && IsPlayerLookingAtStatue() && !interactionCooldown && (mapController == null || !mapController.ismapopen) && !inventory.IsInventoryOpen && !inventory.PausePanel.activeSelf)
        {
            OpenSavePanel(); // Открыть панель сохранения
            cursorManager.ShowCursor();
            ispanelopen = true;
        }

        if (isPlayerInZone && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) && savePanel.activeSelf && !interactionCooldown)
        {
            CloseSavePanel(); // Закрыть панель сохранения
            cursorManager.HideCursor();
            ispanelopen = false;
        }
    }

    // Проверка, смотрит ли игрок в сторону статуи
    public bool IsPlayerLookingAtStatue()
    {
        float horizontal = playerAnimator.GetFloat("Horizontal");
        float vertical = playerAnimator.GetFloat("Vertical");

        // Направление от игрока к статуе
        Vector2 directionToStatue = (transform.position - player.position).normalized;

        // Проверка, смотрит ли игрок в сторону статуи
        if (horizontal > 0 && directionToStatue.x > 0) return true;
        if (horizontal < 0 && directionToStatue.x < 0) return true;
        if (vertical > 0 && directionToStatue.y > 0) return true;
        if (vertical < 0 && directionToStatue.y < 0) return true;

        return false;
    }

    // Обработчик, когда игрок входит в зону
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true; // Игрок входит в зону
        }
    }

    // Обработчик, когда игрок выходит из зоны
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false; // Игрок выходит из зоны
        }
    }

    // Открытие панели сохранения
    private void OpenSavePanel()
    {
        savePanel.SetActive(true); // Показываем панель
        if (movement != null)
        {
            movement.movementInput = Vector2.zero; // Останавливаем движение
            movement.UpdateAnimation(); // Обновляем анимацию
        }
        StartInteractionCooldown();
        cursorManager.ShowCursor();
        ispanelopen = true;
    }

    // Закрытие панели сохранения
    private void CloseSavePanel()
    {
        savePanel.SetActive(false); // Скрываем панель
        StartInteractionCooldown();
    }

    // Запуск кулиса для предотвращения спама
    private void StartInteractionCooldown()
    {
        interactionCooldown = true;
        Invoke(nameof(ResetInteractionCooldown), 0.5f); // Задержка 0.5 секунд
    }

    // Сброс кулиса
    private void ResetInteractionCooldown()
    {
        interactionCooldown = false;
    }

    // Метод для плавного затемнения и перемещения игрока
    public void TeleportPlayerToStatue(int targetStatueId)
    {
        if (statueId == targetStatueId)
        {
            // Начинаем затемнение экрана
            StartCoroutine(FadeOutAndTeleport());
        }
    }

    private IEnumerator FadeOutAndTeleport()
    {
        // Активируем объект для затемнения
        fadeImageObject.SetActive(true);

        // Плавное затемнение экрана
        float timeElapsed = 0f;
        Color initialColor = fadeImage.color;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            fadeImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(0, 1, timeElapsed / fadeDuration));
            yield return null;
        }
        

        // Теперь перемещаем игрока
        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y - zoneSize, transform.position.z);
        player.position = newPosition;
        
        yield return new WaitForSeconds(teleportDelay);

        // Плавное восстановление яркости экрана
        timeElapsed = 0f;
        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            fadeImage.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(1, 0, timeElapsed / fadeDuration));
            yield return null;
        }

        // После восстановления яркости деактивируем объект с изображением
        fadeImageObject.SetActive(false);
    }
}
