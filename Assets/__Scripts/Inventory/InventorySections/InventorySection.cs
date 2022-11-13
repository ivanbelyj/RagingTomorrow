using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Инвентарь представляется некоторым количеством секций. Например, для персонажа это секция
/// рюкзака, секция надетой экипировки и выбранного оружия,
/// секции для ингредиентов и для результатов крафта.
/// Секции инвентаря логически обобщают некоторые элементы инвентаря, а также накладывают 
/// свои ограничения на предметы, которые могут быть добавлены
/// </summary>
public class InventorySection : NetworkBehaviour
{
    // Для удобства в инспекторе
    [SerializeField]
    private string _sectionName;

    /// <summary>
    /// Ограничения, накладываемые на объекты, которые могут содержаться в секции
    /// </summary>
    [SerializeField]
    protected List<ISectionLimitation> _limitations;

    private readonly SyncList<InventoryItem> _syncItems = new SyncList<InventoryItem>();
    
    private List<InventoryItem> _items;

    protected ItemStaticDataManager _itemStaticDataManager;

    public event SyncList<InventoryItem>.SyncListChanged InventoryChanged {
        add {
            _syncItems.Callback += value;
        }
        remove {
            _syncItems.Callback -= value;
        }
    }

    public float TotalWeight
        => _items.Sum(item => GetItemData(item.itemGameData.itemStaticDataName).Mass);
    public List<InventoryItem> Items => _items;

    public ItemStaticData GetItemData(string itemDataName)
    {
        return _itemStaticDataManager.NamesAndData[itemDataName];
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // При старте в _syncItems уже могут быть элементы
        _items = new List<InventoryItem>(_syncItems.Count);
        for (int i = 0; i < _syncItems.Count; i++)
        {
            // AddItemOnClient(_syncItems[i]);
            _items.Add(_syncItems[i]);
        }
    }

    protected virtual void Awake() {
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
        if (_limitations is null)
            _limitations = new List<ISectionLimitation>();
        foreach (var limitation in _limitations) {
            limitation.Initialize(_itemStaticDataManager);
        }
    }

    private void Start()
    {
        _syncItems.Callback += SyncItems;
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }

    #region Sync
    
    [Server]
    private void AddItem(InventoryItem item) {
        _syncItems.Add(item);
    }

    [Server]
    private void RemoveItem(InventoryItem item) {
        _syncItems.Remove(item);
    }

    [Command]
    private void CmdAddItem(InventoryItem item) {
        AddItem(item);
    }

    [Command]
    private void CmdRemoveItem(InventoryItem item) {
        RemoveItem(item);
    }

    private void SyncItems(SyncList<InventoryItem>.Operation op, int index,
        InventoryItem oldItem, InventoryItem newItem) {
        switch (op) {
            case SyncList<InventoryItem>.Operation.OP_ADD:
            {
                // AddItemOnClient(newItem);
                _items.Add(newItem);
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
                // RemoveItemOnClient(oldItem);
                _items.Remove(oldItem);
                break;
            }
            case SyncList<InventoryItem>.Operation.OP_SET:
            {

                break;
            }
        }
    }
    #endregion

    /// <summary>
    /// Перед тем, как элемент будет добавлен в секцию, он может быть изменен в соответствии с
    /// требованиями секции. Например, при добавлении в сетку элемент может быть преобразован
    /// в производный, содержащий координаты в сетке
    /// </summary>
    // protected abstract ItemStack ItemToSection(ItemStack itemStack);

    // public abstract bool IsAllowedToAdd(ItemStack itemGameData);

    // protected virtual void OnItemAdded(ItemStack invItem) { }
    // protected virtual void OnItemRemoved(ItemStack invItem) { }

    /// <summary>
    /// Перед добавлением в секцию инвентаря предмет представлен обычным ItemStack,
    /// который не привязан к специфике какой-то конкретной секции
    /// </summary>
    public bool AddToSection(InventoryItem itemStack) {
        Debug.Log("Попытка добавления в секцию инвентаря");
        foreach (var limitation in _limitations) {
            Debug.Log("\tПроверка ограничения");
            if (!limitation.IsAllowedToAdd(itemStack)) {
                Debug.Log("\tОграничение не пройдено");
                return false;
            }
        }

        // Todo: Синхронизировано ли?
        foreach (var limitation in _limitations) {
            limitation.AcceptToSection(itemStack);
        }
        
        // ItemStack invItem = ItemToSection(itemStack);
        
        if (isServer) {
            AddItem(itemStack);
        } else {
            CmdAddItem(itemStack);
        }
        // OnItemAdded(invItem);

        return true;
    }

    public void RemoveFromSection(InventoryItem invItem) {
        // Todo: синхронизировано ли?
        foreach (var limitation in _limitations) {
            limitation.RemoveFromSection(invItem.id);
        }

        // TItem invItem = ToInventoryItem(itemGameData);
        if (isServer) {
            RemoveItem(invItem);
        } else {
            CmdRemoveItem(invItem);
        }
        // OnItemRemoved(invItem);
    }

    // public abstract bool IsAllowedToAdd(TItem invItem);

    // For test
    public bool AddTestItems()
    {
        for (int n = 0; n < 10; n++) {
            InventoryItem itemStack = new InventoryItem() {
                itemGameData = new ItemData() {
                    itemStaticDataName = _itemStaticDataManager.NamesAndData.ElementAt(0).Key
                },
                count = 1
            };
            bool isAdded = AddToSection(itemStack);
            if (!isAdded) {
                // В инвентаре нет места для предметов данного размера
                return false;
            }
        }
        return true;
    }
}
