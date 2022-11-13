using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// С каждым элементом, на который наложено ограничение на размер (т.к. сетка ограничена),
/// хранится сопутствующая информация
/// </summary>
public class GridItemData
{
    public InventoryItem InventoryItem { get; set; }

    public int InventoryX { get; set; }
    public int InventoryY { get; set; }
}
