using UnityEngine;

public class CharacterLayerControl : MonoBehaviour
{
    private SpriteRenderer clothesRenderer;  // Рендерер одежды
    private SpriteRenderer characterRenderer; // Рендерер персонажа

    public GameObject character; // Ссылка на персонажа

    void Start()
    {
        // Получаем рендерер одежды
        clothesRenderer = GetComponent<SpriteRenderer>();

        // Получаем рендерер персонажа
        if (character != null)
        {
            characterRenderer = character.GetComponent<SpriteRenderer>();
        }

        // Если персонаж не задан вручную, пробуем найти его
        if (characterRenderer == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                characterRenderer = player.GetComponent<SpriteRenderer>();
            }
        }

        // Если рендерер персонажа найден, обновляем слой одежды
        if (characterRenderer != null)
        {
            UpdateLayer();
        }
    }

    void Update()
    {
        // Если рендерер персонажа найден, обновляем слой одежды
        if (characterRenderer != null)
        {
            UpdateLayer();
        }
    }

    void UpdateLayer()
    {
        // Получаем текущий sortingOrder персонажа
        int characterSortingOrder = characterRenderer.sortingOrder;

        // Устанавливаем слой одежды на +1 относительно sortingOrder персонажа
        if (clothesRenderer != null)
        {
            clothesRenderer.sortingOrder = characterSortingOrder + 1;
        }
    }
}