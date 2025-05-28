using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float moveDistance = 200f;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float visibleDuration = 2f;

    [Header("UI References")]
    [SerializeField] private TMP_Text itemNameText; // Поле для названия предмета
    [SerializeField] private TMP_Text itemCountText; // Поле для количества
    [SerializeField] private Image itemIconImage; // Поле для иконки

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private static PanelAnimator instance;
    private static DataBase database;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        database = FindObjectOfType<DataBase>();
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public static void ShowItemPanel(int itemId, int count)
    {
        if (instance != null)
        {
            instance.SetItemInfo(itemId, count);
            instance.StartAnimation();
        }
    }

    private void SetItemInfo(int itemId, int count)
    {
        Item item = database.items.Find(i => i.id == itemId);
        
        if (item != null)
        {
            // Устанавливаем название в отдельное поле
            if (itemNameText != null)
                itemNameText.text = item.name;
            
            // Устанавливаем количество в отдельное поле
            if (itemCountText != null)
                itemCountText.text = $"X {count}";
            
            // Устанавливаем иконку
            if (itemIconImage != null)
            {
                itemIconImage.sprite = item.img;
                itemIconImage.preserveAspect = true;
            }
        }
        else
        {
            if (itemNameText != null)
                itemNameText.text = "Unknown Item";
            
            if (itemCountText != null)
                itemCountText.text = $"X {count}";
        }
    }

    private void StartAnimation()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = originalPosition;
        
        StopAllCoroutines();
        StartCoroutine(AnimatePanel());
    }

    private IEnumerator AnimatePanel()
    {
        // Движение влево
        Vector2 targetPosition = originalPosition + Vector2.left * moveDistance;
        float elapsedTime = 0f;
        
        while (elapsedTime < moveDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        rectTransform.anchoredPosition = targetPosition;
        
        // Ожидание
        yield return new WaitForSeconds(visibleDuration);
        
        // Плавное исчезновение
        elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;
        
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = originalPosition;
        gameObject.SetActive(false);
    }
}