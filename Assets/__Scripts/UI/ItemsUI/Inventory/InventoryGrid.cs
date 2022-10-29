using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class InventoryGrid : MonoBehaviour
{
    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup;

    [SerializeField]
    private GameObject _slotPrefab;

    private InventorySlot[,] _slots;

    private ItemStaticDataManager _itemStaticDataManager;

    private Inventory _inventory;
    // public int Cols {
    //     get => _gridLayoutGroup.constraintCount;
    //     set => _gridLayoutGroup.constraintCount = value;
    // }

    // public int Rows {
    //     get => _rows;
    //     set => _rows = value;
    // }

    // public Inventory Inventory {
    //     get => _inventory;
    //     set {
    //         _inventory = value;
    //         UpdateByInventory(value);
    //     }
    // }

    private void Start() {
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }

    /// <summary>
    /// Для UI устанавливается новый инвентарь
    /// </summary>
    public void SetInventory(Inventory inventory) {
        // Сбрасываем все, что связано со старым инвентарем
        Inventory oldInventory = _inventory;
        _inventory = inventory;
        bool areSlotsSet = false;
        if (oldInventory is not null) {
            oldInventory.OnInventoryChanged -= OnInventoryChanged;

            // Если старый инвентарь такого же размера, можно не создавать слоты заново
            if (oldInventory.width == _inventory.width
                && oldInventory.height == _inventory.height) {
                areSlotsSet = true;
            }
        }

        if (!areSlotsSet) {
            // Todo:
            // ClearSlots();
            SetSlots();
        }

        _inventory.OnInventoryChanged += OnInventoryChanged;
    }

    private void OnInventoryChanged(SyncList<InventoryItem>.Operation op, int index,
        InventoryItem oldItem, InventoryItem newItem) {
        switch (op) {
            case SyncList<InventoryItem>.Operation.OP_ADD:
            {
                // items.Add(newItem);
                SetItemToSlot(newItem);
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

    private void SetSlots() {
        int rows = _inventory.width;
        int cols = _inventory.height;
        _slots = new InventorySlot[rows, cols];
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < cols; c++) {
                SetSlot(r, c);
            }
        }
    }

    /// <summary>
    /// Создает GameObject слота, который может быть пустым или содержать предмет
    /// </summary>
    private void SetSlot(int row, int col) {
        // Добавляем слот в сетку
        GameObject slotGO = Instantiate(_slotPrefab);
        slotGO.transform.SetParent(_gridLayoutGroup.transform);
        slotGO.transform.localScale = Vector3.one;

        InventorySlot slot = slotGO.GetComponent<InventorySlot>();
        _slots[row, col] = slot;

        // slot.Row = row;
        // slot.Col = col;
    }

    private void SetItems() {
        foreach (InventoryItem invItem in _inventory.items) {
            SetItemToSlot(invItem);
        }
    }

    private void SetItemToSlot(InventoryItem invItem) {
        int x = invItem.inventoryX;
        int y = invItem.inventoryY;
        ItemStaticData staticData
            = _itemStaticDataManager.GetItemDataByName(invItem.itemGameData.itemDataName);

        Debug.Log($"Set slot [{x}, {y}] with item {invItem.itemGameData.itemDataName}; "
            + $"_slots is {_slots.GetLength(0)}x{_slots.GetLength(1)}");

        // Todo: рефакторинг, сделать ItemStaticDataManager singleton'ом и инкапсулировать
        // установку слота по InventoryItem внутри InventorySlot
        _slots[x, y].SetSprite(staticData.Sprite);
    }
}
