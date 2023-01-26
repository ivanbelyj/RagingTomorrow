using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public static class GridSectionItemReadWrite
{
    public static void WriteGridSectionItem(this NetworkWriter writer, GridSectionItem gridItem) {
        writer.WriteInt(gridItem.Count);
        writer.WriteInt(gridItem.InventoryX);
        writer.WriteInt(gridItem.InventoryY);
        writer.WriteUInt(gridItem.InventoryNetId);
        writer.Write<ItemData>(gridItem.ItemData);
    }
    public static GridSectionItem ReadGridSectionItem(this NetworkReader reader) {
        GridSectionItem gridItem = new GridSectionItem();
        gridItem.Count = reader.ReadInt();
        gridItem.InventoryX = reader.ReadInt();
        gridItem.InventoryY = reader.ReadInt();
        gridItem.InventoryNetId = reader.ReadUInt();
        gridItem.ItemData = reader.Read<ItemData>();
        return gridItem;
    }
}
