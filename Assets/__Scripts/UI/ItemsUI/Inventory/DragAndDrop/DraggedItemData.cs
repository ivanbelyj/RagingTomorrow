using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggedItemData
{
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

