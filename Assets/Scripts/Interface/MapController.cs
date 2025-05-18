using UnityEngine;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    public GameObject mapPanel; // Панель карты
    public RectTransform marker; // Индикатор персонажа
    public Transform player; // Трансформ персонажа
    public Vector2 mapSize; // Размер карты в мире
    public Vector2 mapUISize; // Размер карты в UI
    public bool IsMapOpen => mapPanel.activeSelf;  // Проверка, открыта ли карта
    public Inventory inventory;
    public bool ismapopen;
    public GameObject PausePanel;
    public GameObject StatuePanel;
    public GameObject SavePanel;
    public GameObject TeleportPanel;

    void Update()
    {
        // Открытие/закрытие карты на M
        if (Input.GetKeyDown(KeyCode.M) && !inventory.IsInventoryOpen && 
            !PausePanel.activeSelf && 
            !StatuePanel.activeSelf && 
            !SavePanel.activeSelf && 
            !TeleportPanel.activeSelf)
        {
            mapPanel.SetActive(!mapPanel.activeSelf);
            ismapopen = mapPanel.activeSelf; // Обновляем флаг
        }

        // Закрытие карты на Esc только если она открыта
        if (Input.GetKeyDown(KeyCode.Escape) && mapPanel.activeSelf)
        {
            mapPanel.SetActive(false);
            ismapopen = false; // Сбрасываем флаг
        }

        // Обновление позиции маркера
        if (mapPanel.activeSelf)
        {
            Vector2 playerPos = new Vector2(player.position.x, player.position.y);
            Vector2 normalizedPos = new Vector2(playerPos.x / mapSize.x, playerPos.y / mapSize.y);
            Vector2 markerPos = new Vector2(normalizedPos.x * mapUISize.x, normalizedPos.y * mapUISize.y);

            marker.anchoredPosition = markerPos;
        }
    }
}