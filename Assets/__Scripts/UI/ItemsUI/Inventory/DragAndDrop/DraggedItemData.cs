using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggedItemData
{
    /// <summary>
    /// Net Id секции инвентаря, в которой находится предмет до перетаскивания
    /// </summary>
    // public uint InventorySectionNetId { get; set; }

    /// <summary>
    /// Id предмета, уникальный в пределах секции
    /// </summary>
    // public uint ItemLocalId { get; set; }

    /// <summary>
    /// Идентификатор размещения предмета в инвентаре
    /// </summary>
    public ItemPlacementId PlacementId { get; set; }

    /// <summary>
    /// Net Id игрока, который перетаскивает предмет
    /// </summary>
    public uint DraggingPlayerNetId { get; set; }

    /// <summary>
    /// Иконка предмета может перетаскиваться не только за левый верхний угол, в таком случае
    /// при добавлении в новое выбранное пользователем место необходимо учитывать
    /// отступ мыши в плитках относительно верхней левой части предмета
    /// </summary>
    public int MouseSlotsOffsetX { get; set; }
    public int MouseSlotsOffsetY { get; set; }
}

