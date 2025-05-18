using UnityEngine;

public class CursorManager : MonoBehaviour
{
    void Start()
    {
        // Скрыть курсор и заблокировать его в центре экрана
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Метод для отображения курсора, например, при открытии инвентаря
    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; // Разблокировать курсор
    }

    // Метод для скрытия курсора, например, при закрытии инвентаря
    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined; // Заблокировать курсор в центре экрана
    }
}