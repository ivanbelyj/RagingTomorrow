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

    private FillingMatrix _sectionFilling;
    [SerializeField]
    private int _initialWidth;
    [SerializeField]
    private int _initialHeight;
    public int Width => _sectionFilling.Width;
    public int Height => _sectionFilling.Height;

    public ItemStaticData GetItemData(string itemDataName)
    {
        return _itemStaticDataManager.NamesAndData[itemDataName];
    }

    private void Awake() {
        _syncItems.Callback += SyncItems;
        _sectionFilling = new FillingMatrix(_initialHeight, _initialWidth);
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // При старте в _syncItems уже могут быть элементы
        _items = new List<GridSectionItem>(_syncItems.Count);
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

    #region Filling Matrix Operations
    private void SetFillingRect(GridSectionItem gridItem, bool value) {
        ItemStaticData staticData = _itemStaticDataManager.GetStaticDataByName(
            gridItem.itemData.itemStaticDataName);
        
        _sectionFilling.SetRect(staticData.Width, staticData.Height,
            gridItem.inventoryX, gridItem.inventoryY, value);
    }

    private bool FindFreePos(ItemData item, out int x, out int y) {
        ItemStaticData staticData = _itemStaticDataManager.GetStaticDataByName(
            item.itemStaticDataName);
        
        bool hasFreePlace = _sectionFilling.FindFreeRectPos(
            staticData.Width, staticData.Height,
            out int resX, out int resY);
        x = resX;
        y = resY;

        return hasFreePlace;
    }
    #endregion

    #region Add And Remove
    public bool AddToFreePlace(ItemData itemData) {
        bool freePosIsFound = FindFreePos(itemData, out int x, out int y);
        if (!freePosIsFound)
            return false;
        
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

    // Todo: pass GridSectionItem
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
            bool isAdded = AddToFreePlace(newItem);
            if (!isAdded) {
                // В инвентаре нет места для предметов данного размера
                return false;
            }
        }
        return true;
    }
    #endregion
}
