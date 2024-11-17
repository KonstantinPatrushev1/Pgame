using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCharacter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    // Массивы спрайтов для каждой из анимаций (Idle и Run)
    public Sprite[] idleUp, idleDown, idleLeft, idleRight;
    public Sprite[] runUp, runDown, runLeft, runRight;

    // Направление движения
    private float directionX;
    private float directionY;

    // Проверка на движение
    private bool isRunning;

    // Переменные для анимации
    private int currentIdleFrame;
    private int currentRunFrame;
    private float animationTimer;
    public float animationSpeed = 0.1f;  // Время между сменой спрайтов (скорость анимации)

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();  // Получаем компонент SpriteRenderer
    }

    void Update()
    {
        // Получаем направления по осям
        directionX = Input.GetAxisRaw("Horizontal");
        directionY = Input.GetAxisRaw("Vertical");

        // Проверка, движется ли персонаж
        isRunning = directionX != 0 || directionY != 0;

        // Обновляем анимацию
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (isRunning)
        {
            // Обновляем анимацию бега мгновенно, без таймеров
            if (directionY > 0)  // Вверх
            {
                spriteRenderer.sprite = runUp[currentRunFrame];
                // Переход к следующему кадру
                currentRunFrame = (currentRunFrame + 1) % runUp.Length;
            }
            else if (directionY < 0)  // Вниз
            {
                spriteRenderer.sprite = runDown[currentRunFrame];
                currentRunFrame = (currentRunFrame + 1) % runDown.Length;
            }
            else if (directionX > 0)  // Вправо
            {
                spriteRenderer.sprite = runRight[currentRunFrame];
                currentRunFrame = (currentRunFrame + 1) % runRight.Length;
            }
            else if (directionX < 0)  // Влево
            {
                spriteRenderer.sprite = runLeft[currentRunFrame];
                currentRunFrame = (currentRunFrame + 1) % runLeft.Length;
            }
        }
        else
        {
            // Обновляем анимацию стояния (idle) мгновенно, без таймеров
            if (directionY > 0)  // Вверх
            {
                spriteRenderer.sprite = idleUp[0]; // Показываем первый кадр стояния вверх
            }
            else if (directionY < 0)  // Вниз
            {
                spriteRenderer.sprite = idleDown[0]; // Показываем первый кадр стояния вниз
            }
            else if (directionX > 0)  // Вправо
            {
                spriteRenderer.sprite = idleRight[0]; // Показываем первый кадр стояния вправо
            }
            else if (directionX < 0)  // Влево
            {
                spriteRenderer.sprite = idleLeft[0]; // Показываем первый кадр стояния влево
            }
            else  // Если персонаж стоит на месте
            {
                spriteRenderer.sprite = idleDown[0]; // Показываем первый кадр стояния вниз (по умолчанию)
            }
        }
    }

}
