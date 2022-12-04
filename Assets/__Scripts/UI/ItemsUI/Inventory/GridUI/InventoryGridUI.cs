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
    private float _slotSize;
    // private float _slotHeight;

    private ItemStaticDataManager _itemStaticDataManager;
    private GridSection _gridSection;
    // private string _inventorySectionName;
    // private InventorySection _invSection;

    private List<InventorySlot> _slots;

    /// <summary>
    /// Иконки отдельных предметов инвентаря. В качестве ключа используется id,
    /// получаемый на основе положения предмета в инвентаре, оно уникально.
    /// Использование словаря позволяет быстрее искать удаляемую иконку при удалении предмета
    /// в инвентаре, за O(log(n))
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
            _slots.Clear();
            CreateSlots();
        }

        // Созданные ранее иконки предметов удаляются (в любом случае)
        foreach (var pair in _slotItems) {
            Destroy(pair.Value.gameObject);
        }
        _slotItems.Clear();

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
                
                uint oldItemId = oldItem.GetLocalIdByInventoryPosition();
                InventorySlotItem slotItemToDestroy = _slotItems[oldItemId];
                    // _slotItems.Find(x => x.GridSectionItem.inventoryX == oldItem.inventoryX
                    // && x.GridSectionItem.inventoryY == oldItem.inventoryY);
                
                // Debug.Log("_slotItems.Count: " + _slotItems.Count);
                // Debug.Log("slotItemToRemove: " + slotItemToDestroy
                //     + $"; name: {slotItemToDestroy.GridSectionItem.itemData.itemStaticDataName}");
                // Debug.Log("Удален ли slotItem? " + _slotItems.Remove(slotItemToDestroy));

                _slotItems.Remove(oldItemId);
                Destroy(slotItemToDestroy.gameObject);
                break;
            }
        }
    }

    private void CreateSlots() {
        int rows = _gridSection.Height;
        int cols = _gridSection.Width;

        float gridHeight = rows * _slotSize + gridSpacing * rows;
        float gridWidth = cols * _slotSize +  + gridSpacing * cols;
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
        invSlot.Initialize(_gridSection, row, col);
        _slots.Add(invSlot);
        return invSlot;

        // Обеспечение дальнейшей работы
        // InventorySlot slot = slotGO.GetComponent<InventorySlot>();
        // _slots[row, col] = slot;
    }
    
    private InventorySlotItem CreateItem(GridSectionItem invItem) {
        int col = invItem.inventoryX;
        int row = invItem.inventoryY;
        // Debug.Log("Добавление элемента в сетку. Позиция: (" + col + ", " + row + ")");

        // Добавляем в сетку
        GameObject itemGO = Instantiate(_slotItemPrefab);
        itemGO.transform.SetParent(_gridParent.transform);
        itemGO.transform.localScale = Vector3.one;
        SetPositionInGrid(itemGO, row, col);

        InventorySlotItem slotItem = itemGO.GetComponent<InventorySlotItem>();
        uint localId = invItem.GetLocalIdByInventoryPosition();
        slotItem.Initialize(invItem.itemData, _slotSize, gridSpacing,
            invItem.Count, localId, _gridSection.GetComponent<NetworkIdentity>().netId);

        _slotItems.Add(localId, slotItem);
        return slotItem;

        // Debug.Log($"Set slot [{x}, {y}] with item {invItem.itemGameData.itemDataName}; "
        //     + $"_slots is {_slots.GetLength(0)}x{_slots.GetLength(1)}");
    }

    /// <summary>
    /// Устанавливает позицию в сетке для GameObject. Например, для слота или предмета слота
    /// </summary>
    private void SetPositionInGrid(GameObject go, int row, int col) {
        // Положение в сетке
        float posX = col * _slotSize;
        float posY = row * _slotSize;
        float spacingX = col * gridSpacing;
        float spacingY = row * gridSpacing;
        go.transform.localPosition = new Vector3(posX + spacingX, -posY - spacingY);
    }
}
