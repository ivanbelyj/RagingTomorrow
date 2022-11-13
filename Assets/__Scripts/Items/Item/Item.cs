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
    // [SerializeField]
    // private ItemStaticData _staticData;

    [Header("Set in inspector")]

    [SyncVar(hook = nameof(OnItemGameDataChanged))]
    private ItemData _syncItemGameData;
    private ItemData _itemGameData;
    public ItemData ItemGameData => _itemGameData;

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
    private void OnItemGameDataChanged(ItemData oldData, ItemData newData) {
        _itemGameData = newData;
    }

    [Server]
    public void ChangeItemGameData(ItemData newData) {
        _syncItemGameData = newData;
    }

    [Command]
    public void CmdChangeItemGameData(ItemData newData) {
        ChangeItemGameData(newData);
    }
    #endregion

    /// <summary>
    /// Инициализация на сервере перед спавном
    /// </summary>
    [Server]
    public void Initialize(ItemData itemGameData, ItemStaticData staticData) {
        Rigidbody rb = GetComponent<Rigidbody>();
        Debug.Log($"Static data of item {staticData.ItemName}: mass: {staticData.Mass}." +
            $"Rigidbody: {rb}");

        rb.mass = staticData.Mass;
        ChangeItemGameData(itemGameData);
    }
}
