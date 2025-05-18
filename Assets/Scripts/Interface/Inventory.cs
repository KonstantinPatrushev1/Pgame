using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image; // Для работы с TextMeshPro

using Random = UnityEngine.Random;
using Save;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public GameObject selectionFrame;  // Ссылка на рамку
    public RectTransform selectionFrameTransform;  // Ссылка на RectTransform рамки
    
    public GameObject hotbarPanel; // Ссылка на горячую панель (панель для первых нескольких ячеек)
    public int hotbarSize = 10; // Количество ячеек в горячей панели

    public CursorManager cursorManager;

    public GameObject droppedItemPrefab;
    private bool isMovingInventory = false;
    public DataBase data;
    public List<ItemInventory> items = new List<ItemInventory>();

    public GameObject gameObjShow;

    public GameObject InventoryMainObject;
    public int maxCount;

    public Camera cam;
    public EventSystem es;

    public int currentID;
    public ItemInventory currentItem;

    public RectTransform movingObject;
    public Vector3 offset;

    public GameObject background;

    public bool IsInventoryOpen = false;

    public PanelMover panelmover;

    public MapController mapController;
    
    public int lastSelectedHotbarSlot = 0;

    public GameObject chestPanel;
    public int inventoryCopySize = 20;
    
    private GameData gameData;
    private Storage storage;
    
    public GameObject PausePanel;
    
    public GameObject StatuePanel;
    public GameObject SavePanel;
    public GameObject TeleportPanel;
    

    public void Start()
    { 
        gameData = new GameData();
        storage = new Storage();
        selectionFrameTransform = selectionFrame.GetComponent<RectTransform>();
        selectionFrame.SetActive(false);
        if (items.Count == 0)
        {
            AddGraphics();
        }

        for (int i = 0; i < maxCount - inventoryCopySize; i++) // тест
        {
            Item randomItem = data.items[Random.Range(0, data.items.Count)];
            int itemCount = randomItem.istool ? 1 : Random.Range(1, 99); // Если инструмент, то всегда 1
        
            AddItem(i, randomItem, itemCount);
        }
        
        UpdateInventory();
    }

    public void Update()
    {
        if (isMovingInventory)
        {
            return;
        }
        HandleHotbarSelection();
        if (currentID != -1)
        {
            MoveObject();
        }

        // Открытие инвентаря только на I
        if (Input.GetKeyDown(KeyCode.I) && !mapController.ismapopen && !PausePanel.activeSelf
            && !StatuePanel.activeSelf 
            && !SavePanel.activeSelf 
            && !TeleportPanel.activeSelf)
        {
            if (ChestZones.IsAnyChestOpen) // Проверяем глобальный флаг
            {
                return; // Если сундук открыт, не открываем инвентарь
            }

            if (!IsInventoryOpen) // Если инвентарь не открыт, открываем его
            {
                OpenInventory();
            }
            else // Если инвентарь уже открыт, скрываем его
            {
                CloseInventory();
            }
        }

        // Закрытие инвентаря на Esc
        if (Input.GetKeyDown(KeyCode.Escape) && IsInventoryOpen)
        {
            CloseInventory();
        }

        if (Input.GetMouseButtonUp(0) && currentID != -1)
        {
            // Проверяем, что указатель мыши находится за пределами всех активных панелей
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                    InventoryMainObject.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    cam) &&
                !RectTransformUtility.RectangleContainsScreenPoint(
                    hotbarPanel.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    cam) &&
                // Проверяем, что chestPanel либо скрыта, либо не содержит мышь
                (!chestPanel.activeInHierarchy || !RectTransformUtility.RectangleContainsScreenPoint(
                    chestPanel.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    cam)))
            {
                DropItem(); // Выбрасываем предмет
            }
        }

        SelectHotbarItem(lastSelectedHotbarSlot);
    }
    
    public void DisplayChestContents(ChestZones chestZone)
    {
        var chestItems = chestZone.chestStorage[chestZone.chestId];
        for (int i = maxCount - inventoryCopySize; i < maxCount; i++)
        {
            int chestSlotIndex = i - (maxCount - inventoryCopySize);

            items[i].id = chestItems[chestSlotIndex].id;
            items[i].count = chestItems[chestSlotIndex].count;
            items[i].istool = chestItems[chestSlotIndex].istool;

            if (chestItems[chestSlotIndex].itemGameObj != null)
            {
                items[i].itemGameObj.GetComponent<Image>().sprite = chestItems[chestSlotIndex].itemGameObj.GetComponent<Image>().sprite;
            }
        }
        UpdateInventory();
    }

    public void SaveChestContents(ChestZones chestZone)
    {
        var chestItems = chestZone.chestStorage[chestZone.chestId];
        for (int i = maxCount - inventoryCopySize; i < maxCount; i++)
        {
            int chestSlotIndex = i - (maxCount - inventoryCopySize);
            chestItems[chestSlotIndex].id = items[i].id;
            chestItems[chestSlotIndex].count = items[i].count;
            chestItems[chestSlotIndex].istool = items[i].istool;
            chestItems[chestSlotIndex].itemGameObj = items[i].itemGameObj;
        }
    }

    public void OpenInventory()
    {
        IsInventoryOpen = true;
        background.SetActive(true);
        panelmover.move = true; // Перемещаем инвентарь
        panelmover.move1 = false; // Останавливаем движение для закрытия инвентаря
        cursorManager.ShowCursor();
        UpdateInventory();
    }

    public void CloseInventory()
    {
        panelmover.move1 = true; // Даем команду двигать панель для скрытия
        panelmover.move = false; // Останавливаем движение для открытия
        cursorManager.HideCursor();
        StartCoroutine(CloseInventoryWithDelay(0.5f)); // Задержка 0.5 секунды
    }
    public void AddItemToInventory(DroppedItem droppedItem)
{
    int slot = FindItemSlot(droppedItem.itemID);

    if (droppedItem.istool) // Если это инструмент, ищем первую пустую ячейку
    {
        slot = FindEmptySlot();
        if (slot != -1)
        {
            AddItem(slot, data.items[droppedItem.itemID], droppedItem.count);
            UpdateInventory();
        }
        return; // Завершаем обработку
    }

    if (slot != -1) // Если уже есть предметы этого типа
    {
        while (droppedItem.count > 0)
        {
            int remainingSpace = MaxStackSize - items[slot].count;

            if (remainingSpace > 0)
            {
                if (droppedItem.count <= remainingSpace)
                {
                    items[slot].count += droppedItem.count;
                    droppedItem.count = 0;
                }
                else
                {
                    items[slot].count = MaxStackSize;
                    droppedItem.count -= remainingSpace;
                }
            }

            if (droppedItem.count > 0)
            {
                slot = FindNextSlotWithSameItem(droppedItem.itemID);
                if (slot == -1) break;
            }
        }

        while (droppedItem.count > 0) // Если остались предметы
        {
            int emptySlot = FindEmptySlot();
            if (emptySlot != -1)
            {
                int countToAdd = Mathf.Min(droppedItem.count, MaxStackSize);
                AddItem(emptySlot, data.items[droppedItem.itemID], countToAdd);
                droppedItem.count -= countToAdd;
            }
            else
            {
                Debug.LogWarning("Нет места в инвентаре!");
                break;
            }
        }

        UpdateInventory();
    }
    else // Если нет предметов этого типа
    {
        int emptySlot = FindEmptySlot();
        if (emptySlot != -1)
        {
            AddItem(emptySlot, data.items[droppedItem.itemID], droppedItem.count);
            UpdateInventory();
        }
        else
        {
            Debug.LogWarning("Нет места в инвентаре!");
        }
    }
}

