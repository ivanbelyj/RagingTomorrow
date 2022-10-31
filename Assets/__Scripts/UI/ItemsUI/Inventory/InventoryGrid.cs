using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/// <summary>
/// UI для отображения определенной секции инвентаря
/// </summary>
public class InventoryGrid : MonoBehaviour
{
    /// <summary>
    /// Расстояние между слотами в сетке по вертикали и горизонтали
    /// </summary>
    public float gridSpacing = 3;

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
    private Inventory _inventory;
    private int _inventorySection;

    private void Awake() {
        RectTransform slotRectTransform = _slotPrefab.GetComponent<RectTransform>();
        _slotHeight = slotRectTransform.sizeDelta.x;
        _slotWidth = slotRectTransform.sizeDelta.y;
    }
    private void Start() {
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }

    /// <summary>
    /// Для UI устанавливается новый инвентарь. Например, игрок открывает ящик, и для UI инвентаря
    /// устанавливается инвентарь ящика
    /// </summary>
    public void SetInventory(Inventory inventory, int section) {
        // Сбрасываем все, что связано со старым инвентарем
        Inventory oldInventory = _inventory;
        _inventory = inventory;

        int oldSection = _inventorySection;
        _inventorySection = section;

        bool areSlotsSet = false;
        if (oldInventory is not null) {
            oldInventory.InventoryChanged -= OnInventoryChanged;

            // Если секция старого инвентаря такого же размера, можно не создавать слоты заново
            if (oldInventory.GetSectionWidth(oldSection)
                == _inventory.GetSectionWidth(_inventorySection)
                && oldInventory.GetSectionHeight(oldSection)
                == _inventory.GetSectionHeight(_inventorySection)) {
                areSlotsSet = true;
            }
        }

        if (!areSlotsSet) {
            // Todo:
            // ClearSlots();
            CreateSlots();
        }

        _inventory.InventoryChanged += OnInventoryChanged;
    }

    private void OnInventoryChanged(SyncList<InventoryItem>.Operation op, int index,
        InventoryItem oldItem, InventoryItem newItem) {
        switch (op) {
            case SyncList<InventoryItem>.Operation.OP_ADD:
            {
                // Изменения в других секциях инвентаря игнорируются для данной сетки
                if (newItem.inventorySection == _inventorySection)
                    CreateItem(newItem);
                break;
            }
            case SyncList<InventoryItem>.Operation.OP_CLEAR:
            {

                break;
            }
            case SyncList<InventoryItem>.Operation.OP_INSERT:
            {

                break;
            }
            case SyncList<InventoryItem>.Operation.OP_REMOVEAT:
            {
                // items.Remove(oldItem);
                break;
            }
            case SyncList<InventoryItem>.Operation.OP_SET:
            {

                break;
            }
        }
    }

    private void CreateSlots() {
        int rows = _inventory.GetSectionHeight(_inventorySection);
        int cols = _inventory.GetSectionWidth(_inventorySection);

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

    private void CreateSlot(int row, int col) {
        // Добавляем в сетку
        GameObject slotGO = Instantiate(_slotPrefab);
        slotGO.transform.SetParent(_gridParent.transform);
        slotGO.transform.localScale = Vector3.one;

        SetPositionInGrid(slotGO, row, col);

        // Обеспечение дальнейшей работы
        // InventorySlot slot = slotGO.GetComponent<InventorySlot>();
        // _slots[row, col] = slot;
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

    private void CreateItems() {
        foreach (InventoryItem invItem in _inventory.Items) {
            CreateItem(invItem);
        }
    }

    private void CreateItem(InventoryItem invItem) {
        int col = invItem.inventoryX;
        int row = invItem.inventoryY;

        // Добавляем в сетку
        GameObject itemGO = Instantiate(_slotItemPrefab);
        itemGO.transform.SetParent(_gridParent.transform);
        itemGO.transform.localScale = Vector3.one;
        SetPositionInGrid(itemGO, row, col);

        InventorySlotItem slotItem = itemGO.GetComponent<InventorySlotItem>();
        slotItem.Initialize(_itemStaticDataManager, _slotWidth, _slotHeight, gridSpacing);
        slotItem.SetItem(invItem);

        // Debug.Log($"Set slot [{x}, {y}] with item {invItem.itemGameData.itemDataName}; "
        //     + $"_slots is {_slots.GetLength(0)}x{_slots.GetLength(1)}");

        // Todo: рефакторинг, инкапсулировать
        // установку слота по InventoryItem внутри InventorySlot
        // _slots[x, y].SetItem(invItem);

    }
}
