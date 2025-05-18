using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameToMenu : MonoBehaviour
{
    public GameObject panel;
    public GameObject panelMap;
    public GameObject panelInventory;
    public bool ispanelopen;

    public CursorManager cursorManager;
    
    public GameObject StatuePanel;
    public GameObject SavePanel;
    public GameObject TeleportPanel;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Если панель уже открыта, закрываем её
            if (ispanelopen)
            {
                panel.SetActive(false);
                cursorManager.HideCursor();
                ispanelopen = false;
            }
            // Если никакая другая панель не активна, открываем панель
            else if (!panelInventory.activeSelf 
                     && !panelMap.activeSelf
                     && !StatuePanel.activeSelf 
                     && !SavePanel.activeSelf 
                     && !TeleportPanel.activeSelf)
            {
                panel.SetActive(true);
                cursorManager.ShowCursor();
                ispanelopen = true;
            }
        }
    }

    public void ToMenu(string sceneName)
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneName);
    }
}
