using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Компонент позволяет GameObject хранить набор предметов
/// </summary>
public class Inventory : NetworkBehaviour
{
    private readonly SyncList<InventoryItem> _syncItems = new SyncList<InventoryItem>();
    
    private List<InventoryItem> _items;
    public List<InventoryItem> Items => _items;
    public List<InventoryItem> GetItemsOfSection(int sectionIndex)
        => _items.Where(item => item.inventorySection == sectionIndex).ToList();

    [SerializeField]
    private FillingMatrix[] _sections;

    [SerializeField]
    private int _defaultSection;
    public int DefaultSection => _defaultSection;

    private ItemStaticDataManager _itemDataManager;

    public override void OnStartClient()
    {
        base.OnStartClient();

        // При старте в _syncItems уже могут быть элементы
        _items = new List<InventoryItem>(_syncItems.Count);
        for (int i = 0; i < _syncItems.Count; i++)
        {
            AddItemOnClient(_syncItems[i]);
        }
    }

    private void Awake() {
        if (_sections.Length < 1) {
            Debug.LogError("One section for inventory is necessary");
        }
        if (_defaultSection < 0 || _defaultSection >= _sections.Length) {
            Debug.LogError("Default section value is wrong");
        }
    }

    private void Start()
    {
        _syncItems.Callback += SyncItems;
        _itemDataManager = FindObjectOfType<ItemStaticDataManager>();
        foreach (var fillingMatrix in _sections) {
            fillingMatrix.Initialize();
        }
    }

    public int GetSectionWidth(int sectionIndex) => _sections[sectionIndex].Width;
    public int GetSectionHeight(int sectionIndex) => _sections[sectionIndex].Height;

    public bool AddTestItems()
    {
        for (int n = 0; n < 10; n++) {
            // InventoryItem invItem = new InventoryItem() {
            //     itemGameData = new ItemGameData() {
            //         itemDataName = _itemDataManager.NamesAndData.ElementAt(0).Key
            //     }
            // };
            var itemGameData = new ItemGameData() {
                itemStaticDataName = _itemDataManager.NamesAndData.ElementAt(0).Key
            };
            bool isAdded = FindPlaceAndAddItemToDefaultSection(itemGameData);
            if (!isAdded) {
                // В инвентаре нет места для предметов данного размера
                return false;
            }
        }
        return true;
    }

    // /// <summary>
    // /// Получает свободное место для данного предмета. true, если позиция найдена успешно
    // /// </summary>
    // public bool GetFreePosition(ItemGameData itemData, out int x, out int y) {
    //     Debug.Log("Getting free position for " + itemData.itemDataName);
    //     for (int row = 0; row < height; row++) {
    //         for (int col = 0; col < width; col++) {
    //             if (HasFreeSpaceForItemInPosition(itemData, col, row)) {
    //                 x = col;
    //                 y = row;
    //                 return true;
    //             }
    //         }
    //     }
    //     x = default(int);
    //     y = default(int);
    //     return false;
    // }

    // /// <summary>
    // /// Проверяет, возможно ли добавить данный объект в данную позицию в инвентаре.
    // /// Проходит по списку, проверяя для каждого, не занимает ли он указанное место
    // /// </summary>
    // public bool HasFreeSpaceForItemInPosition(ItemGameData itemGameData,
    //     int inventoryX, int inventoryY) {
    //     ItemStaticData invItemData =  GetItemData(itemGameData.itemDataName);
    //     SlotsRect invItemRect = new SlotsRect() {
    //         x = inventoryX, y = inventoryY,
    //         width = invItemData.Width,
    //         height = invItemData.Height
    //     };
    //     Debug.Log($"\tChecking pos [{inventoryX}, {inventoryY}]. Item size is {invItemData.Width}x{invItemData.Height}");
    //     foreach (var otherItem in Items) {
    //         Debug.Log($"\tIs there {otherItem.itemGameData.itemDataName} on pos?");
    //         ItemStaticData otherItemData = GetItemData(otherItem.itemGameData.itemDataName);
            
    //         SlotsRect otherItemRect = new SlotsRect() {
    //             x = otherItem.inventoryX, y = otherItem.inventoryY,
    //             width = otherItemData.Width,
    //             height = otherItemData.Height
    //         };
    //         if (RectsOverlap(invItemRect, otherItemRect)) {
    //             Debug.Log("\tYes! Rects are overlapping!");
    //             return false;
    //         }
    //     }
    //     Debug.Log("\tPosition is free");
    //     return true;
    // }

