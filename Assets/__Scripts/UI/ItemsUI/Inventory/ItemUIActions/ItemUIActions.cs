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
                case ItemUIActionType.MoveToWearSection:
                    Debug.Log("(Не реализовано) Добавление предмета в WearSection");
                    break;
                case ItemUIActionType.None:
                    Debug.Log("По нажатию на иконку предмета ничего не происходит");
                    break;
            }
        }
    }

    private void MoveToOtherInventory() {
        IInventoryItem item = _itemDataProvider.InventoryItem;
        Debug.Log($"{item.ItemData} отправляется в другой инвентарь");
        if (_otherInventory.TryToAdd(item.ItemData, item.Count)) {
            _currentInventory.Remove(item.PlacementId); 
        }
    }

    private void SetWearSection(WearSection wearSection) {
        _wearSection = wearSection;
    }

    /// <summary>
    /// Устанавливает инвентарь, в котором предмет расположен изначально
    /// </summary>
    public void SetCurrentInventory(IInventory inv) {
        _currentInventory = inv;
    }

    /// <summary>
    /// Заставляет компонент работать  в режиме одного открытого инвентаря с особой интерпретацией
    /// действий пользователя в UI: по двойному нажатию не происходит ничего
    /// </summary>
    public void SwitchToOneInventoryOpenedActions() {
        _otherInventory = null;

        // Todo: действие - переместить в WearSection
        _doubleClickAction = ItemUIActionType.MoveToWearSection;
    }

    /// <summary>
    /// Заставляет компонент работать в режиме двух открытых инвентарей. Действия пользователя в UI
    /// интерпретируются особым образом: по двойному нажатию предмет будет перемещаться
    /// в другой инвентарь.
    /// </summary>
    public void SwitchToTwoInventoriesOpenedActions(IInventory second) {
        _otherInventory = second;
        _doubleClickAction = ItemUIActionType.MoveToOtherInventory;
    }
}
