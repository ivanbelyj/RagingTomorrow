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
    private InventorySection _sectionToPick;
    private Collider _collider;

    private void Awake() {
        _collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision col) {
        // Если столкнулись с GameObject, и это - предмет
        Item item = col.gameObject.GetComponent<Item>();
        if (item is not null) {
            InventoryItem itemStack = new InventoryItem() {
                itemGameData = item.ItemGameData,
                count = 1
            };
            _sectionToPick.AddToSection(itemStack);
            NetworkServer.Destroy(item.gameObject);
            Debug.Log($"Item {item.ItemGameData.itemStaticDataName} is picked up to inventory");
        }
    }

    public void ThrowAway(InventorySection section, InventoryItem invItem) {
        section.RemoveFromSection(invItem);
        
        Debug.Log("ThrowAway: itemData - " + invItem.itemGameData.itemStaticDataName);
        
        if (isServer) {
            SpawnAndThrowAway(invItem.itemGameData);
        } else {
            CmdSpawnAndThrowAway(invItem.itemGameData);
        }
    }

    // В Command (как и другие Remote actions) можно передавать не все типы данных
    [Command]
    public void CmdSpawnAndThrowAway(ItemData itemGameData) {
        // ItemStaticData itemData = _inventory.GetItemData(itemStaticDataName);
        SpawnAndThrowAway(itemGameData);
    }

    [Server]
    public void SpawnAndThrowAway(ItemData itemGameData) {
        // Полная неизменная информация о предмете берется на основе названия,
        // которое используется для эффективной синхронизации инвентаря
        ItemStaticData itemStaticData = _sectionToPick.GetItemData(itemGameData.itemStaticDataName);

        Debug.Log("ThrowAway: itemData - " + itemStaticData.name +
            $". {itemStaticData.ItemName}, {itemStaticData.Description}, Item: {itemStaticData.ItemPrefab}");
        
        // Небольшой отступ, чтобы предмет не подбирался сразу после выбрасывания
        Vector3 offsetForNotToPick = transform.forward * (_collider.bounds.size.z * 1.5f) / 2;
        GameObject itemGO = Instantiate(itemStaticData.ItemPrefab,
            transform.position + offsetForNotToPick, Quaternion.identity);
        
        // Предметы должны быть инициализированы
        Item item = itemGO.GetComponent<Item>();
        item.Initialize(itemGameData, itemStaticData);

        // Отправляем информацию о сетевом объекте всем игрокам
        NetworkServer.Spawn(itemGO);
        
        // Бросок
        Rigidbody itemRb = itemGO.GetComponent<Rigidbody>();
        // Сила броска зависит от массы: и легкая аптечка, и тяжелый автомат слегка отбрасываются
        itemRb.AddForce(transform.forward * throwAwayForce * itemRb.mass, ForceMode.Impulse);
    }
}