// Метод для поиска следующей ячейки с тем же предметом
    private int FindNextSlotWithSameItem(int itemID)
    {
        for (int i = 0; i < items.Count - inventoryCopySize; i++) // Проверяем только основные слоты и хот-бар
        {
            if (items[i].id == itemID && items[i].count < MaxStackSize)
            {
                return i;
            }
        }
        return -1; // Нет подходящих ячеек
    }
    public int FindItemSlot(int itemID)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].id == itemID)
            {
                return i;
            }
        }
        return -1; // Нет такого предмета в инвентаре
    }
    public int FindEmptySlot()
    {
        for (int i = 0; i < items.Count - inventoryCopySize; i++) // Проверяем только основные слоты и хот-бар
        {
            if (items[i].id == 0)
            {
                return i;
            }
        }
        return -1; // Все подходящие ячейки заняты
    }
    public Vector3 dropEndPosition;
    public void DropItem()
    {
        if (currentItem != null && currentItem.id != 0)
        {
            Vector3 dropStartPosition = transform.position; // Получаем позицию мыши
            dropStartPosition.z = 0; // Устанавливаем Z в 0, чтобы не было ошибки
            dropStartPosition.y -= 0.8f;

            Vector3 dropDirection = GetDropDirection(); // Получаем направление броска
            float dropDistance = 1f; // Расстояние броска

            // Проверяем наличие коллайдера перед персонажем
            RaycastHit2D hit = Physics2D.Raycast(dropStartPosition, dropDirection, dropDistance, LayerMask.GetMask("Wall"));

            // Если перед персонажем есть препятствие, предмет падает перед препятствием
            if (hit.collider != null && !hit.collider.CompareTag("Player"))
            {
                dropEndPosition = hit.point;
            }
            else
            {
                dropEndPosition = dropStartPosition + dropDirection * dropDistance;
            }

            // Создаем новый экземпляр выброшенного предмета
            GameObject droppedItem = Instantiate(droppedItemPrefab, dropStartPosition, Quaternion.identity);
            droppedItem.GetComponent<SpriteRenderer>().sprite = data.items[currentItem.id].img;

            DroppedItem droppedItemScript = droppedItem.GetComponent<DroppedItem>();
            if (droppedItemScript != null)
            {
                droppedItemScript.itemID = currentItem.id;
                droppedItemScript.count = currentItem.count;
                droppedItemScript.istool = currentItem.istool;
                droppedItemScript.dropEndPosition = dropEndPosition;
            }

            // Уменьшаем размер объекта для отображения на земле
            droppedItem.transform.localScale *= 0.5f;

            // НЕ сбрасывайте позицию объекта в анимации
            StartCoroutine(AnimateItemDrop(droppedItem.transform, dropStartPosition, dropEndPosition));

            // Сбрасываем текущий предмет
            currentID = -1;
            currentItem = null;
            movingObject.gameObject.SetActive(false);
        }
    }





    public Animator animator;
    private Vector3 GetDropDirection()
    {

        // Предполагаем, что в Animator есть параметры "Horizontal" и "Vertical"
        float horizontal = animator.GetFloat("Horizontal");
        float vertical = animator.GetFloat("Vertical");

        // Формируем вектор направления
        Vector3 dropDirection = new Vector3(horizontal, vertical, 0).normalized;

        // Если вектор нулевой (персонаж стоит), выбрасываем предмет вправо как дефолт
        if (dropDirection == Vector3.zero)
        {
            dropDirection = new Vector3(1, 0, 0);
        }

        return dropDirection;
    }


    private IEnumerator AnimateItemDrop(Transform itemTransform, Vector3 start, Vector3 end)
    {
        float duration = 1f; // Общая длительность анимации
        float bounceHeight = 1.0f; // Высота первого отскока
        int bounces = 3; // Количество отскоков
        float gravity = 2.0f; // Ускорение "гравитации"

        Vector3 currentPosition = start; // Используем начальную позицию
        float time = 0f;

        // Выполняем несколько отскоков
        for (int i = 0; i < bounces; i++)
        {
            float bounceDuration = duration / (bounces * 2); // Длительность каждого отскока
            Vector3 bounceApex = new Vector3(end.x, end.y + bounceHeight, end.z); // Пик отскока

            // Подъем до пика
            while (time < bounceDuration)
            {
                currentPosition = Vector3.Lerp(start, bounceApex, time / bounceDuration);
                itemTransform.position = currentPosition;
                time += Time.deltaTime;
                yield return null;
            }

            time = 0f; // Сбрасываем время для падения
            start = bounceApex; // Новый старт - это пик отскока

            // Падение
            while (time < bounceDuration)
            {
                currentPosition = Vector3.Lerp(start, end, time / bounceDuration);
                itemTransform.position = currentPosition;
                time += Time.deltaTime;
                yield return null;
            }

            time = 0f; // Сбрасываем время
            start = end; // Новый старт - это точка приземления
            bounceHeight /= gravity; // Каждый отскок становится ниже
        }

        itemTransform.position = end; // Финальная позиция

        // Вызываем MarkAsLanded() сразу после того, как предмет упал
        DroppedItem droppedItemScript = itemTransform.GetComponent<DroppedItem>();
        if (droppedItemScript != null)
        {
            droppedItemScript.MarkAsLanded();  // Помечаем, что предмет упал
            droppedItemScript.startSwaying = true;  // Запускаем колебания
        }
    }


    public void AddItem(int id, Item item, int count)
{
    // Проверяем, что индекс в пределах допустимого диапазона
    if (id >= items.Count)
    {
        // Если индекс выходит за пределы, добавляем недостающие ячейки
        for (int i = items.Count; i <= id; i++)
        {
            ItemInventory newItem = new ItemInventory();
            items.Add(newItem);
        }
    }

    // Если в ячейке уже есть предметы
    ItemInventory targetItem = items[id];
    
    if (targetItem.itemGameObj == null)
    {
        GameObject newItemObj = Instantiate(gameObjShow);  // Создаем новый объект для предмета
        targetItem.itemGameObj = newItemObj;  // Присваиваем его ячейке
    }
    
    if (targetItem.id == item.id && !item.istool) // Если предметы одинаковые (не инструмент)
    {
        // Суммируем количество в текущей ячейке
        int totalCount = targetItem.count + count;

        if (totalCount <= 100)  // Если все помещается в одну ячейку
        {
            targetItem.count = totalCount;  // Обновляем количество
        }
        else  // Если сумма больше 100
        {
            targetItem.count = 100;  // Заполняем до максимума
            count = totalCount - 100;  // Оставшийся остаток

            // Ищем следующую свободную ячейку для остатка
            int nextEmptySlot = FindNextEmptySlot();
            if (nextEmptySlot != -1)  // Если есть свободная ячейка
            {
                AddItem(nextEmptySlot, item, count);  // Рекурсивно добавляем остаток в следующую ячейку
            }
            else
            {
                // Если нет свободных ячеек, например, можно вывести ошибку
                Debug.LogWarning("Нет места в инвентаре для остатков.");
            }
        }
    }
    else  // Если предметы разные
    {
        // Добавляем новый предмет в эту ячейку
        targetItem.id = item.id;
        targetItem.count = count;
        targetItem.istool = item.istool;
        targetItem.itemGameObj.GetComponent<Image>().sprite = item.img;
    }

    // Обновляем отображение в инвентаре
    UpdateInventory();
}

