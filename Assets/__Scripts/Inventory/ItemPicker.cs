using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Компонент, позволяющий перемещать предметы между сценой и инвентарем с помощью
/// поднятия и выбрасывания
/// </summary>
[RequireComponent(typeof(Collider))]
public class ItemPicker : NetworkBehaviour
{
    public float throwAwayForce = 0.2f;

    // Можно установить что-то стороннее в качестве инвентаря, например, рюкзак
    [SerializeField]
    private Inventory _inventory;
    private Collider _collider;

    private void Awake() {
        _collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision col) {
        // Если столкнулись с GameObject, и это - предмет
        Item item = col.gameObject.GetComponent<Item>();
        if (item is not null) {
            if (isServer) {
                _inventory.AddItem(item.ItemData.name);
            } else {
                _inventory.CmdAddItem(item.ItemData.name);
            }
            NetworkServer.Destroy(item.gameObject);
            Debug.Log($"Item {item.ItemData.name}, {item.ItemData.ItemName} is picked up to inventory");
        }
    }

    public void ThrowAway(InventoryItem invItem) {
        if (isServer) {
            _inventory.RemoveItem(invItem);
        } else {
            _inventory.CmdRemoveItem(invItem);
        }
        
        // Debug.Log("Throwed item is " + invItem.itemDataName);

        ItemData itemData = _inventory.GetItemData(invItem.itemDataName);
        Debug.Log("ThrowAway: itemData - " + itemData.name +
            $". {itemData.ItemName}, {itemData.Description}");
        
        if (isServer) {
            SpawnAndThrowAway(itemData.name);
        } else {
            CmdSpawnAndThrowAway(itemData.name);
        }
    }

    // В Command (как и другие Remote actions) можно передавать не все типы данных
    [Command]
    public void CmdSpawnAndThrowAway(string itemDataName) {
        SpawnAndThrowAway(itemDataName);
    }

    [Server]
    public void SpawnAndThrowAway(string itemDataName) {
        ItemData itemData = _inventory.GetItemData(itemDataName);

        Debug.Log("ThrowAway: itemData - " + itemData.name +
            $". {itemData.ItemName}, {itemData.Description}, Item: {itemData.Item}");
        
        // Небольшой отступ, чтобы предмет не подбирался сразу после выбрасывания
        Vector3 offsetForNotToPick = transform.forward * (_collider.bounds.size.z * 1.5f) / 2;
        GameObject item = Instantiate(itemData.Item.gameObject,
            transform.position + offsetForNotToPick, Quaternion.identity);

        // Отправляем информацию о сетевом объекте всем игрокам
        NetworkServer.Spawn(item);
        
        // Бросок
        Rigidbody itemRb = item.GetComponent<Rigidbody>();
        itemRb.AddForce(transform.forward * throwAwayForce, ForceMode.Impulse);
    }
}
