using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

// Todo: вместо IItemsGrid использовать GridInventorySection
public interface IItemsGrid
{
    // Todo: если оставлять этот интерфейс, то нужен собственный enum
    public delegate void GridChangedEventHandler(SyncList<GridItemData>.Operation op, int index,
        GridItemData oldItem, GridItemData newItem);
    int Width { get; }
    int Height { get; }
    event GridChangedEventHandler GridChanged;
    IEnumerable<GridItemData> GridItems { get; }

    // Todo: если не убирать этот интерфейс, то это свойство не совсем корректно
    float TotalWeight { get; }
}