// Метод для поиска следующей пустой ячейки
private int FindNextEmptySlot()
{
    for (int i = 0; i < items.Count; i++)
    {
        if (items[i].id == 0)  // Если ячейка пуста
        {
            return i;
        }
    }
    return -1;  // Нет пустых ячеек
}


    
    public void AddGraphics()
{
    // Горячая панель (первые hotbarSize ячеек)
    for (int i = 0; i < hotbarSize; i++)
    {
        GameObject newHotbarItem = Instantiate(gameObjShow, hotbarPanel.transform) as GameObject;
        newHotbarItem.name = i.ToString();

        ItemInventory ii = new ItemInventory();
        ii.itemGameObj = newHotbarItem;

        RectTransform rt = newHotbarItem.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;

        InventoryButton buttonHandler = newHotbarItem.AddComponent<InventoryButton>();
        buttonHandler.inventory = this;

        items.Add(ii);
    }

    // Основной инвентарь (следующие maxCount - hotbarSize ячеек)
    for (int i = hotbarSize; i < maxCount; i++)
    {
        GameObject newItem = Instantiate(gameObjShow, InventoryMainObject.transform) as GameObject;
        newItem.name = i.ToString();

        ItemInventory ii = new ItemInventory();
        ii.itemGameObj = newItem;

        RectTransform rt = newItem.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;

        InventoryButton buttonHandler = newItem.AddComponent<InventoryButton>();
        buttonHandler.inventory = this;

        items.Add(ii);
    }

    // Копия инвентаря (следующие inventoryCopySize ячеек)
    for (int i = maxCount; i < maxCount + inventoryCopySize; i++)
    {
        GameObject newCopyItem = Instantiate(gameObjShow, chestPanel.transform) as GameObject;
        newCopyItem.name = i.ToString();

        ItemInventory ii = new ItemInventory();
        ii.itemGameObj = newCopyItem;

        RectTransform rt = newCopyItem.GetComponent<RectTransform>();
        rt.localPosition = Vector3.zero;
        rt.localScale = Vector3.one;

        InventoryButton buttonHandler = newCopyItem.AddComponent<InventoryButton>();
        buttonHandler.inventory = this;

        items.Add(ii);
    }

    maxCount += inventoryCopySize; // Увеличиваем общий размер инвентаря
}




    public void UpdateInventory()
    {
        // Обновление горячей панели
        for (int i = 0; i < hotbarSize; i++)
        {
            if (i < items.Count)
            {
                var textComponent = items[i].itemGameObj.GetComponentInChildren<TextMeshProUGUI>();

                if (items[i].id != 0 && items[i].count > 1 && !data.items[items[i].id].istool)
                {
                    textComponent.text = items[i].count.ToString();
                }
                else
                {
                    textComponent.text = "";
                }

                items[i].itemGameObj.GetComponentInChildren<Image>().sprite = data.items[items[i].id].img;
            }
        }

        // Обновление основного инвентаря
        for (int i = hotbarSize; i < maxCount - inventoryCopySize; i++)
        {
            if (i < items.Count)
            {
                var textComponent = items[i].itemGameObj.GetComponentInChildren<TextMeshProUGUI>();

                if (items[i].id != 0 && items[i].count > 1 && !data.items[items[i].id].istool)
                {
                    textComponent.text = items[i].count.ToString();
                }
                else
                {
                    textComponent.text = "";
                }

                items[i].itemGameObj.GetComponentInChildren<Image>().sprite = data.items[items[i].id].img;
            }
        }

        // Обновление копии инвентаря
        for (int i = maxCount - inventoryCopySize; i < maxCount; i++)
        {
            if (i < items.Count)
            {
                var textComponent = items[i].itemGameObj.GetComponentInChildren<TextMeshProUGUI>();

                if (items[i].id != 0 && items[i].count > 1 && !data.items[items[i].id].istool)
                {
                    textComponent.text = items[i].count.ToString();
                }
                else
                {
                    textComponent.text = "";
                }

                items[i].itemGameObj.GetComponentInChildren<Image>().sprite = data.items[items[i].id].img;
            }
        }
    }





    public void SelectObject()
{
    if (!IsInventoryOpen) return;  // Если инвентарь закрыт, не выполняем действия

    int targetID = int.Parse(es.currentSelectedGameObject.name); // ID целевой ячейки
    ItemInventory targetItem = items[targetID]; // Целевая ячейка

    if (currentID == -1) // Если в мышке ничего нет
    {
        if (targetItem.id != 0) // Если ячейка не пустая
        {
            currentID = targetID; // Выбираем предмет
            currentItem = CopyInventoryItem(targetItem); // Копируем предмет в мышку
            movingObject.gameObject.SetActive(true); 
            movingObject.GetComponent<Image>().sprite = data.items[currentItem.id].img;
            movingObject.GetComponent<RectTransform>().sizeDelta = new Vector2(90, 90);

            AddItem(targetID, data.items[0], 0); // Очищаем ячейку
        }
    }
    else // Если что-то уже в мышке
    {
        if (targetItem.id == 0) // Если целевая ячейка пустая
        {
            AddItem(targetID, data.items[currentItem.id], currentItem.count); // Переносим предметы
            currentID = -1; // Сбрасываем выбор
            currentItem.count = 0;
            movingObject.gameObject.SetActive(false);
        }
        else if (targetItem.id == currentItem.id && !data.items[currentItem.id].istool) // Если предметы одинаковые и не инструмент
        {
            int totalCount = targetItem.count + currentItem.count;

            if (totalCount <= 100) // Если всё влезает
            {
                targetItem.count = totalCount; // Обновляем количество в целевой ячейке
                currentID = -1; // Сбрасываем выбор
                currentItem.count = 0;
                movingObject.gameObject.SetActive(false);
            }
            else // Если не всё влезает
            {
                targetItem.count = 100; // Заполняем целевую ячейку до максимума
                currentItem.count = totalCount - 100; // Оставляем остаток в мышке
                targetItem.itemGameObj.GetComponentInChildren<TextMeshProUGUI>().text = targetItem.count.ToString();
            }
        }
        else if (targetItem.id == currentItem.id && data.items[currentItem.id].istool) // Если предмет — инструмент, то его нельзя стакнуть
        {
            // Ничего не делаем, так как инструменты нельзя стакнуть
        }
        else // Если предметы разные
        {
            // Обмен предметами между текущей и целевой ячейкой
            ItemInventory tempItem = CopyInventoryItem(targetItem); // Копируем предмет из целевой ячейки
            AddItem(targetID, data.items[currentItem.id], currentItem.count); // Перемещаем предмет в целевую ячейку
            AddItem(currentID, data.items[tempItem.id], tempItem.count); // Перемещаем предмет из целевой ячейки в текущую

            currentID = -1; // Сбрасываем выбор
            currentItem.count = 0;
            movingObject.gameObject.SetActive(false);
        }
    }

    UpdateInventory(); // Обновляем инвентарь для отображения изменений
}






    public void MoveObject()
    {
        if (!IsInventoryOpen) return;  // Если инвентарь закрыт, не выполняем действия

        Vector3 pos = Input.mousePosition + offset;
        pos.z = InventoryMainObject.GetComponent<RectTransform>().position.z;
        movingObject.position = cam.ScreenToWorldPoint(pos);

        var movingText = movingObject.GetComponentInChildren<TextMeshProUGUI>();
        if (currentItem != null && currentItem.count > 1)
        {
            movingText.text = currentItem.count.ToString();
        }
        else
        {
            movingText.text = "";
        }
    }


    public void HandleHotbarSelection()
    {
        bool selectionMade = false; // Переменная для отслеживания, была ли выбрана ячейка

        for (int i = 0; i < hotbarSize; i++) 
        {
            if (Input.GetKeyDown((KeyCode)(49 + i)) && !mapController.ismapopen && !PausePanel.activeSelf)  // Проверка на нажатие цифры 1-9
            {
                lastSelectedHotbarSlot = i;
                SelectHotbarItem(i);
                UpdateSelectionFramePosition(i);
                selectionMade = true;
            }
        }

        // Добавляем проверку для клавиши "0" (код 48)
        if (Input.GetKeyDown(KeyCode.Alpha0)) 
        {
            lastSelectedHotbarSlot = 9;
            SelectHotbarItem(9);  // Индекс 9 для "0" в горячей панели
            UpdateSelectionFramePosition(9);
            selectionMade = true;
        }

        // Если была выбрана ячейка, то показываем рамку
        if (selectionMade)
        {
            selectionFrame.SetActive(true);
        }
    }

    public void UpdateSelectionFramePosition(int index)
    {
        if (items[index].itemGameObj == null)
        {
            Debug.LogWarning("itemGameObj для ячейки " + index + " не инициализирован!");
            return;  // Прекращаем выполнение, если объект не инициализирован
        }
        // Получаем позицию ячейки горячей панели
        RectTransform hotbarItemTransform = items[index].itemGameObj.GetComponent<RectTransform>();

        // Обновляем позицию рамки (на основе позиции выбранной ячейки)
        selectionFrameTransform.position = hotbarItemTransform.position;
        
    }

    
    public void SelectHotbarItem(int index)
    {
        if (index < items.Count)
        {
            // Если ячейка не пуста
            if (items[index].id != 0)
            {
                Item selectedItem = data.items[items[index].id];
                
                if (selectedItem.istool)
                {
                    // Если это инструмент, выбираем его
                    WeaponManager.instance.weaponId = items[index].id;
                }
                else
                {
                    // Если это не инструмент, сбрасываем weaponId
                    WeaponManager.instance.weaponId = -1;
                }
                
                UpdateSelectionFramePosition(index);  // Обновляем позицию рамки
            }
            else
            {
                WeaponManager.instance.weaponId = -1;
            }
        }
    }
    private const int MaxStackSize = 100; // Максимальное количество одного предмета в слоте

    public void DropAndPickUpAllItems()
    {
        // Список выброшенных предметов
        List<DroppedItem> droppedItems = new List<DroppedItem>();

        // Выбрасываем только предметы из основного инвентаря (11-30 ячейки)
        for (int i = hotbarSize; i < maxCount - inventoryCopySize; i++)
        {
            if (items[i].id != 0) // Если ячейка не пуста
            {
                // Создаём объект выбрасываемого предмета
                GameObject droppedItemObj = Instantiate(droppedItemPrefab, transform.position, Quaternion.identity);
                droppedItemObj.GetComponent<SpriteRenderer>().sprite = data.items[items[i].id].img;

                DroppedItem droppedItemScript = droppedItemObj.GetComponent<DroppedItem>();
                droppedItemScript.itemID = items[i].id;
                droppedItemScript.count = items[i].count;
                droppedItemScript.istool = items[i].istool; // Сохраняем информацию об инструменте

                // Добавляем выброшенный предмет в список
                droppedItems.Add(droppedItemScript);

                // Очищаем ячейку в инвентаре
                items[i].id = 0;
                items[i].count = 0;
                items[i].istool = false; // Сбрасываем флаг инструмента
            }
        }

        UpdateInventory(); // Обновляем отображение инвентаря

        // Подбираем выброшенные предметы обратно
        StartCoroutine(PickUpItems(droppedItems));
    }

