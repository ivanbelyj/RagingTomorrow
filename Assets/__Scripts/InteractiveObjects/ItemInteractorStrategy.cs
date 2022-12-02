using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Компонент, обеспечивающий взаимодействие с предметами, т.е. их сбор в инвентарь
/// </summary>
// Todo: вместо наследования от Interactor - агрегация нескольких обработчиков в Interactor
public class ItemInteractorStrategy : MonoBehaviour, IInteractorStrategy
{
    // Можно установить что-то стороннее в качестве инвентаря, например, рюкзак
    [SerializeField]
    private WearSection _wearSectionToPick;
    [SerializeField]
    private GridSection _sectionToPick;
    // Todo: character inventory

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

        if (!_sectionToPick.TryToAddToSection(itemData)) {
            Debug.Log("Не удалось поместить поднятый предмет в инвентарь");
        }

        // Todo:
        // if (!_wearSectionToPick.AddToAccordingSlot(itemData)) {
            
        // }

        // Todo: можно ли выполнять на клиенте?
        NetworkServer.Destroy(item.gameObject);
        Debug.Log($"Item {item.ItemData.itemStaticDataName} is picked up to inventory");
    }

    public void LookToObject(Collider col)
    {
        Debug.Log("Looking to item");
    }
}
