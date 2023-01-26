using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

// Todo: Отображение текущего/макс. веса инвентаря персонажа (для других
// инвентарей, как правило, это не имеет смысла)
/// <summary>
/// UI, представляющий информацию инвентаря, его сеточную секцию и информацию об
/// общем весе
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private InventoryInfoUI _invInfoUI;

    [SerializeField]
    private InventoryGridUI _invGridUI;

    [Header("Опционально")]
    [SerializeField]
    private InventoryWeightBar _weightBar;

    private IGridSectionInventory _inventory;
    private IInventoryInfoProvider _infoProvider;

    public void SetAsPlayersInventory(IGridSectionInventory inventory, IInventoryInfoProvider invInfoProvider) {
        Set(inventory, invInfoProvider);
        _invGridUI.Set(inventory.GridSection, new SlotItemCreator(inventory, null));
    }

    public void SetAsOtherInventory(IGridSectionInventory inventory,
        IGridSectionInventory otherInventory, IInventoryInfoProvider invInfoProvider) {
        Set(otherInventory, invInfoProvider);
        _invGridUI.Set(otherInventory.GridSection, new SlotItemCreator(otherInventory, inventory));
    }

    private void Set(IGridSectionInventory inventory, IInventoryInfoProvider invInfoProvider) {
        // Если до установки инвентаря был старый, необходимо отвязаться от него
        GridSection oldInventory = _inventory?.GridSection;
        if (oldInventory is not null) {
            oldInventory.InventoryChanged -= OnInventoryChanged;
        }
        _inventory = inventory;
        _inventory.GridSection.InventoryChanged += OnInventoryChanged;
        // _invGridUI.SetInventorySection(_inventory.GridSection);
        
        // Привязаться к изменениям информации
        _infoProvider = invInfoProvider;
        IInventoryInfoProvider oldInvInfo = _infoProvider;
        if (oldInvInfo is not null) {
            oldInvInfo.InventoryInfoChanged -= OnInfoChanged;
        }
        _infoProvider.InventoryInfoChanged += OnInfoChanged;
        // Предполагается, что на данный момент данные уже определны и синхронизированы
        _invInfoUI.SetInfo(_infoProvider.InventoryInfo);
    }

    private void OnInfoChanged(InventoryInfo newInfo) {
        _invInfoUI.SetInfo(newInfo);
    }

    private void OnInventoryChanged(SyncList<GridSectionItem>.Operation op, int index,
        GridSectionItem oldItem, GridSectionItem newItem) {
        // Todo: максимальный вес
        if (_weightBar != null)
            _weightBar.UpdateWeightInfoText(_inventory.TotalWeight, 0);
    }

    // private float GetMaxWeight() {
    //     // Todo: сделать максимальный переносимый вес свойством живого существа,
    //     // а возможно, даже параметром жизненного цикла.
    //     // При его обновлении будет меняться и отображаемое значение. Событие изменения
    //     // параметра жизненного цикла может вызываться очень много раз, в таком случае нужно
    //     // не пересчитывать TotalWeight, а сохранить его в _weight, а обновлять _weight
    //     // по изменении инвентаря
    //     return -1;
    // }
}