private IEnumerator PickUpItems(List<DroppedItem> droppedItems)
{
    // Ждем короткую задержку перед подбором
    yield return new WaitForSeconds(0f); // Можно регулировать время задержки

    // Сортируем выпавшие предметы по типу (id)
    droppedItems.Sort((a, b) => a.itemID.CompareTo(b.itemID));

    // Подбираем все предметы
    foreach (var droppedItem in droppedItems)
    {
        int remainingCount = droppedItem.count; // Оставшееся количество предметов для добавления

        // Если предмет не стакается (например, это инструмент), добавляем его в первую пустую ячейку
        if (!CanStack(droppedItem))
        {
            for (int i = 10; i < items.Count; i++)
            {
                if (items[i].id == 0) // Если ячейка пуста, добавляем предмет
                {
                    // Добавляем уникальный предмет (например, инструмент)
                    items[i].id = droppedItem.itemID;
                    items[i].count = droppedItem.count; // Устанавливаем количество для уникальных предметов
                    items[i].istool = droppedItem.istool;
                    break; // Переходим к следующему предмету
                }
            }
        }
        else
        {
            // Если предмет стакается, добавляем его в ячейки с тем же типом
            for (int i = 10; i < items.Count; i++)
            {
                if (items[i].id == droppedItem.itemID) // Если предмет того же типа уже есть
                {
                    // Проверяем, можем ли мы добавить в текущую ячейку
                    int countToAdd = Mathf.Min(remainingCount, 100 - items[i].count); // Оставшееся место в ячейке
                    items[i].count += countToAdd;
                    remainingCount -= countToAdd;

                    // Если все предметы добавлены, выходим из цикла
                    if (remainingCount <= 0)
                    {
                        break;
                    }
                }
                else if (items[i].id == 0) // Если ячейка пуста и предмет того типа, который можно добавить
                {
                    int countToAdd = Mathf.Min(remainingCount, 100); // Максимум 100 штук в ячейке
                    items[i].id = droppedItem.itemID;
                    items[i].count = countToAdd;
                    remainingCount -= countToAdd;

                    if (remainingCount <= 0)
                    {
                        break; // Если все предметы добавлены, выходим из цикла
                    }
                }
            }
        }

        // Если предметы не поместились в инвентарь, можно добавить логику для обработки
        if (remainingCount > 0)
        {
            Debug.Log("Инвентарь полный, не все предметы можно поместить!");
        }

        // Уничтожаем объект предмета
        Destroy(droppedItem.gameObject);
    }

    // Обновляем инвентарь после подбора
    UpdateInventory();
}

