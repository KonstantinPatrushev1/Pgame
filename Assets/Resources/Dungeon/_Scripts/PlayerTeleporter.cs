using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PlayerTeleporter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private RoomFirstDungeonGenerator dungeonGenerator;
    [SerializeField] public GameObject fadeImageObject;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float blackScreenDuration = 0.5f;
    [SerializeField] private KeyCode teleportKey = KeyCode.L;
    [SerializeField] private bool regenerateDungeon = true;
    [SerializeField] private Camera camera;

    private Image fadeOverlay;

    void Start()
    {
        fadeOverlay = fadeImageObject.GetComponent<Image>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(teleportKey))
        {
            StartCoroutine(FullTeleportSequence());
        }
    }
    
    private void Awake()
    {
        if (fadeOverlay == null)
        {
            fadeOverlay = GameObject.Find("FadeOverlay")?.GetComponent<Image>();
            if (fadeOverlay != null)
            {
                fadeOverlay.gameObject.SetActive(true);
                fadeOverlay.enabled = true;
            }
        }
    }

    private IEnumerator FullTeleportSequence()
    {
        if (player == null || dungeonGenerator == null) yield break;
        
        // 1. Генерация данжа (если включено)
        if (regenerateDungeon)
        {
            // Очищаем старый данж
            dungeonGenerator.ClearDungeon();
            
            // Запускаем генерацию нового
            dungeonGenerator.GenerateDungeon();
            yield return new WaitUntil(() => dungeonGenerator.IsGenerationComplete);
        }
        fadeImageObject.SetActive(true);

        // 2. Затемнение экрана
        yield return StartCoroutine(FadeScreen(0f, 1f));
        
        // 3. Телепортация
        TeleportPlayerToRandomRoom();
        
        if (ColorUtility.TryParseHtmlString("#25131A", out Color color))
        {
            camera.backgroundColor = color;
        }
        else
        {
            Debug.LogError("Невозможно распознать цвет!");
        }
        
        // 4. Пауза с черным экраном
        yield return new WaitForSeconds(blackScreenDuration);
        
        // 5. Осветление
        yield return StartCoroutine(FadeScreen(1f, 0f));
        fadeImageObject.SetActive(false);
    }

    private void TeleportPlayerToRandomRoom()
    {
        if (dungeonGenerator == null) return;

        // Получаем все коридорные тайлы внутри комнат
        HashSet<Vector2Int> corridorTilesInRooms = dungeonGenerator.GetCorridorTilesInRooms();
    
        if (corridorTilesInRooms.Count == 0)
        {
            Debug.LogWarning("No corridor tiles in rooms found! Using random room center instead.");
            // Fallback - используем центр комнаты, если нет коридорных тайлов
            Vector2Int? randomCenter = dungeonGenerator.GetRandomRoomCenter();
            if (randomCenter.HasValue && player != null)
            {
                player.position = new Vector3(randomCenter.Value.x, randomCenter.Value.y, player.position.z);
            }
            return;
        }

        // Выбираем случайный коридорный тайл из найденных
        Vector2Int[] corridorArray = new Vector2Int[corridorTilesInRooms.Count];
        corridorTilesInRooms.CopyTo(corridorArray);
        Vector2Int randomCorridorPosition = corridorArray[Random.Range(0, corridorArray.Length)];

        // Телепортируем игрока
        if (player != null)
        {
            player.position = new Vector3(randomCorridorPosition.x, randomCorridorPosition.y, player.position.z);
        }
    }

    private IEnumerator FadeScreen(float startAlpha, float targetAlpha)
    {
        if (fadeOverlay == null) yield break;
        
        float elapsed = 0f;
        Color color = fadeOverlay.color;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            fadeOverlay.color = color;
            yield return null;
        }
        
        color.a = targetAlpha;
        fadeOverlay.color = color;
    }
}