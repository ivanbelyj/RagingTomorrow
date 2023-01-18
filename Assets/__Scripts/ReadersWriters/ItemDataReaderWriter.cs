using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public static class ItemDataReaderWriter
{
    private enum ItemType : int
    {
        None,
        LifecycleEffectItem,
        Armor,
        Weapon,
    }

    private static ItemType GetItemType(ItemData itemData) {
        if (itemData is LifecycleEffectItemData)
            return ItemType.LifecycleEffectItem;
        else
            return ItemType.None;
    }

    public static void WriteItemData(this NetworkWriter writer, ItemData itemData) {
        ItemType type = GetItemType(itemData);
        writer.WriteInt((int)type); // 1
        writer.WriteString(itemData.ItemStaticDataName); // 2
        switch (type) {
            case ItemType.Armor:
            break;
            case ItemType.LifecycleEffectItem:
            LifecycleEffectItemData lifecycleEffectItemData = (LifecycleEffectItemData)itemData;
            writer.WriteInt(lifecycleEffectItemData.Uses); // 3
            break;
            case ItemType.Weapon:
            break;
            default:
            break;
        }
    }
    public static ItemData ReadItemData(this NetworkReader reader) {
        ItemType itemType = (ItemType)reader.ReadInt();  // 1
        string itemStaticDataName = reader.ReadString();  // 2
        ItemData res = new ItemData();
        switch (itemType) {
            case ItemType.LifecycleEffectItem:
            var lifecycleEffectItemData = new LifecycleEffectItemData();
            lifecycleEffectItemData.Uses = reader.ReadInt();  // 3
            res = lifecycleEffectItemData;
            break;
            case ItemType.Armor:
            break;
            case ItemType.Weapon:
            break;
        }
        res.ItemStaticDataName = itemStaticDataName;
        return res;
    }
}
