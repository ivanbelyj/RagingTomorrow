using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GridSectionItem : IEquatable<GridSectionItem>
{
    public ItemData itemData;
    public int inventoryX;
    public int inventoryY;
    public int count;

    public bool Equals(GridSectionItem other)
    {
        return (this.itemData.Equals(other.itemData) &&
            this.inventoryX == other.inventoryX &&
            this.inventoryY == other.inventoryY);
    }

    public override string ToString()
    {
        return $"GridSectionItem: {itemData.itemStaticDataName}; ({inventoryX}, {inventoryY}); " +
            $"count: {count}";
    }
}
