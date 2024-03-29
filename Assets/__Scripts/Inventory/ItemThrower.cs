using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class ItemThrower : NetworkBehaviour
{
    public float throwAwayForce = 0.2f;

    [SerializeField]
    private WearSection _wearSectionToPick;
    [SerializeField]
    private GridSection _sectionToPick;

    /// <summary>
    /// Бросок происходит с отступом от бросающего, Collider требуется для определения
    /// границ бросающего
    /// </summary>
    // [SerializeField]
    // private Collider _collider;
    private ItemStaticDataManager _itemStaticDataManager;

    private void Awake() {
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }

    public void ThrowAwayFirstIfExists() {
        if (_sectionToPick.Items.Count == 0)
            return;
        GridSectionItem first = _sectionToPick.Items.Values.First();
        ThrowAwayFromGridSection(first);
    }

    private void ThrowAwayFromWearSection(WearSection wearSection, ItemData itemData) {
        wearSection.RemoveFromSection(itemData);
        ThrowAway(itemData);
    }
    private void ThrowAwayFromGridSection(GridSectionItem gridItem) {
        // За раз удаляется весь стак
        _sectionToPick.RemoveFromSection(gridItem.PlacementId.LocalId);

        // Но каждый предмет из стака спавнится отдельно
        for (int i = 0; i < gridItem.Count; i++) {
            ThrowAway(gridItem.ItemData);
        }
    }

    private void ThrowAway(ItemData itemData) {
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
            itemGameData.ItemStaticDataName);

        // Debug.Log("ThrowAway: itemData - " + itemStaticData.name +
        //     $". {itemStaticData.ItemName}, {itemStaticData.Description}, Item: {itemStaticData.ItemPrefab}");
        
        Vector3 spawnOffset = Vector3.zero; // transform.forward * (_collider.bounds.size.z / 2);
        GameObject itemGO = Instantiate(itemStaticData.ItemPrefab,
            transform.position + spawnOffset, Quaternion.identity);
        
        // Предметы должны быть инициализированы
        Item item = itemGO.GetComponent<Item>();
        if (item is null)
            Debug.LogError("Инстанцируемый префаб предмета не имеет компонента Item");
        item.Initialize(itemGameData);

        // Отправляем информацию о сетевом объекте всем игрокам
        NetworkServer.Spawn(itemGO);
        
        // Бросок
        Rigidbody itemRb = itemGO.GetComponent<Rigidbody>();
        // Debug.Log($"Масса бросаемого предмета: {itemRb.mass}; сила броска: {throwAwayForce}");
        // Сила броска зависит от массы: и легкая аптечка, и тяжелый автомат слегка отбрасываются
        itemRb.AddForce(transform.forward * throwAwayForce * itemRb.mass, ForceMode.Impulse);
    }
}
