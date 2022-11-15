using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/// <summary>
/// UI для отображения сетки с предметами инвентаря
/// </summary>
public class InventoryGridUI : MonoBehaviour
{
    /// <summary>
    /// Расстояние между слотами в сетке по вертикали и горизонтали
    /// </summary>
    [SerializeField]
    private float gridSpacing = 3;

    [SerializeField]
    private RectTransform _gridParent;

    [SerializeField]
    private GameObject _slotPrefab;

    [SerializeField]
    private GameObject _slotItemPrefab;

    // private InventorySlot[,] _slots;
    private float _slotWidth;
    private float _slotHeight;

    private ItemStaticDataManager _itemStaticDataManager;
    private GridSection _gridSection;
    // private string _inventorySectionName;
    // private InventorySection _invSection;

    private List<InventorySlot> _slots;
    private List<InventorySlotItem> _slotItems;

    private void Awake() {
        RectTransform slotRectTransform = _slotPrefab.GetComponent<RectTransform>();
        _slotHeight = slotRectTransform.sizeDelta.x;
        _slotWidth = slotRectTransform.sizeDelta.y;

        _slots = new List<InventorySlot>();
        _slotItems = new List<InventorySlotItem>();
    }
    private void Start() {
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }

    /// <summary>
    /// Для UI устанавливается новый инвентарь. Например, игрок открывает ящик, и для UI инвентаря
    /// устанавливается инвентарь ящика
    /// </summary>
    public void SetInventorySection(GridSection gridSection) {
        // Сбрасываем все, что связано со старой сеткой
        GridSection oldInvGrid = _gridSection;
        _gridSection = gridSection;

        bool areSlotsSet = false;
        if (oldInvGrid is not null) {
            oldInvGrid.InventoryChanged -= OnInventoryChanged;

            // Если секция старого инвентаря такого же размера, можно не создавать слоты заново
            if (oldInvGrid.Width == _gridSection.Width
                && oldInvGrid.Height == _gridSection.Height) {
                areSlotsSet = true;
            }
        }

        if (!areSlotsSet) {
            // Старые слоты удаляются
            foreach (var slot in _slots) {
                Destroy(slot.gameObject);
            }
            CreateSlots();
        }
        // Созданные ранее UI объектов удаляются
        foreach (var slotItem in _slotItems) {
            Destroy(slotItem.gameObject);
        }
        CreateItems();

        _gridSection.InventoryChanged += OnInventoryChanged;
    }

    private void OnInventoryChanged(SyncList<GridSectionItem>.Operation op, int index,
        GridSectionItem oldItem, GridSectionItem newItem) {
        switch (op) {
            case SyncList<GridSectionItem>.Operation.OP_ADD:
            {
                CreateItem(newItem);
                break;
            }
            case SyncList<GridSectionItem>.Operation.OP_REMOVEAT:
            {
                // items.Remove(oldGridItem);
                // Todo: Equals
                InventorySlotItem slotItemToDestroy =
                    _slotItems.Find(x => x.GridSectionItem.inventoryX == oldItem.inventoryX
                    && x.GridSectionItem.inventoryY == oldItem.inventoryY);
                Debug.Log("_slotItems.Count: " + _slotItems.Count);
                Debug.Log("slotItemToRemove: " + slotItemToDestroy
                    + $"; name: {slotItemToDestroy.GridSectionItem.itemData.itemStaticDataName}");
                Debug.Log("Удален ли slotItem? " + _slotItems.Remove(slotItemToDestroy));
                Destroy(slotItemToDestroy.gameObject);
                break;
            }
        }
    }

    private void CreateSlots() {
        int rows = _gridSection.Height;
        int cols = _gridSection.Width;

        float gridHeight = rows * _slotHeight + gridSpacing * rows;
        float gridWidth = cols * _slotWidth +  + gridSpacing * cols;
        _gridParent.sizeDelta = new Vector2(gridWidth, gridHeight);

        // _slots = new InventorySlot[rows, cols];
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < cols; c++) {
                CreateSlot(r, c);
            }
        }
    }

    private void CreateItems() {
        foreach (GridSectionItem invItem in _gridSection.Items) {
            CreateItem(invItem);
        }
    }

    private InventorySlot CreateSlot(int row, int col) {
        // Добавляем в сетку
        GameObject slotGO = Instantiate(_slotPrefab);
        slotGO.transform.SetParent(_gridParent.transform);
        slotGO.transform.localScale = Vector3.one;

        SetPositionInGrid(slotGO, row, col);
        InventorySlot invSlot = slotGO.GetComponent<InventorySlot>();
        _slots.Add(invSlot);
        return invSlot;

        // Обеспечение дальнейшей работы
        // InventorySlot slot = slotGO.GetComponent<InventorySlot>();
        // _slots[row, col] = slot;
    }
    
    private InventorySlotItem CreateItem(GridSectionItem invItem) {
        int col = invItem.inventoryX;
        int row = invItem.inventoryY;
        Debug.Log("Добавление элемента в сетку. Позиция: (" + col + ", " + row + ")");

        // Добавляем в сетку
        GameObject itemGO = Instantiate(_slotItemPrefab);
        itemGO.transform.SetParent(_gridParent.transform);
        itemGO.transform.localScale = Vector3.one;
        SetPositionInGrid(itemGO, row, col);

        InventorySlotItem slotItem = itemGO.GetComponent<InventorySlotItem>();
        slotItem.Initialize(invItem, _itemStaticDataManager, _slotWidth, _slotHeight, gridSpacing);
        // slotItem.SetItem(invItem);

        _slotItems.Add(slotItem);
        return slotItem;

        // Debug.Log($"Set slot [{x}, {y}] with item {invItem.itemGameData.itemDataName}; "
        //     + $"_slots is {_slots.GetLength(0)}x{_slots.GetLength(1)}");

        // Todo: рефакторинг, инкапсулировать
        // установку слота по InventoryItem внутри InventorySlot
        // _slots[x, y].SetItem(invItem);
    }

    /// <summary>
    /// Устанавливает позицию в сетке для GameObject. Например, для слота или предмета слота
    /// </summary>
    private void SetPositionInGrid(GameObject go, int row, int col) {
        // Положение в сетке
        float posX = col * _slotWidth;
        float posY = row * _slotHeight;
        float spacingX = col * gridSpacing;
        float spacingY = row * gridSpacing;
        go.transform.localPosition = new Vector3(posX + spacingX, -posY - spacingY);
    }
}
