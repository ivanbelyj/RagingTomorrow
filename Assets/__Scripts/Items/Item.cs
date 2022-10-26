using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Компонент, присоединяемый к экземпляру (или префабу) предмета, который может храниться
/// в инвентаре
/// </summary>
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Item : NetworkBehaviour
{
    [SerializeField]
    private ItemData _itemData;

    /// <summary>
    /// Ссылка на ItemData (ScriptableObject), содержащий информацию о шаблоне предмета,
    /// к которому относится данный конкретный экземпляр.
    /// </summary>
    public virtual ItemData ItemData {
        get => _itemData;
        set {
            _itemData = value;
            SetMass();
        }
    }

    private void Start() {
        // Например, _itemData получен на старте из сериализованных данных
        if (_itemData is not null)
            SetMass();
    }

    private void SetMass() {
        GetComponent<Rigidbody>().mass = _itemData.Mass;
    }
}
