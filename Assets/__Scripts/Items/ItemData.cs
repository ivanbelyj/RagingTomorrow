using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// В инвентаре синхронизируются не просто имена, а объекты, содержащие как имя (позволяет
/// получить неизменные характеристики, которые неэффективно постоянно передавать по сети),
/// так и динамическую информацию
/// </summary>
[Serializable]
public class ItemData
{
    public string itemStaticDataName;
    public ItemDynamicData dynamicData;
}
