using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Компонент позволяет GameObject хранить набор предметов (Item)
/// </summary>
public class Inventory : NetworkBehaviour
{
    private readonly SyncList<InventoryItem> _syncItems = new SyncList<InventoryItem>();
    
    public List<InventoryItem> items;
    

    /// <summary>
    /// Ширина области инвентаря
    /// </summary>
    public int width;

    /// <summary>
    /// Высота области инвентаря
    /// </summary>
    public int height;

    private ItemDataManager _itemDataManager;

    public override void OnStartClient()
    {
        base.OnStartClient();

        _syncItems.Callback += SyncItems;

        // При старте в _syncItems уже могут быть элементы
        items = new List<InventoryItem>(_syncItems.Count);
        for (int i = 0; i < _syncItems.Count; i++)
        {
            _syncItems.Add(_syncItems[i]);
        }
    }

    private void Start()
    {
        _itemDataManager = FindObjectOfType<ItemDataManager>();
    }

    public void AddTestItems()
    {
        for (int n = 0; n < 20; n++) {
            InventoryItem invItem = new InventoryItem() {
                itemDataName = _itemDataManager.NamesAndData.ElementAt(0).Key
            };
            if (isServer) {
                AddItem(invItem);
            } else {
                CmdAddItem(invItem);
            }
        }
    }

    #region Sync
    [Server]
    public void AddItem(InventoryItem item) {
        _syncItems.Add(item);
    }

    [Server]
    public void AddItem(string itemDataName) {
        InventoryItem invItem = new InventoryItem() {
            itemDataName = itemDataName
        };
        _syncItems.Add(invItem);
    }

    [Server]
    public void RemoveItem(InventoryItem item) {
        _syncItems.Remove(item);
    }

    [Command]
    public void CmdAddItem(InventoryItem item) {
        AddItem(item);
    }

    [Command]
    public void CmdAddItem(string itemDataName) {
        AddItem(itemDataName);
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
                items.Add(newItem);
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
                items.Remove(oldItem);
                break;
            }
            case SyncList<InventoryItem>.Operation.OP_SET:
            {

                break;
            }
        }
    }
    #endregion

    public event SyncList<InventoryItem>.SyncListChanged OnInventoryChanged {
        add {
            _syncItems.Callback += value;
        }
        remove {
            _syncItems.Callback -= value;
        }
    }

    public float TotalWeight => items.Sum(item => GetItemData(item.itemDataName).Mass);

    public ItemData GetItemData(string itemDataName)
    {
        return _itemDataManager.NamesAndData[itemDataName];
    }
}
