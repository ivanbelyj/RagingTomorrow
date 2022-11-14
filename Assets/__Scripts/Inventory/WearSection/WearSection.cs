using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public class WearSection : NetworkBehaviour, ITotalWeight
{
    private readonly SyncDictionary<WearSlot, ItemData> _syncSlots
        = new SyncDictionary<WearSlot, ItemData>();

    protected ItemStaticDataManager _itemStaticDataManager;

    public event SyncDictionary<WearSlot, ItemData>.SyncDictionaryChanged InventoryChanged {
        add {
            _syncSlots.Callback += value;
        }
        remove {
            _syncSlots.Callback -= value;
        }
    }

    private Dictionary<WearSlot, ItemData> _slots;
    public Dictionary<WearSlot, ItemData> Slots => _slots;

    public ItemStaticData GetItemData(string itemDataName)
    {
        return _itemStaticDataManager.NamesAndData[itemDataName];
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // При старте в _syncSlots уже могут быть элементы
        _slots = new Dictionary<WearSlot, ItemData>();
        foreach (var pair in _syncSlots)
        {
            _slots.Add(pair.Key, pair.Value);
        }
    }

    private void Awake() {
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
        // if (_limitations is null)
        //     _limitations = new List<ISectionLimitation>();
        // foreach (var limitation in _limitations) {
        //     limitation.Initialize(_itemStaticDataManager);
        // }
    }

    private void Start()
    {
        _syncSlots.Callback += SyncItems;
    }

    public float GetTotalWeight()
        => _slots.Sum(pair => GetItemData(pair.Value.itemStaticDataName).Mass);

    #region Sync
    
    [Server]
    private void AddItem(WearSlot slot, ItemData itemData) {
        _syncSlots.Add(slot, itemData);
    }

    [Server]
    private void RemoveItem(WearSlot slot) {
        _syncSlots.Remove(slot);
    }

    [Command]
    private void CmdAddItem(WearSlot slot, ItemData itemData) {
        AddItem(slot, itemData);
    }

    [Command]
    private void CmdRemoveItem(WearSlot slot) {
        RemoveItem(slot);
    }

    private void SyncItems(SyncDictionary<WearSlot, ItemData>.Operation op,
        WearSlot key, ItemData item) {
        switch (op) {
            case SyncDictionary<WearSlot, ItemData>.Operation.OP_ADD:
            {
                _slots.Add(key, item);
                break;
            }
            case SyncDictionary<WearSlot, ItemData>.Operation.OP_REMOVE:
            {
                _slots.Remove(key);
                break;
            }
        }
    }
    #endregion

    #region Add And Remove
    public bool AddToAccordingSlot(ItemData itemData) {
        WearSlot slot = GetSlotForItem(itemData);
        return AddToSection(slot, itemData);
    }

    private WearSlot GetSlotForItem(ItemData itemData) {
        // Todo: к какому слоту корректно отнести предмет?
        return WearSlot.Head;
    }

    // Например, автомат нельзя положить в слот для шлема
    private bool IsItemAllowed(WearSlot slot, ItemData item) {
        // Todo: корректно ли относить предмет к данному слоту?
        return false;
    }

    public bool AddToSection(WearSlot slot, ItemData item) {
        if (!IsItemAllowed(slot, item)) {
            return false;
        }

        // В занятый слот нельзя добавить предмет
        bool isSlotFree = !_slots.ContainsKey(slot);
        if (!isSlotFree) {
            return false;
        }

        if (isServer) {
            AddItem(slot, item);
        } else {
            CmdAddItem(slot, item);
        }

        return true;
    }

    public void RemoveFromSection(WearSlot slot) {
        if (isServer) {
            RemoveItem(slot);
        } else {
            CmdRemoveItem(slot);
        }
    }

    // For test
    public bool AddTestItems()
    {
        var newItem = new ItemData() {
            itemStaticDataName = "TestHelmet",
        };
        bool isAdded = AddToSection(WearSlot.Head, newItem);
        if (!isAdded) {
            // В инвентаре нет места для предметов данного размера
            return false;
        }
        return true;
    }
    #endregion
}
