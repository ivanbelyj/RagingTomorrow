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
    [SerializeField]
    private RectTransform _gridParent;

    [SerializeField]
    private GameObject _slotPrefab;

    [SerializeField]
    private GameObject _slotItemPrefab;

    // private InventorySlot[,] _slots;
    private float _slotSize;

    private ItemStaticDataManager _itemStaticDataManager;
    private GridSection _gridSection;

    private List<InventorySlot> _slots;
    private SlotItemCreator _slotItemCreator;

    /// <summary>
    /// Иконки отдельных предметов инвентаря. В качестве ключа используется id,
    /// получаемый на основе положения предмета в инвентаре, оно уникально в пределах инвентаря.
    /// Использование словаря позволяет быстрее искать удаляемую иконку при удалении предмета
    /// в инвентаре, за O(1)
    /// </summary>
    private Dictionary<uint, InventorySlotItem> _slotItems;

    private void Awake() {
        RectTransform slotRectTransform = _slotPrefab.GetComponent<RectTransform>();
        // _slotHeight = slotRectTransform.sizeDelta.x;
        if (slotRectTransform.sizeDelta.x != slotRectTransform.sizeDelta.y) {
            Debug.LogError("Slot width and height must be same");
        }
        _slotSize = slotRectTransform.sizeDelta.y;

        _slots = new List<InventorySlot>();
        _slotItems = new Dictionary<uint, InventorySlotItem>();
    }
    private void Start() {
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }

    // public void SetAsPlayersInventoryGrid(IGridSectionInventory inventory) {
    //     SetInventorySection(inventory.GridSection);
    //     SetInventoriesAndCreateItems(inventory, null);
    // }

    // public void SetAsSecondInventoryGrid(IGridSectionInventory inventory,
    //     IGridSectionInventory secondInventory) {
    //     SetInventorySection(secondInventory.GridSection);
    //     SetInventoriesAndCreateItems(secondInventory, inventory);
    // }

    public void Set(GridSection gridSection, SlotItemCreator slotItemCreator) {
        _slotItemCreator = slotItemCreator;
        SetInventorySection(gridSection);
        foreach (GridSectionItem invItem in _gridSection.Items.Values) {
            CreateSlotItem(invItem);
        }
    }

    /// <summary>
    /// Для UI устанавливается новый инвентарь. Например, игрок открывает ящик, и для UI инвентаря
    /// устанавливается инвентарь ящика
    /// </summary>
    private void SetInventorySection(GridSection gridSection) {
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
            _slots.Clear();
            CreateSlots();
        }

        // Созданные ранее иконки предметов удаляются (в любом случае)
        foreach (var pair in _slotItems) {
            Destroy(pair.Value.gameObject);
        }
        _slotItems.Clear();

        _gridSection.InventoryChanged += OnInventoryChanged;
    }

    private void OnInventoryChanged(SyncList<GridSectionItem>.Operation op, int index,
        GridSectionItem oldItem, GridSectionItem newItem) {
        switch (op) {
            case SyncList<GridSectionItem>.Operation.OP_ADD:
            {
                CreateSlotItem(newItem);
                break;
            }
            case SyncList<GridSectionItem>.Operation.OP_REMOVEAT:
            {
                uint oldItemId = oldItem.PlacementId.LocalId;
                InventorySlotItem slotItemToDestroy = _slotItems[oldItemId];

                _slotItems.Remove(oldItemId);
                Destroy(slotItemToDestroy.gameObject);
                break;
            }
        }
    }

    private void CreateSlotItem(GridSectionItem invItem) {
        InventorySlotItem slotItem = _slotItemCreator.CreateItem(invItem, _slotItemPrefab,
            _gridParent, _slotSize);
        _slotItems.Add(invItem.PlacementId.LocalId, slotItem);
    }

    private void CreateSlots() {
        int rows = _gridSection.Height;
        int cols = _gridSection.Width;

        float gridHeight = rows * _slotSize;
        float gridWidth = cols * _slotSize;
        _gridParent.sizeDelta = new Vector2(gridWidth, gridHeight);

        // _slots = new InventorySlot[rows, cols];
        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < cols; c++) {
                CreateSlot(r, c);
            }
        }
    }

    private InventorySlot CreateSlot(int row, int col) {
        // Добавляем в сетку
        GameObject slotGO = Instantiate(_slotPrefab);
        slotGO.transform.SetParent(_gridParent.transform);
        slotGO.transform.localScale = Vector3.one;
        slotGO.transform.localPosition = new Vector3(col * _slotSize, -row * _slotSize);

        InventorySlot invSlot = slotGO.GetComponent<InventorySlot>();
        invSlot.Initialize(_gridSection, row, col);
        _slots.Add(invSlot);
        return invSlot;

        // Обеспечение дальнейшей работы
        // InventorySlot slot = slotGO.GetComponent<InventorySlot>();
        // _slots[row, col] = slot;
    }
}
