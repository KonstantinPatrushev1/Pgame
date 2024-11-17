using UnityEngine;

public class AnimationCharacter : MonoBehaviour
{
    public Sprite[] idleRight; // Анимация стояния направо
    public Sprite[] idleLeft;  // Анимация стояния налево
    public Sprite[] idleUp;    // Анимация стояния вверх
    public Sprite[] idleDown;  // Анимация стояния вниз

    public Sprite[] runRight;  // Анимация бега направо
    public Sprite[] runLeft;   // Анимация бега налево
    public Sprite[] runUp;     // Анимация бега вверх
    public Sprite[] runDown;   // Анимация бега вниз

    public float walkSpeed = 3.0f;  // Скорость ходьбы, зависит от движка игры

    private SpriteRenderer spriteRenderer;
    private Vector2 movement;

    private float animationTimer = 0f;
    private int currentFrame = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Получаем движение игрока (например, с клавиш WASD или стрелки)
        movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        
    }

    void PlayRunningAnimation(Sprite[] runAnim)
    {
        // Игрок бегает, воспроизводим анимацию бега
        animationTimer += Time.deltaTime * walkSpeed;
        if (animationTimer >= 0.1f) // Пауза между сменой спрайтов
        {
            currentFrame++;
            if (currentFrame >= runAnim.Length)
            {
                currentFrame = 0; // Сбросить анимацию при достижении конца
            }

            spriteRenderer.sprite = runAnim[currentFrame];
            animationTimer = 0f;
        }
    }

    void PlayIdleAnimation(Sprite[] idleAnim)
    {
        // Игрок стоит, воспроизводим анимацию стояния
        animationTimer += Time.deltaTime * walkSpeed;
        if (animationTimer >= 0.1f) // Пауза между сменой спрайтов
        {
            currentFrame++;
            if (currentFrame >= idleAnim.Length)
            {
                currentFrame = 0; // Сбросить анимацию при достижении конца
            }

            spriteRenderer.sprite = idleAnim[currentFrame];
            animationTimer = 0f;
        }
    }
}
