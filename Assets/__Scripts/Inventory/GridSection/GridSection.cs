using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class GridSection : NetworkBehaviour, ITotalWeight
{
    // Для удобства в инспекторе
    [SerializeField]
    private string _sectionName;

    private readonly SyncList<GridSectionItem> _syncItems = new SyncList<GridSectionItem>();
    
    private List<GridSectionItem> _items;
    public List<GridSectionItem> Items => _items;

    protected ItemStaticDataManager _itemStaticDataManager;

    public event SyncList<GridSectionItem>.SyncListChanged InventoryChanged {
        add {
            _syncItems.Callback += value;
        }
        remove {
            _syncItems.Callback -= value;
        }
    }

    // public float TotalWeight
    //     => _items.Sum(item => GetItemData(item.itemStaticDataName).Mass);

    // private FillingMatrix _sectionFilling;
    [SerializeField]
    private int _initialWidth;
    [SerializeField]
    private int _initialHeight;
    public int Width { get; private set; }
    public int Height { get; private set; }

    public ItemStaticData GetItemData(string itemDataName)
    {
        return _itemStaticDataManager.NamesAndData[itemDataName];
    }

    private void Awake() {
        _syncItems.Callback += SyncItems;
        Width = _initialWidth;
        Height = _initialHeight;
        // _sectionFilling = new FillingMatrix(_initialHeight, _initialWidth);
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();

        // Todo: не всегда ли 0?
        _items = new List<GridSectionItem>(_syncItems.Count);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // При старте в _syncItems уже могут быть элементы
        for (int i = 0; i < _syncItems.Count; i++)
        {
            _items.Add(_syncItems[i]);
        }

        // Debug.Log("Grid section items are initialized");
    }

    public float GetTotalWeight()
        => _items.Sum(item => GetItemData(item.itemData.itemStaticDataName).Mass);

    #region Sync
    
    [Server]
    private void AddItem(GridSectionItem item) {
        _syncItems.Add(item);
    }

    [Server]
    private void RemoveItem(GridSectionItem item) {
        Debug.Log($"Remove grid section item on server: " + item);
        _syncItems.Remove(item);
    }

    [Command(requiresAuthority = false)]
    private void CmdAddItem(GridSectionItem item) {
        Debug.Log("From CmdAddItem: item is " + item);
        AddItem(item);
    }

    [Command(requiresAuthority = false)]
    private void CmdRemoveItem(GridSectionItem item) {
        RemoveItem(item);
    }

    private void SyncItems(SyncList<GridSectionItem>.Operation op, int index,
        GridSectionItem oldItem, GridSectionItem newItem) {
        switch (op) {
            case SyncList<GridSectionItem>.Operation.OP_ADD:
            {
                _items.Add(newItem);
                break;
            }
            case SyncList<GridSectionItem>.Operation.OP_REMOVEAT:
            {
                _items.Remove(oldItem);
                break;
            }
        }
    }
    #endregion

    // private void SetFillingRect(GridSectionItem gridItem, bool value) {
    //     ItemStaticData staticData = _itemStaticDataManager.GetStaticDataByName(
    //         gridItem.itemData.itemStaticDataName);
        
    //     _sectionFilling.SetRect(staticData.Width, staticData.Height,
    //         gridItem.inventoryX, gridItem.inventoryY, value);
    // }

    #region Filling Matrix Operations
    /// <summary>
    /// Находит координаты верхнего левого угла свободного прямоугольника, в который можно
    /// поместить данный предмет. true, если такой прямоугольник найден.
    /// Асимптотика: O(n^2)
    /// </summary>
    private bool FindFreePos(FillingMatrix sectionFilling, ItemData item, out int x, out int y) {
        // O(log(n))
        ItemStaticData staticData = _itemStaticDataManager.GetStaticDataByName(
            item.itemStaticDataName);
        
        // O(n^2)
        bool hasFreePlace = sectionFilling.FindFreeRectPos(
            staticData.Width, staticData.Height,
            out int resX, out int resY);
        x = resX;
        y = resY;

        return hasFreePlace;
    }

    /// <summary>
    /// Получает информацию о заполненности инвентаря.
    /// Асимптотика: O(n * log(n))
    /// </summary>
    private FillingMatrix GetFillingMatrix() {
        // O(n * log(n))
        var fillingRects = new FillingMatrix.FillingRect[_items.Count];
        for (int i = 0; i < _items.Count; i++) {
            // O(log(n))
            var itemStaticData = _itemStaticDataManager
                .GetStaticDataByName(_items[i].itemData.itemStaticDataName);
            fillingRects[i] = new FillingMatrix.FillingRect() {
                height = itemStaticData.Height,
                width = itemStaticData.Width,
                x = _items[i].inventoryX,
                y = _items[i].inventoryY,
            };
        }

        // O(n)
        return FillingMatrix.Create(_initialHeight, _initialWidth, fillingRects);
    }
    #endregion

    #region Add And Remove
    private FillingMatrix.FillingRect GetRectForItem(GridSectionItem gridItem) {
        ItemStaticData staticData = _itemStaticDataManager
            .GetStaticDataByName(gridItem.itemData.itemStaticDataName);

        FillingMatrix.FillingRect itemRect = new FillingMatrix.FillingRect() {
            width = staticData.Width,
            height = staticData.Height,
            x = gridItem.inventoryX,
            y = gridItem.inventoryY
        };
        return itemRect;
    }

    /// <summary>
    /// Добавляет предмет с заданными параметрами, специфичными для сеточного инвентаря.
    /// </summary>
    /// <param name="ignoreOldItemFilling">
    /// Если добавляемый предмет на самом деле перемещается и перед добавлением уже
    /// присутствует в инвентаре, то занятое им место игнорируется, т.к. будет
    /// гарантированно освобождено после добавления (перемещения) предмета
    /// </param>
    public bool TryToAddGridSectionItem(GridSectionItem gridItem,
        GridSectionItem ignoreOldItemFilling) {
        FillingMatrix filling = GetFillingMatrix();
        var itemRect = GetRectForItem(gridItem);

        if (ignoreOldItemFilling is not null) {
            filling.SetRect(GetRectForItem(ignoreOldItemFilling), false);
            // Это позволяет перемещать, например, предмет 2x2 на один слот
            // вверх/вниз/вправо/влево от предыдущей позиции
        }
        
        if (!filling.HasPlaceForRect(itemRect))
            return false;

        if (isServer) {
            AddItem(gridItem);
        } else {
            CmdAddItem(gridItem);
        }

        return true;
    }

    public bool TryToAddToSection(ItemData itemData) {
        Debug.Log("Попытка добавления элемента в секцию");
        bool isAdded = TryToAddToUnfilledItemStack(itemData);
        Debug.Log("\tУдалось добавить в имеющийся стак: " + isAdded);
        if (!isAdded) {
            bool res = TryToAddToFreePlace(itemData);
            Debug.Log("\tНайдено свободное место: " + res);
            return res;
        } else {
            return true;
        }
    }

    private bool TryToAddToUnfilledItemStack(ItemData itemData) {
        GridSectionItem unfilled = FindUnfilledItemStack(itemData);
        if (unfilled is null) {
            return false;
        }
        RemoveFromSection(unfilled);
        unfilled.count += 1;
        if (isServer) {
            AddItem(unfilled);
        } else {
            CmdAddItem(unfilled);
        }
        return true;
    }

    /// <summary>
    /// Находит стак предметов, который не заполнен до максимального значения
    /// </summary>
    private GridSectionItem FindUnfilledItemStack(ItemData itemData) {
        ItemStaticData staticData =
            _itemStaticDataManager.GetStaticDataByName(itemData.itemStaticDataName);
        int stackSize = staticData.StackSize;
        Debug.Log("Find unfilled item stack");
        Debug.Log("_items.Count: " + _items.Count);
        foreach (var item in _items) {
            // Стаковать можно только одинаковое, чтобы не потерять различия
            bool sameItem = item.itemData.Equals(itemData);
            Debug.Log($"\tМожно ли застаковать {itemData.itemStaticDataName} в "
                + $"({item.itemData}, {item.count})?");

            // Некоторые предметы, например, не могут стаковаться
            bool itemStackLimitNotReached = item.count < stackSize;

            Debug.Log($"\tЗаполнен ли стек {item.itemData.itemStaticDataName}?" +
                $"{!itemStackLimitNotReached}");

            if (sameItem && itemStackLimitNotReached) {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    /// Находит свободное место в инвентаре и добавляет туда предмет
    /// </summary>
    private bool TryToAddToFreePlace(ItemData itemData) {
        Debug.Log($"Добавление предмета {itemData.itemStaticDataName} в свободное место.");
        Debug.Log($"\tПостроение матрицы заполненности");
        // O(n * log(n))
        FillingMatrix fillingMatrix = GetFillingMatrix();

        // O(n^2)
        bool freePosIsFound = FindFreePos(fillingMatrix, itemData, out int x, out int y);
        if (!freePosIsFound)
            return false;
        Debug.Log($"\tНайдена свободная позиция: ({x}, {y})");
        
        GridSectionItem gridItem = new GridSectionItem() {
            itemData = itemData,
            count = 1,
            inventoryX = x,
            inventoryY = y
        };

        if (isServer) {
            AddItem(gridItem);
        } else {
            CmdAddItem(gridItem);
        }
        return true;
        
        // return AddToSection(gridItem);
    }

    // private bool AddToSection(GridSectionItem gridItem) {
    //     bool isPlaceFree = FindFreePos(gridItem.itemData, out int x, out int y);;
    //     if (!isPlaceFree)
    //         return false;
        
    //     if (isServer) {
    //         AddItem(gridItem);
    //     } else {
    //         CmdAddItem(gridItem);
    //     }

    //     return true;
    // }

    /// <summary>
    /// Удаляет стак предметов из секции
    /// </summary>
    public bool RemoveFromSection(GridSectionItem invItem) {
        Debug.Log("Remove from section - has authority: " + this.hasAuthority);
        bool hasItem = Items.IndexOf(invItem) != -1;
        if (!hasItem)
            return false;
        
        if (isServer) {
            Debug.Log("Remove Item");
            RemoveItem(invItem);
        } else {
            Debug.Log("Cmd Remove Item");
            CmdRemoveItem(invItem);
        }
        return true;
    }

    // For test
    public bool AddTestItems()
    {
        // To fix: добавление каждого нового значения опирается на синхронизированные данные.
        // Однако, насколько я понял, обновление этих данных происходит в следующем кадре,
        // поэтому каждое последующее добавление в текущем кадре считает, что синхронизированные
        // данные пусты
        for (int n = 0; n < 1; n++) {
            var newItem = new ItemData() {
                // itemStaticDataName = _itemStaticDataManager.NamesAndData.ElementAt(0).Key,
                itemStaticDataName = "TestArmor"
            };
            bool isAdded = TryToAddToSection(newItem);
            if (!isAdded) {
                // В инвентаре нет места для предметов данного размера
                return false;
            }
        }
        return true;
    }
    #endregion
}
