using UnityEngine;
using UnityEngine.UI;

public class StatuePanel : MonoBehaviour
{
    public Button[] statueButtons; // Кнопки для каждой статуи
    public StatueZones[] statues; // Массив статуй

    void Start()
    {
        // Привязываем обработчики для каждой кнопки
        for (int i = 0; i < statueButtons.Length; i++)
        {
            int statueId = statues[i].statueId;
            statueButtons[i].onClick.AddListener(() => TeleportToStatue(statueId));
        }
    }

    void TeleportToStatue(int statueId)
    {
        // Найдем нужную статую по ID и телепортируем игрока к ней
        foreach (var statue in statues)
        {
            if (statue.statueId == statueId)
            {
                statue.TeleportPlayerToStatue(statueId);
                gameObject.SetActive(false);
                break; // Прерываем цикл, чтобы телепортироваться только к одной статуе
            }
        }
    }
}