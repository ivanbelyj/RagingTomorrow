using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Непосредственно в Item данные для инициализации нелья установить в инспекторе,
/// т.к. типы объектов, представляющих эти данные, различаются.
/// </summary>
[RequireComponent(typeof(Item))]
public abstract class ItemInitializer<TItemData> : MonoBehaviour
    where TItemData : ItemData
{
    [SerializeField]
    private TItemData _initialItemData;
    public TItemData InitialItemData { get => _initialItemData; }
    protected virtual void Awake() {
        Debug.Log("ItemInitializer.Awake; " + _initialItemData);
        GetComponent<Item>().Initialize(_initialItemData);
    }
}
