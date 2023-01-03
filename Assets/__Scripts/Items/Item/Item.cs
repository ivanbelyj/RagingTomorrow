using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Компонент, присоединяемый к GameObject, представляющему предмет.
/// </summary>
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Item : NetworkBehaviour
{
    [Header("Set in inspector")]
    /// <summary>
    /// Изначальные данные о предмете для инициализации. Должны быть определены до спавна
    /// предмета на сервере.
    /// Устанавливаются в инспекторе, либо с помощью специального метода класса Item
    [SerializeField]
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
        if (_initialItemData is not null) {
            _itemData = _initialItemData;
            Debug.Log("Initializing Item on scene. name: " + _itemData.itemStaticDataName);

            var staticData = _itemStaticDataManager.GetStaticDataByName(_itemData.itemStaticDataName);

            Rigidbody rb = GetComponent<Rigidbody>();
            Debug.Log($"Static data of item {staticData.ItemName}: mass: {staticData.Mass}." +
                $"Rigidbody: {rb}");

            rb.mass = staticData.Mass;
            ChangeItemData(_itemData);
        }
    }

    /// <summary>
    /// Ссылка на ItemStaticData (ScriptableObject), содержащий информацию о шаблоне предмета,
    /// к которому относится данный конкретный экземпляр.
    /// При установке также обновляется параметр mass в RigidBody.
    /// Устанавливается с помощью ItemStaticDataManager при обращении к данному свойству
    /// </summary>
    // public virtual ItemStaticData StaticData {
    //     get => _staticData;

    //     set {
    //         _staticData = value;
    //         SetMass();
    //     }
    // }

    // Todo: itemStaticData устанавливать не в инспекторе, а получать c помощью
    // ItemStaticDataManager в Item

    // private void Start() {
    //     // Например, _itemData получен на старте из сериализованных данных
    //     if (_staticData is not null)
    //         SetMass();
    // }

    // private void SetMass() {
    //     GetComponent<Rigidbody>().mass = _staticData.Mass;
    // }

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


    /// <summary>
    /// Хотя взаимодействовать с предметом можно, взаимодействие сводится к поднятию предмета,
    /// а оно обеспечивается на стороне того, кто вызывает Interact(), поэтому реализация
    /// данного метода не выполняет никаких действий
    /// </summary>
    // public void Interact()
    // {
        
    // }
}
