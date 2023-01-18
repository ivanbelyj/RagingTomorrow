using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Позволяет создавать объекты интерактивных иконок предметов, основываясь на информации,
/// переданной сверху, но не получая доступ к ней
/// </summary>
public class SlotItemCreator
{
    private IInventory _currentInventory;
    private IInventory _otherInventory;

    /// <summary>
    /// При создании интерактивных иконок требуется знать открытый инвентарь (или два инвентаря),
    /// наличие этих данных у SlotItem обеспечивает возможность некоторых манипуляций с предметами,
    /// например, перемещение между секциями инвентаря. В то же время эти данные не относятся
    /// к компоненту, ответственному за формирование UI сеточного инвентаря
    /// </summary>
    public SlotItemCreator(IInventory currentInventory, IInventory otherInventory) {
        _currentInventory = currentInventory;
        _otherInventory = otherInventory;
    }

    public InventorySlotItem CreateItem(GridSectionItem invItem, GameObject slotItemPrefab,
        RectTransform gridParent, float slotSize) {
        int col = invItem.InventoryX;
        int row = invItem.InventoryY;
        // Debug.Log("Добавление элемента в сетку. Позиция: (" + col + ", " + row + ")");

        // Добавляем в сетку
        GameObject itemGO = Object.Instantiate(slotItemPrefab);
        itemGO.transform.SetParent(gridParent.transform);
        itemGO.transform.localScale = Vector3.one;
        itemGO.transform.localPosition = new Vector3(col * slotSize, -row * slotSize);

        InventorySlotItem slotItem = itemGO.GetComponent<InventorySlotItem>();
        slotItem.Initialize(invItem, slotSize, 0);

        // Интерактивная иконка работает как активатор всплывающей подсказки, а его необходимо
        // инициализировать. Для этого используется специфическая надстройка над системой
        // всплывающих подсказок, которая устанавливает родительский объект ItemsUI как родитель
        // для новых всплывающих подсказок. Эта логика не включена в TooltipActivator,
        // т.к. связана исключительно с ItemsUI
        itemGO.GetComponent<ItemsUITooltipActivatorInitializer>().Initialize();

        // Обеспечение таких манипуляций, как перемещение в другой инвентарь по двойному клику
        var itemUIActions = itemGO.GetComponent<ItemUIActions>();
        itemUIActions.SetCurrentInventory(_currentInventory);
        if (_otherInventory == null) {
            itemUIActions.SwitchToOneInventoryOpenedActions();
        } else {
            itemUIActions.SwitchToTwoInventoriesOpenedActions(_otherInventory);
        }

        return slotItem;

        // Debug.Log($"Set slot [{x}, {y}] with item {invItem.itemGameData.itemDataName}; "
        //     + $"_slots is {_slots.GetLength(0)}x{_slots.GetLength(1)}");
    }
}