    // private struct SlotsRect {
    //     public int x;
    //     public int y;
    //     public int width;
    //     public int height;
    // }

    // private bool RectsOverlap(SlotsRect r1, SlotsRect r2) {
    //     bool overlapWidth = OverlapSide(r => r.x, r => r.width);
    //     Debug.Log($"\t\tDo x sides overlap? [{r1.x}, {r1.x + r1.width}] and [{r2.x},"
    //         + $" {r2.x + r2.width}]. {overlapWidth}");

    //     bool overlapHeight = OverlapSide(r => r.y, r => r.height);
    //     Debug.Log($"\t\tDo y sides overlap? [{r1.y}, {r1.y + r1.height}] and [{r2.y},"
    //         + $" {r2.y + r2.height}]. {overlapWidth}");
    //     return overlapWidth && overlapHeight;

    //     bool OverlapSide(Func<SlotsRect, int> getAxe, Func<SlotsRect, int> getLength)
    //         => IsPointInSegment(getAxe(r2), getAxe(r1), getAxe(r1) + getLength(r1))
    //         || IsPointInSegment(getAxe(r1), getAxe(r2), getAxe(r2) + getLength(r2));
        
    //     bool IsPointInSegment(int point, int segStart, int segEnd)
    //         => point >= segStart && point <= segEnd;
    // }

    /// <summary>
    /// Добавляет предмет и обновляет матрицу заполнения на клиенте.
    /// Используется для поддержания актуальных данных на клиенте в соответствии с
    /// новыми полученными данными об изменении инвентаря
    /// </summary>
    private void AddItemOnClient(InventoryItem invItem) {
        _items.Add(invItem);
        SetFillingRect(invItem, true);
    }

    private void RemoveItemOnClient(InventoryItem invItem) {
        _items.Remove(invItem);
        SetFillingRect(invItem, false);
    }

    private void SetFillingRect(InventoryItem invItem, bool value) {
        ItemStaticData staticData = _itemDataManager.GetItemDataByName(
            invItem.itemGameData.itemStaticDataName);
        
        _sections[invItem.inventorySection]
            .SetRect(staticData.Width, staticData.Height,
            invItem.inventoryX, invItem.inventoryY, value);
    }

    #region Sync
    [Server]
    private void AddItem(InventoryItem item) {
        _syncItems.Add(item);
    }

    [Server]
    public void RemoveItem(InventoryItem item) {
        _syncItems.Remove(item);
    }

    [Command]
    private void CmdAddItem(InventoryItem item) {
        AddItem(item);
    }

    [Command]
    public void CmdRemoveItem(InventoryItem item) {
        RemoveItem(item);
    }

    private void SyncItems(SyncList<InventoryItem>.Operation op, int index,
        InventoryItem oldItem, InventoryItem newItem) {
        switch (op) {
            case SyncList<InventoryItem>.Operation.OP_ADD:
            {
                AddItemOnClient(newItem);
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
                RemoveItemOnClient(oldItem);
                break;
            }
            case SyncList<InventoryItem>.Operation.OP_SET:
            {

                break;
            }
        }
    }
    #endregion

    public bool FindPlaceAndAddItemToDefaultSection(ItemGameData itemGameData) {
        ItemStaticData staticData = GetItemData(itemGameData.itemStaticDataName);
        bool hasFreePlace = _sections[_defaultSection].FindFreeRectPos(
            staticData.Width, staticData.Height,
            out int x, out int y);
        if (!hasFreePlace)
            return false;
        
        InventoryItem invItem = new InventoryItem() {
            count = 1,
            inventoryX = x,
            inventoryY = y,
            itemGameData = itemGameData,
            inventorySection = _defaultSection
        };
        if (isServer) {
            AddItem(invItem);
        } else {
            CmdAddItem(invItem);
        }
        return true;
    }

    public event SyncList<InventoryItem>.SyncListChanged InventoryChanged {
        add {
            _syncItems.Callback += value;
        }
        remove {
            _syncItems.Callback -= value;
        }
    }

    public float TotalWeight => Items.Sum(item => GetItemData(item.itemGameData.itemStaticDataName).Mass);

    public ItemStaticData GetItemData(string itemDataName)
    {
        return _itemDataManager.NamesAndData[itemDataName];
    }
}
