using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggedItemData
{
    /// <summary>
    /// Net Id секции инвентаря, в которой находится предмет до перетаскивания
    /// </summary>
    public uint InventorySectionNetId { get; set; }

    /// <summary>
    /// Id предмета, уникальный в пределах секции
    /// </summary>
    public uint ItemLocalId { get; set; }

    /// <summary>
    /// Net Id игрока, который перетаскивает предмет
    /// </summary>
    public uint DraggingPlayerNetId { get; set; }
}