// Метод для проверки, можно ли сложить предмет (например, инструменты нельзя)
private bool CanStack(DroppedItem droppedItem)
{
    // Пример: предположим, что предметы с определенным id не могут быть сложены
    return droppedItem.istool == false; // Если это инструмент, то он не может быть сложен
}




    public ItemInventory CopyInventoryItem(ItemInventory old)
    {
        ItemInventory New = new ItemInventory();

        New.id = old.id;
        New.itemGameObj = old.itemGameObj;
        New.count = old.count;
        New.istool = old.istool;

        return New;
    }
    
    private IEnumerator CloseInventoryWithDelay(float delay)
    {
        isMovingInventory = true;
        // Ждем заданную задержку

        // Если есть предмет в мышке
        if (currentID != -1)
        {
            // Возвращаем предмет в исходный слот
            AddItem(currentID, data.items[currentItem.id], currentItem.count);
            currentID = -1;
            currentItem.count = 0;
            movingObject.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(delay);
        // После задержки скрываем инвентарь
        IsInventoryOpen = false;
        background.SetActive(false);
        isMovingInventory = false;
    }
    
    
    public void SaveInventory(Storage storage, GameData gameData, string currentSlotFileName)
    {
        gameData.items.Clear();  // Очищаем список, чтобы обновить его

        // Перебираем все предметы в инвентаре и добавляем их в список GameData
        foreach (var item in items)
        {
            gameData.items.Add(new ItemInventoryData(item));  // Добавляем только нужные данные
        }
    }

    // Загрузка инвентаря из GameData
    public void LoadInventory(Storage storage, GameData gameData, string currentSlotFileName)
    {
        gameData = (GameData)storage.Load(currentSlotFileName, new GameData());  // Загружаем сохраненные данные
    
        if (gameData != null)
        {
            // Загружаем данные инвентаря
            for (int i = 0; i < gameData.items.Count; i++)
            {
                if (i < items.Count)
                {
                    ItemInventory item = items[i];
                    var itemData = gameData.items[i];

                    item.id = itemData.id;
                    item.count = itemData.count;
                    item.istool = itemData.istool;
                }
            }
            UpdateInventory();
        }
    }
}


[System.Serializable]
public class ItemInventory
{
    public int id;
    public GameObject itemGameObj;
    public bool istool;
    public int count;
    public ItemInventory() { }
    
    public ItemInventory(ItemInventoryData data)
    {
        id = data.id;
        istool = data.istool;
        count = data.count;
    }
}