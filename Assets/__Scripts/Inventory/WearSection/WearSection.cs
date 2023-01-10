using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

/// <summary>
/// Секция инвентаря для предметов, в настоящий момент используемых персонажем.
/// Данная секция состоит из слотов, в каждом слоте может находиться максимум 1 предмет.
/// Todo:
/// В некоторых случаях предметы могут занимать сразу несколько слотов. Например, если надет
/// экзоскелет, то надеть бронежилет или шлем уже нельзя
/// </summary>
public class WearSection : NetworkBehaviour, ITotalWeight
{
    /// <summary>
    /// Для хранения информации о заполненности WearSection и обращения к хранимым предметам
    /// извне
    /// </summary>
    public enum WearSlot
    {
        None,  // Для преметов, которые нельзя поместить в какой-либо слот
        Head,
        Vest,
        Legs,
        Feet,
        Tail,
        HandGun,
        Rifle,
    }

    private readonly SyncDictionary<WearSlot, ItemData> _syncSlots
        = new SyncDictionary<WearSlot, ItemData>();

    // Каждый слот выполняет конкретную функцию и может быть использован кодом в процессе игры.
    // Например, в слоте оружия лежит объект определенного класса, поэтому логично
    // добавлять предметы не по WearSlot, который не привязан к классам, а по кастомным
    // признакам добавляемого предмета. С реализацией с WearSlot ничего не мешает добавить в
    // слот для оружия еду, у которой установлено WearSlot соотв. образом.
    // С другой стороны, WearSlot позволяет содержать словарь, который легче синхронизировать,
    // чем множество отдельных полей

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
        => _slots.Sum(pair => GetItemData(pair.Value.ItemStaticDataName).Mass);

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
        return AddToSection(GetAccordingSlot(itemData), itemData);
    }

    private WearSlot GetAccordingSlot(ItemData itemData) {
        var staticData = _itemStaticDataManager.GetStaticDataByName(itemData.ItemStaticDataName);
        if (staticData is WeaponItemStaticData weaponData) {
            if (weaponData.Type == WeaponItemStaticData.WeaponType.HandGun)
                return WearSlot.HandGun;
            else if (weaponData.Type == WeaponItemStaticData.WeaponType.Rifle)
                return WearSlot.Rifle;
        } else if (staticData is ArmorItemStaticData armorData) {
            return WearSlot.Vest;
        }
        return WearSlot.None;
    }

    // Например, автомат нельзя положить в слот для шлема или пистолета
    // private bool IsItemAllowed(WearSlot slot, ItemData item) {
    //     var staticData = _itemStaticDataManager.GetStaticDataByName(item.itemStaticDataName);
    //     if (staticData.AllowedWearSlot == WearSlot.None || slot == WearSlot.None) {
    //         return false;
    //     }
    //     return staticData.AllowedWearSlot == slot;
    // }

    private bool AddToSection(WearSlot slot, ItemData item) {
        // if (!IsItemAllowed(slot, item)) {
        //     return false;
        // }

        // В занятый слот нельзя добавить предмет
        bool isSlotFree = !_slots.ContainsKey(slot);
        if (!isSlotFree || slot == WearSlot.None) {
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
            ItemStaticDataName = "TestArmor",
        };
        bool isAdded = AddToAccordingSlot(newItem);
        if (!isAdded) {
            // В инвентаре нет места для предметов данного размера
            return false;
        }
        return true;
    }
    #endregion
}
