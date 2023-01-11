using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Данный компонент, реализующий абстрактную логику любого предмета, который можно подбирать
/// в инвентарь, нуждается в получении изначальной информации
/// о предмете в виде объекта типа, производного от ItemData.
/// Задаются эти объекты не в Item, а в других компонентах (обеспечивающих поведение
/// предмета соответствующего типа). В Item задание инициализирующей информации невозможно,
/// т.к. редактор в Unity не поддерживает установку параметров выбранного в инспекторе
/// производного типа (т.е., в редакторе устанавливать свойства можно только объектам конкретных типов)
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Item : NetworkBehaviour
{
    /// <summary>
    /// Изначальные данные о предмете для инициализации. Должны быть определены
    /// до спавна предмета на сервере.
    /// Устанавливаются с помощью специального метода класса Item
    /// </summary>
    private ItemData _initialItemData;

    [SyncVar(hook = nameof(OnItemDataChanged))]
    private ItemData _syncItemData;
    private ItemData _itemData;
    public ItemData ItemData => _itemData;

    private ItemStaticDataManager _itemStaticDataManager;

    private void Awake() {
        _itemStaticDataManager = FindObjectOfType<ItemStaticDataManager>();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        ChangeItemData(_initialItemData);
    }

    // Вызывается после OnStartServer()
    private void Start()
    {
        var staticData = _itemStaticDataManager.GetStaticDataByName(_itemData.ItemStaticDataName);

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.mass = staticData.Mass;
    }

    #region Sync
    private void OnItemDataChanged(ItemData oldData, ItemData newData) {
        _itemData = newData;
    }

    [Server]
    public void ChangeItemData(ItemData newData) {
        _syncItemData = newData;
    }

    [Command]
    public void CmdChangeItemData(ItemData newData) {
        ChangeItemData(newData);
    }
    #endregion

    public void Initialize(ItemData initialItemData) {
        _initialItemData = initialItemData;
    }
}
