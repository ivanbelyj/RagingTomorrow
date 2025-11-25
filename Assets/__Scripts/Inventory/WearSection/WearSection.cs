using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppearanceCustomization3D;
using Mirror;
using UnityEngine;

/// <summary>
/// Секция инвентаря для предметов, в настоящий момент используемых персонажем
/// </summary>
public class WearSection : NetworkBehaviour
{
    private readonly SyncList<ItemData> _syncItems = new SyncList<ItemData>();
    protected ItemStaticDataManager _itemStaticDataManager;

    public event Action<SyncList<ItemData>.Operation, int, ItemData, ItemData> InventoryChanged
    {
        add
        {
            _syncItems.Callback += value;
        }
        remove
        {
            _syncItems.Callback -= value;
        }
    }

    private List<ItemData> _items;
    public List<ItemData> Items => _items;

    [SerializeField]
    private CustomizableAppearance _appearance;

    public override void OnStartClient()
    {
        base.OnStartClient();

        // При старте в _syncSlots уже могут быть элементы
        _items = new List<ItemData>();
        foreach (ItemData itemData in _syncItems)
        {
            _items.Add(itemData);
        }
    }

    private void Awake()
    {
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }

    private void Start()
    {
        _syncItems.Callback += SyncItems;
    }

    public float TotalWeight => _items.Sum(itemData => _itemStaticDataManager
        .GetStaticDataByName(itemData.ItemStaticDataName).Mass);

    #region Sync
    [Server]
    private void AddItem(ItemData itemData)
    {
        _syncItems.Add(itemData);
    }

    [Server]
    private void RemoveItem(ItemData itemData)
    {
        _syncItems.Remove(itemData);
    }

    [Command]
    private void CmdAddItem(ItemData itemData)
    {
        AddItem(itemData);
    }

    [Command]
    private void CmdRemoveItem(ItemData itemData)
    {
        RemoveItem(itemData);
    }

    private void SyncItems(SyncList<ItemData>.Operation op, int index,
        ItemData oldItem, ItemData newItem)
    {
        switch (op)
        {
            case SyncList<ItemData>.Operation.OP_ADD:
                {
                    _appearance.ActivateNonStaticElementsAndDeactivateOccupied(
                        GetAppearanceItemStaticData(newItem).CharacterAppearanceElementsLocalIds
                    );
                    _items.Add(newItem);
                    break;
                }
            case SyncList<ItemData>.Operation.OP_REMOVEAT:
                {
                    _appearance.DeactivateNonStaticElements(
                        GetAppearanceItemStaticData(oldItem).CharacterAppearanceElementsLocalIds
                    );
                    _items.Remove(oldItem);
                    break;
                }
        }
    }
    #endregion

    private AppearanceItemStaticData GetAppearanceItemStaticData(ItemData itemData) =>
        (AppearanceItemStaticData)_itemStaticDataManager.GetStaticDataByName(itemData.ItemStaticDataName);

    #region Add And Remove

    /// <summary>
    /// true, если возможно надеть предмет, не снимая предыдущие, занимающие те же части тела
    /// <summary>
    public bool CanAdd(ItemData itemData)
    {
        return CanAdd(itemData, out _);
    }

    private List<AppearanceElement> GetOccupied(AppearanceItemStaticData appearanceItemStaticData)
        => _appearance.GetOccupied(appearanceItemStaticData.CharacterAppearanceElementsLocalIds);

    private bool CanAdd(ItemData itemData, out List<AppearanceElement> occupied)
    {
        if (_itemStaticDataManager.GetStaticDataByName(itemData.ItemStaticDataName)
            is AppearanceItemStaticData appearanceItemStaticData)
        {
            occupied = GetOccupied(appearanceItemStaticData);
            return occupied.Count == 0;
        }
        occupied = null;
        return false;
    }

    private bool TryToAddToSection(ItemData itemData)
    {
        bool canAdd = CanAdd(itemData);
        if (!canAdd)
        {
            return false;
        }

        if (isServer)
        {
            AddItem(itemData);
        }
        else
        {
            CmdAddItem(itemData);
        }

        return true;
    }

    public void RemoveFromSection(ItemData itemData)
    {
        if (isServer)
        {
            RemoveItem(itemData);
        }
        else
        {
            CmdRemoveItem(itemData);
        }
    }

    // Для теста
    public bool AddTestItems()
    {
        var newItem = new ItemData()
        {
            ItemStaticDataName = "TestArmor",
        };
        bool isAdded = TryToAddToSection(newItem);
        if (!isAdded)
        {
            Debug.Log("Не удалось надеть предмет " + newItem);
            return false;
        }
        return true;
    }
    #endregion
}
