using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Компонент, обеспечивающий взаимодействие с предметами, т.е. их сбор в инвентарь
/// </summary>
public class ItemInteractorStrategy : NetworkBehaviour, IInteractorStrategy
{
    public event Action<Item> LookedAtItem;
    public event Action LookedAwayFromItem;

    // Можно установить что-то стороннее в качестве инвентаря, например, рюкзак
    // [SerializeField]
    // private WearSection _wearSectionToPick;
    // [SerializeField]
    // private GridSection _sectionToPick;
    
    [SerializeField]
    private CharactersInventory _characterInventory;

    private ItemStaticDataManager _itemStaticDataManager;

    private void Awake() {
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }

    public bool CanInteract(Collider col)
    {
        return col.GetComponent<Item>() is not null;
    }

    public void InteractObject(Collider col) {
        Item item = col.GetComponent<Item>();
        ItemData itemData = item.ItemData;

        if (_characterInventory.TryToAdd(itemData, 1)) {
            if (isServer) {
                NetworkServer.Destroy(item.gameObject);
            } else {
                CmdDestroy(item.netId);
            }
        } else {
            Debug.Log("Не удалось поместить поднятый предмет в инвентарь");
        }
    }

    [Command]
    private void CmdDestroy(uint netId) {
        NetworkServer.Destroy(NetworkServer.spawned[netId].gameObject);
    }

    public void LookedAtObject(Collider col)
    {
        LookedAtItem?.Invoke(col.GetComponent<Item>());
    }

    public void LookedAwayFromObject() {
        LookedAwayFromItem?.Invoke();
    }
}
