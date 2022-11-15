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
    private WearSection _wearSectionToPick;
    [SerializeField]
    private GridSection _sectionToPick;
    private Collider _collider;

    private ItemStaticDataManager _itemStaticDataManager;

    private void Awake() {
        _collider = GetComponent<Collider>();
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }

    private void OnCollisionEnter(Collision col) {
        // Иначе предметы добавляются дублированно
        if (!isServer)
            return;
        
        // Если столкнулись с GameObject, и это - предмет
        Item item = col.gameObject.GetComponent<Item>();
        if (item is not null) {
            Debug.Log("Collision with item");

            ItemData itemData = item.ItemData;

            if (!_sectionToPick.TryToAddToSection(itemData)) {
                Debug.Log("Не удалось поместить поднятый предмет в инвентарь");
            }

            // Todo:
            // if (!_wearSectionToPick.AddToAccordingSlot(itemData)) {
                
            // }

            NetworkServer.Destroy(item.gameObject);
            Debug.Log($"Item {item.ItemData.itemStaticDataName} is picked up to inventory");
        }
    }

    public void ThrowAwayFromWearSection(WearSection wearSection, WearSlot slot) {
        // Todo:
        // wearSection.RemoveFromSection(slot);
        ThrowAway(wearSection.Slots[slot]);
    }
    public void ThrowAwayFromGridSection(/*GridSection gridSection,*/ GridSectionItem gridItem) {
        if (_sectionToPick.Items.Find(item => item.Equals(gridItem)) is null) {
            Debug.Log("Не удалось выбросить предмет, т.к. его нет в инвентаре");
            return;
        }
        _sectionToPick.RemoveFromSection(gridItem);
        ThrowAway(gridItem.itemData);
        
        // gridSection.RemoveFromSection(gridItem);
    }

    private void ThrowAway(ItemData itemData) {
        Debug.Log("ThrowAway: itemData - " + itemData.itemStaticDataName);
        
        if (isServer) {
            SpawnAndThrowAway(itemData);
        } else {
            CmdSpawnAndThrowAway(itemData);
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
        ItemStaticData itemStaticData = _itemStaticDataManager.GetStaticDataByName(
            itemGameData.itemStaticDataName);

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
        Debug.Log($"Масса бросаемого предмета: {itemRb.mass}; сила броска: {throwAwayForce}");
        // Сила броска зависит от массы: и легкая аптечка, и тяжелый автомат слегка отбрасываются
        itemRb.AddForce(transform.forward * throwAwayForce * itemRb.mass, ForceMode.Impulse);
    }
}
