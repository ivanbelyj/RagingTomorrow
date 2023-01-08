using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Несмотря на то, что для получения базовой информации о предмете достаточно имени предмета
/// в Asset bundle, параметры некоторых предметов в процессе игры могут измениться. Эти параметры,
/// помимо имени, также нужно синхронизировать.
/// Например, патроны, оставшиеся заряженными в автомате, показатель поношенности брони, и т.п.
/// </summary>
[Serializable]
public abstract class ItemDynamicData
{
    [HideInInspector]
    public DynamicItemType type;
    public abstract override bool Equals(object obj);
    public abstract override int GetHashCode();
}
