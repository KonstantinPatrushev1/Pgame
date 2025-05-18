using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class ChestEIndicator : MonoBehaviour
{
    public Transform chestTransform; // Ссылка на сундук

    private Coroutine scaleCoroutine;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private float scaleDuration = 0.2f;
    public float scale;
    private void Start()
    {
        if (chestTransform != null)
        {
            originalScale = chestTransform.localScale;
            targetScale = originalScale * scale; // Увеличение на 20% относительно оригинального размера
        }
        else
        {
            Debug.LogError("ChestEIndicator: chestTransform не назначен!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartScaleAnimation(targetScale);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartScaleAnimation(originalScale);
        }
    }

    private void StartScaleAnimation(Vector3 toScale)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleChest(toScale));
    }

    private IEnumerator ScaleChest(Vector3 toScale)
    {
        float time = 0f;
        Vector3 fromScale = chestTransform.localScale;

        while (time < scaleDuration)
        {
            time += Time.deltaTime;
            float t = time / scaleDuration;
            chestTransform.localScale = Vector3.Lerp(fromScale, toScale, t);
            yield return null;
        }

        chestTransform.localScale = toScale;
    }
}