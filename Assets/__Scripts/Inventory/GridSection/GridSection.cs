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

        Debug.Log("Grid section items are initialized");
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
        Debug.Log("Remove on server");
        _syncItems.Remove(item);
    }

    [Command]
    private void CmdAddItem(GridSectionItem item) {
        Debug.Log("From CmdAddItem: item is " + item);
        AddItem(item);
    }

    [Command]
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
    public bool TryToAddToSection(ItemData itemData) {
        bool isAdded = TryToAddToUnfilledItemStack(itemData);
        if (!isAdded) {
            return TryToAddToFreePlace(itemData);
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
        foreach (var item in _items) {
            // Стаковать можно только одинаковое, чтобы не потерять различия
            bool sameItem = item.itemData.Equals(itemData);

            // Некоторые предметы, например, не могут стаковаться
            bool itemStackLimitNotReached = item.count < stackSize;
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
        Debug.Log($"Найдена свободная позиция: ({x}, {y})");
        
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

    public void RemoveFromSection(GridSectionItem invItem) {
        if (isServer) {
            Debug.Log("Remove Item");
            RemoveItem(invItem);
        } else {
            Debug.Log("Cmd Remove Item");
            CmdRemoveItem(invItem);
        }
    }

    // For test
    public bool AddTestItems()
    {
        for (int n = 0; n < 10; n++) {
            var newItem = new ItemData() {
                itemStaticDataName = _itemStaticDataManager.NamesAndData.ElementAt(0).Key,
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
