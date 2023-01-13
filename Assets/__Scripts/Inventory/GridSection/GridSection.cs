using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class GridSection : NetworkBehaviour, IItemsProvider
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
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();

        _items = new List<GridSectionItem>();
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
        => _items.Sum(item => GetItemData(item.itemData.ItemStaticDataName).Mass);

    #region Sync
    
    [Server]
    private void AddItem(GridSectionItem item) {
        _syncItems.Add(item);
    }

    [Server]
    private void RemoveItem(GridSectionItem item) {
        // Debug.Log($"Remove grid section item on server: " + item);
        _syncItems.Remove(item);
    }

    [Command(requiresAuthority = false)]
    private void CmdAddItem(GridSectionItem item) {
        // Debug.Log("From CmdAddItem: item is " + item);
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

    #region Filling Matrix Operations
    /// <summary>
    /// Находит координаты верхнего левого угла свободного прямоугольника, в который можно
    /// поместить данный предмет. true, если такой прямоугольник найден.
    /// Асимптотика: O(n^2)
    /// </summary>
    private bool FindFreePos(FillingMatrix sectionFilling, ItemData item, out int x, out int y) {
        // O(log(n))
        ItemStaticData staticData = _itemStaticDataManager.GetStaticDataByName(
            item.ItemStaticDataName);
        
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
                .GetStaticDataByName(_items[i].itemData.ItemStaticDataName);
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

    #region CanAdd
    /// <summary>
    /// Находит стаки предметов, которые не заполнены до максимального значения
    /// и могут поместить предмет в заданном количестве. Если заданное количество предметов
    /// не помещается в существующие стаки инвентаря, то в качестве результата возвращается
    /// число таких непомещающихся предметов. В случае, если все помещается - 0.
    /// В некоторых случаях возвращаемое значение отрицательно: выбранные стаки могут принять еще больше
    /// предметов, чем требуется
    /// </summary>
    private int FindUnfilledItemStacksForCount(ItemData itemData, int count,
        out List<GridSectionItem> unfilled) {
        int maxStackSize = _itemStaticDataManager
            .GetStaticDataByName(itemData.ItemStaticDataName).StackSize;
        
        // Количество, для которого ищутся незаполненные стаки
        int countToAdd = count;
        unfilled = new List<GridSectionItem>();
        foreach (GridSectionItem gridItem in _items) {
            // Стаковать можно только одинаковое, чтобы не потерять различия
            bool areSameItems = gridItem.itemData.Equals(itemData);

            // Сколько предметов может принять стак
            // Некоторые предметы, например, не могут стаковаться, либо полностью заполнены
            int stackCanAccept = maxStackSize - gridItem.count;
            bool itemStackLimitNotReached = stackCanAccept > 0;

            if (areSameItems && itemStackLimitNotReached) {
                // Стак такого же типа, и не заполнен, значит, может взять по крайней мере часть
                // заданного количества предмета
                unfilled.Add(gridItem);
                countToAdd -= stackCanAccept;
            }

            // Если незаполненные стаки, чтобы поместить заданное количество предмета,
            // найдены, значит, пора вернуть результат
            // if (countToAdd <= 0) {
            //     return unfilled;
            // }
        }
        return countToAdd;
    }

    // См. описание FindUnfilledItemStackForCount
    // private int CanAddToUnfilledItemStacks(ICountableItem countableItem, out List<GridSectionItem> unfilled) {
    //     int canNotAdd = FindUnfilledItemStacksForCount(countableItem, out var unfilledStacks);
    //     unfilled = unfilledStacks;
    //     return unfilled != null && unfilled.Count > 0;
    // }

    // private bool CanAddToFreePlace(ItemData itemData, out int x, out int y) {
        

        
    //     x = resX;
    //     y = resY;
    //     return isFreePosFound;
    // }

    /// <summary>
    /// freePlaces -- те свободные места, в которые можно добавить предметы в заданном количестве.
    /// Избыточных свободных мест не возвращается
    /// </summary>
    private bool CanAddToFreePlaces(ItemData itemData, int count, out List<Vector2Int> freePlaces) {
        freePlaces = new List<Vector2Int>();
        var itemStaticData = _itemStaticDataManager.GetStaticDataByName(itemData.ItemStaticDataName);
        int maxStackSize = itemStaticData.StackSize;
        int stacks = count / maxStackSize + (count % maxStackSize != 0 ? 1 : 0);

        // O(n * log(n))
        FillingMatrix fillingMatrix = GetFillingMatrix();
        for (int i = 0; i < stacks; i++) {
            // O(n^2)
            bool canAddItem = FindFreePos(fillingMatrix, itemData, out int x, out int y);
            if (canAddItem) {
                FillingMatrix.FillingRect itemRect = new FillingMatrix.FillingRect() {
                    width = itemStaticData.Width,
                    height = itemStaticData.Height,
                    x = x,
                    y = y
                };
                fillingMatrix.SetRect(itemRect, true);
                freePlaces.Add(new Vector2Int(x, y));
            } else
                // Если хотя бы для одного стака предмета нет свободной позиции
                return false;
        }
        return true;
    }
    
    /// <summary>
    /// true, если все предметы в заданном количестве возможно добавить в секцию инвентаря 
    /// </summary>
    public bool CanAddToSection(ItemData itemData, int count) {
        int toAddToFreePlace = FindUnfilledItemStacksForCount(itemData, count, out _);
        if (toAddToFreePlace <= 0)
            return true;
        
        return CanAddToFreePlaces(itemData, toAddToFreePlace, out _);
    }
    #endregion

    #region Add And Remove
    private FillingMatrix.FillingRect GetRectForItem(GridSectionItem gridItem) {
        ItemStaticData staticData = _itemStaticDataManager
            .GetStaticDataByName(gridItem.itemData.ItemStaticDataName);

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

    /// <summary>
    /// Пробует добавить предмет в заданном количестве. Либо добавляются все предметы,
    /// либо ни один
    /// </summary>
    public bool TryToAddToSection(ItemData itemData, int count = 1) {
        int canNotAddToStacks = FindUnfilledItemStacksForCount(itemData, count, out var unfilled);
        if (canNotAddToStacks <= 0) {
            TryToAddToUnfilledItemStacks(itemData, count);
            return true;
        }

        bool canAddToFreePlaces = CanAddToFreePlaces(itemData,
            canNotAddToStacks, out var freePlaces);
        // To optimize: CanAddToFreePlaces выполняется дополнительно в TryToAddToFreePlaces
        // (также и FindUnfilledItemStacksForCount в TryToAddToUnfilledItemStacks).
        // Можно передавать в вызовы некоторые уже вычисленные данные, а не вычислять заново
        if (!canAddToFreePlaces)
            return false;
        
        // Если каждый из этапов возможен, "транзакция" выполняется
        TryToAddToUnfilledItemStacks(itemData, count);
        TryToAddToFreePlaces(itemData, canNotAddToStacks);
        return true;
    }

    /// <summary>
    /// Возвращает число предметов, которые не были добавлены. Если оно 0 или отриц., то все предметы были
    /// добавлены
    /// </summary>
    private int TryToAddToUnfilledItemStacks(ItemData itemData, int count) {
        int notAdded = FindUnfilledItemStacksForCount(itemData, count, out List<GridSectionItem> unfilled);
        if (notAdded >= count)
            return notAdded;
        
        // Удаляем все незаполненные стаки, чтобы добавить их в уже заполненном виде
        foreach (GridSectionItem unfilledStack in unfilled) {
            RemoveFromSection(unfilledStack);
        }

        int countToAdd = count;
        int maxStackSize = _itemStaticDataManager
            .GetStaticDataByName(itemData.ItemStaticDataName).StackSize;
        foreach (GridSectionItem unfilledToFill in unfilled) {
            // Сколько осталось до максимально возможного размера стака?
            int canAddToStack = maxStackSize - unfilledToFill.Count;
            // Сколько можно фактически добавить (учитывая, сколько еще есть предметов)?
            int addToStack = countToAdd < canAddToStack ? countToAdd : canAddToStack;
            unfilledToFill.count += addToStack;
            countToAdd -= addToStack;

            if (isServer) {
                AddItem(unfilledToFill);
            } else {
                CmdAddItem(unfilledToFill);
            }
        }

        return notAdded;
    }

    /// <summary>
    /// Находит свободные места в инвентаре и добавляет туда предмет в заданном количестве
    /// </summary>
    private bool TryToAddToFreePlaces(ItemData itemData, int count) {
        bool canAdd = CanAddToFreePlaces(itemData, count, out var freePlaces);
        if (!canAdd)
            return false;
        
        int maxStackSize = _itemStaticDataManager.GetStaticDataByName(itemData.ItemStaticDataName).StackSize;
        int countToAdd = count;
        foreach (Vector2Int freePlace in freePlaces) {
            // Добавляется либо полный новый стак, либо та часть, которая осталась
            int countInNewStack = countToAdd >= maxStackSize ? maxStackSize : countToAdd;
            GridSectionItem gridItem = new GridSectionItem(netId) {
                itemData = itemData,
                count = countInNewStack,
                inventoryX = freePlace.x,
                inventoryY = freePlace.y
            };

            if (isServer) {
                AddItem(gridItem);
            } else {
                CmdAddItem(gridItem);
            }
            countToAdd -= countInNewStack;
        }
        
        return true;
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
        // Debug.Log("Remove from section - has authority: " + this.hasAuthority);
        bool hasItem = Items.IndexOf(invItem) != -1;
        if (!hasItem)
            return false;
        
        if (isServer) {
            // Debug.Log("Remove Item");
            RemoveItem(invItem);
        } else {
            // Debug.Log("Cmd Remove Item");
            CmdRemoveItem(invItem);
        }
        return true;
    }
    #endregion

    #region Test
    public bool TryToAddTestItems() {
        LifecycleEffectItemData newItem = new LifecycleEffectItemData() {
            ItemStaticDataName = "FirstAidKit",
            Uses = 8
        };
        bool areFirstAidKitsAdded = TryToAddToSection(newItem, 31);
        ItemData armor = new ItemData() {
            ItemStaticDataName = "TestArmor"
        };
        return areFirstAidKitsAdded && TryToAddToSection(armor, 3);
    }
    #endregion

    #region IItemsProvider
    public IInventoryItem PeekNext()
    {
        if (Items.Count == 0)
            return null;
        else
            return Items[Items.Count - 1];
    }

    public IInventoryItem RemoveLastPeekedItem()
    {
        IInventoryItem itemData = PeekNext();
        if (itemData == null)
            return null;
        
        RemoveFromSection((GridSectionItem)itemData);
        return itemData;
    }
    #endregion
}
