using System;
using UnityEngine;

public class PanelMover : MonoBehaviour
{
    public float speed; // Скорость движения панели
    public float targetX; // Целевая позиция X, куда переместится панель (для открытия)
    public float targetX1; // Целевая позиция X, куда переместится панель (для закрытия)
    public bool move = false; // Флаг для начала движения (для открытия)
    public bool move1 = false; // Флаг для начала движения (для закрытия)

    private Vector3 initialPosition; // Начальная позиция панели для сброса

    void Start()
    {
        initialPosition = transform.localPosition; // Сохраняем начальную позицию панели
    }

    void Update()
    {
        // Если движение активировано флагом move
        if (move)
        {
            Vector3 currentPosition = transform.localPosition;
            currentPosition.x = Mathf.MoveTowards(currentPosition.x, targetX, speed * Time.deltaTime);
            transform.localPosition = currentPosition;

            if (Mathf.Approximately(currentPosition.x, targetX))
            {
                move = false; // Останавливаем движение
            }
        }

        // Если движение активировано флагом move1 (для скрытия)
        if (move1)
        {
            Vector3 currentPosition = transform.localPosition;
            currentPosition.x = Mathf.MoveTowards(currentPosition.x, targetX1, speed * Time.deltaTime);
            transform.localPosition = currentPosition;

            if (Mathf.Approximately(currentPosition.x, targetX1))
            {
                move1 = false; // Останавливаем движение
            }
        }
    }
}