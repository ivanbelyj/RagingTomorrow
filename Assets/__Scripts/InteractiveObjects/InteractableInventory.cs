using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Компонент интерактивного объекта-хранилища, имеющего инвентарь, в который персонажи могут
/// складывать предметы
/// </summary>
[RequireComponent(typeof(GridSection))]
public class InteractableInventory : MonoBehaviour, IInventoryInfoProvider, IGridSectionInventory
{
    [SerializeField]
    private InventoryInfo _inventoryInfo;
    public InventoryInfo InventoryInfo {
        get => _inventoryInfo;
    }

    public GridSection GridSection { get; private set; }

    public float TotalWeight => GridSection.TotalWeight;

    private void Awake() {
        GridSection = GetComponent<GridSection>();
    }

    public bool Remove(ItemPlacementId placementId)
    {
        if (placementId.InventorySectionNetId == GridSection.netId) {
            return GridSection.RemoveFromSection(placementId.LocalId);
        } else {
            Debug.Log("Предмет нельзя удалить из инвентаря, т.к. он находится в секции, " +
                "не относящейся к нему");
            return false;
        }
    }

    public bool TryToAdd(ItemData itemData, int count)
    {
        return GridSection.TryToAddToSection(itemData, count);
    }

    public bool CanAdd(ItemData itemData, int count)
    {
        return GridSection.CanAddToSection(itemData, count);
    }

#pragma warning disable CS0067
    public event IInventoryInfoProvider.InfoChangedEventHandler InventoryInfoChanged;

#pragma warning restore CS0067
}
