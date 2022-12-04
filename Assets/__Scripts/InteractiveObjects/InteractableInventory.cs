using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компонент интерактивного объекта-хранилища, имеющего инвентарь, в который персонажи могут
/// складывать предметы
/// </summary>
[RequireComponent(typeof(GridSection))]
public class InteractableInventory : MonoBehaviour, IInventoryInfoProvider
{
    [SerializeField]
    private InventoryInfo _inventoryInfo;
    public InventoryInfo InventoryInfo {
        get => _inventoryInfo;
    }

    public GridSection Inventory { get; private set; }

    public event IInventoryInfoProvider.InfoChangedEventHandler InventoryInfoChanged;

    private void Awake() {
        Inventory = GetComponent<GridSection>();
    }
}
