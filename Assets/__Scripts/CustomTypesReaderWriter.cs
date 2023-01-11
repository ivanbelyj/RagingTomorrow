using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public static class CustomTypesReaderWriter
{
    public static void WriteLifecycleEffect(this NetworkWriter writer,
        LifecycleEffect effect) {

        writer.WriteFloat(effect.duration);
        writer.WriteBool(effect.isInfinite);
        writer.WriteBool(effect.recoverToInitial);
        writer.WriteFloat(effect.speed);

        writer.WriteByte((byte)effect.targetParameter);
        // writer.WriteBool(effect.isActive);
        writer.WriteDouble(effect.startTime);
        writer.WriteUShort(effect.effectId);
        // writer.WriteInt(effect.UpdateIterations);
        // writer.WriteInt(effect.UpdateIterationsCurrent);
    }

    public static LifecycleEffect ReadLifecycleEffect(this NetworkReader reader) {
        
        float duration = reader.ReadFloat();
        bool isInfinite = reader.ReadBool();
        bool recoverToInitial =  reader.ReadBool();
        float speed = reader.ReadFloat();

        byte targetParameterIndex = reader.ReadByte();
        // effect.isActive = reader.ReadBool();
        double startTime = reader.ReadDouble();
        ushort effectId = reader.ReadUShort();

        LifecycleEffect effect = new LifecycleEffect() {
            speed = speed,
            isInfinite = isInfinite,
            recoverToInitial = recoverToInitial, 
            targetParameter = (EntityParameterEnum)targetParameterIndex,
            duration = duration,
            startTime = startTime,
            effectId = effectId
        };
        return effect;
    }

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
    
    // public static void WriteItemDynamicData(this NetworkWriter writer, ItemDynamicData dynamicData)
    // {
    //     DynamicItemType type;
    //     if (dynamicData is TestDynamicData) {
    //         type = DynamicItemType.Test;
    //     } else {
    //         type = DynamicItemType.None;
    //     }
    //     writer.WriteInt((int)type);

    //     switch (type) {
    //         case DynamicItemType.Test:
    //             var testData = (TestDynamicData)dynamicData;
    //             writer.WriteString(testData.bookNotes);
    //             break;
    //     }
    // }

    // public static ItemDynamicData ReadItemDynamicData(this NetworkReader reader) {
    //     switch ((DynamicItemType)reader.ReadInt()) {
    //         case DynamicItemType.Test:
    //             string bookNotes = reader.ReadString();
    //             return new TestDynamicData() {
    //                 bookNotes = bookNotes
    //             };
    //         default:
    //             return null;
    //     }
        
    // }
}
