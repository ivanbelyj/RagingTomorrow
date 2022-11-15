using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

// Todo: отображение текущего/макс. веса инвентаря персонажа (для других инвентарей, как правило,
// это не имеет смысла)
/// <summary>
/// UI, представляющий информацию инвентаря, определенную сеточную секцию и информацию об
/// общем весе
/// </summary>
public class InventoryUI : MonoBehaviour
{
    #region UI
    [SerializeField]
    private InventoryInfoUI _invInfoUI;

    [SerializeField]
    private InventoryGridUI _invGridUI;

    // [SerializeField]
    // private InventoryWeightBar _weightBar;
    #endregion

    #region Sources
    private GridSection _gridSection;
    private IInventoryInfoProvider _infoProvider;
    // private ITotalWeight _totalWeight;
    #endregion

    public void Set(IInventoryInfoProvider invInfoProvider, GridSection gridSection) {
        // Если до установки инвентаря был старый, необходимо отвязаться от него
        GridSection oldInventory = _gridSection;
        if (oldInventory is not null) {
            oldInventory.InventoryChanged -= OnInventoryChanged;
        }
        _gridSection = gridSection;
        _gridSection.InventoryChanged += OnInventoryChanged;
        _invGridUI.SetInventorySection(_gridSection);
        
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

    private void OnInventoryChanged(SyncList<GridSectionItem>.Operation op, int index,
        GridSectionItem oldItem, GridSectionItem newItem) {
        // _weightBar.UpdateWeightInfoText(GetTotalWeight(), GetMaxWeight());
    }

    // private float GetTotalWeight() {
    //     // To optimize: можно кэшировать вес, а не считать заново
    //     // Todo: определение общего веса
    //     return -1;    
    // }

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
