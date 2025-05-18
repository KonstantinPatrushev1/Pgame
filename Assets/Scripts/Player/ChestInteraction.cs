using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    public float interactionDistance = 1.5f; // Радиус взаимодействия
    public Animator animator; // Ссылка на Animator персонажа
    private Transform chest; // Ссылка на сундук

    private void Start()
    {
        chest = GameObject.FindGameObjectWithTag("Chest").transform;
    }

    private void Update()
    {
        // Проверяем условия для взаимодействия: расстояние и направление
        if (chest != null && IsFacingChest() && IsInInteractionRange())
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Взаимодействие с сундуком!");
            }
        }
    }

    // Проверка на расстояние между персонажем и сундуком
    private bool IsInInteractionRange()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        Collider2D chestCollider = chest.GetComponent<Collider2D>();

        if (playerCollider != null && chestCollider != null)
        {
            return Vector2.Distance(playerCollider.bounds.center, chestCollider.bounds.center) <= interactionDistance;
        }
        return false;
    }

    // Проверка, смотрит ли персонаж в сторону сундука
    private bool IsFacingChest()
    {
        // Получаем параметры направления из Animator
        float horizontal = animator.GetFloat("Horizontal");
        float vertical = animator.GetFloat("Vertical");

        // Вектор направления взгляда персонажа
        Vector2 lookDirection = new Vector2(horizontal, vertical).normalized;

        // Вектор от персонажа к сундуку
        Vector2 toChest = (chest.position - transform.position).normalized;

        // Проверка угла между направлением взгляда и сундуком
        float angle = Vector2.Angle(lookDirection, toChest);

        return angle < 45f; // Считаем, что персонаж "смотрит" на сундук, если угол меньше 45 градусов
    }
}