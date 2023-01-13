using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Обеспечивает действия над предметом инвентаря, осуществляемые с помощью пользовательского ввода:
/// перемещение в другой инвентарь с помощью двойного клика
/// </summary>
[RequireComponent(typeof(IInventoryItemProvider))]
public class ItemUIActions : MonoBehaviour, IPointerClickHandler
{
    private WearSection _wearSection;
    private IInventory _currentInventory;
    private IInventory _otherInventory;
    private IInventoryItemProvider _itemDataProvider;
    private ItemUIActionType _doubleClickAction;

    private void Awake() {
        _itemDataProvider = GetComponent<IInventoryItemProvider>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2) {
            switch (_doubleClickAction) {
                case ItemUIActionType.MoveToOtherInventory:
                    MoveToOtherInventory();
                    break;
            }
        }
    }

    private void MoveToOtherInventory() {
        IInventoryItem item = _itemDataProvider.InventoryItem;
        if (_otherInventory.TryToAdd(item.ItemData, item.Count)) {
            _currentInventory.Remove(item); 
        }
        SwapInventories();
    }

    private void SwapInventories() {
        IInventory tmp = _otherInventory;
        _otherInventory = _currentInventory;
        _currentInventory = tmp;
    }

    private void SetWearSection(WearSection wearSection) {
        _wearSection = wearSection;
    }

    public void SetCurrentInventory(IInventory inv) {
        _currentInventory = inv;
    }

    public void SetOtherInventory(IInventory inv) {
        _otherInventory = inv;
    }

    public void SetActionByDoubleClick(ItemUIActionType actionType) {
        _doubleClickAction = actionType;
    }
}
