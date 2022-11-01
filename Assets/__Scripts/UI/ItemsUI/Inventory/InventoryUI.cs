using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Компонент, управляющий UI инвентаря
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private InventoryInfoUI _invInfo;

    [SerializeField]
    private InventoryGrid _invGrid;

    [SerializeField]
    private InventoryWeightBar _weightBar;

    private Inventory _inventory;

    public void SetInventory(IInventoryInfo inventoryInfo, Inventory inventory) {
        Inventory oldInventory = _inventory;
        _inventory = inventory;
        // Если до установки инвентаря был старый, необходимо отвязаться от него
        if (oldInventory is not null) {
            oldInventory.InventoryChanged -= OnInventoryChanged;
        }
        
        _invInfo.SetInfo(inventoryInfo);
        _invGrid.SetInventory(inventory, inventory.DefaultSection);

        inventory.InventoryChanged += OnInventoryChanged;
    }

    private void OnInventoryChanged(SyncList<InventoryItem>.Operation op, int index,
        InventoryItem oldItem, InventoryItem newItem) {
        // To optimize: в случае чего можно кэшировать вес, а не считать заново

        // Todo: сделать максимальный переносимый вес свойством живого существа,
        // а возможно, даже параметром жизненного цикла.
        // При его обновлении будет меняться и отображаемое значение. Событие изменения
        // параметра жизненного цикла может вызываться очень много раз, в таком случае нужно
        // не пересчитывать TotalWeight, а сохранить его в _weight, а обновлять _weight
        // по изменению инвентаря
        float maxWeight = 40;
        _weightBar.UpdateWeightInfoText(_inventory.TotalWeight, maxWeight);
    }
}
