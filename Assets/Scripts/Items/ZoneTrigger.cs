using Unity.VisualScripting;
using UnityEngine;
public class ZoneTrigger : MonoBehaviour
{
    private bool isPlayerInZone = false;
    public ChestZones chest;
    public bool chestIsOpen;
    private bool interactionCooldown = false;
    public MapController mapController;
    public CursorManager cursormanager;
    private Inventory inventory;


    void Start()
    {
        GameObject inventoryObject = GameObject.Find("Main Camera");
        inventory = inventoryObject.GetComponent<Inventory>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true; // Игрок входит в зону
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false; // Игрок выходит из зоны
        }
    }

    void Update()
    {
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.E) && chest.IsPlayerLookingAtChest() && !interactionCooldown && (mapController == null || !mapController.ismapopen) && !inventory.IsInventoryOpen && !inventory.PausePanel.activeSelf)
        {
            if (!chestIsOpen)
            {
                OpenChest();
            }
            else
            {
                CloseChest();
            }
        }

        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) && chestIsOpen && !interactionCooldown)
        {
            CloseChest();
        }

    }

    private void OpenChest()
    {
        chest.chestPanel.SetActive(true);
        chest.inventory.OpenInventory();
        chest.inventory.DisplayChestContents(chest); // Отобразить содержимое сундука
        chestIsOpen = true;
        ChestZones.IsAnyChestOpen = true;
        StartInteractionCooldown();
    }

    private void CloseChest()
    {
        chest.inventory.SaveChestContents(chest); // Сохранить содержимое сундука
        chest.chestPanel.SetActive(false);
        chestIsOpen = false;
        ChestZones.IsAnyChestOpen = false;
        cursormanager.HideCursor();
        StartInteractionCooldown();
        inventory.CloseInventory();
    }

    private void StartInteractionCooldown()
    {
        interactionCooldown = true;
        Invoke(nameof(ResetInteractionCooldown), 0f); // Задержка 0.5 секунд
    }

    private void ResetInteractionCooldown()
    {
        interactionCooldown = false;
    }
}