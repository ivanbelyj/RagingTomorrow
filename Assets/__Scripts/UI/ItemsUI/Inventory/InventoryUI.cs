using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Компонент, управляющий UI инвентаря
/// </summary>
public class InventoryUI : MonoBehaviour
{
    #region UI
    [SerializeField]
    private InventoryInfoUI _invInfoUI;

    [SerializeField]
    private InventoryGridUI _invGridUI;

    [SerializeField]
    private InventoryWeightBar _weightBar;
    #endregion

    #region Sources
    private IItemsGrid _itemsGrid;
    private IInventoryInfoProvider _infoProvider;
    #endregion

    public void Set(IInventoryInfoProvider invInfoProvider, IItemsGrid itemsGrid) {
        _itemsGrid = itemsGrid;
        _invGridUI.SetGrid(itemsGrid);

        // Привязаться к изменениям инвентаря
        IItemsGrid oldInventory = _itemsGrid;
        // Если до установки инвентаря был старый, необходимо отвязаться от него
        if (oldInventory is not null) {
            oldInventory.GridChanged -= OnInventoryChanged;
        }
        _itemsGrid.GridChanged += OnInventoryChanged;
        
        // Привязаться к изменениям информации
        _infoProvider = invInfoProvider;
        IInventoryInfoProvider oldInvInfo = _infoProvider;
        if (oldInvInfo is not null) {
            oldInvInfo.InventoryInfoChanged -= OnInfoChanged;
        }
        _infoProvider.InventoryInfoChanged += OnInfoChanged;
        _invInfoUI.SetInfo(_infoProvider.InventoryInfo);
    }

    private void OnInfoChanged(InventoryInfo newInfo) {
        _invInfoUI.SetInfo(newInfo);
    }

    private void OnInventoryChanged(SyncList<GridItemData>.Operation op, int index,
        GridItemData oldItem, GridItemData newItem) {
        // To optimize: в случае чего можно кэшировать вес, а не считать заново

        // Todo: сделать максимальный переносимый вес свойством живого существа,
        // а возможно, даже параметром жизненного цикла.
        // При его обновлении будет меняться и отображаемое значение. Событие изменения
        // параметра жизненного цикла может вызываться очень много раз, в таком случае нужно
        // не пересчитывать TotalWeight, а сохранить его в _weight, а обновлять _weight
        // по изменении инвентаря
        float maxWeight = 40;
        _weightBar.UpdateWeightInfoText(_itemsGrid.TotalWeight, maxWeight);
    }
}
