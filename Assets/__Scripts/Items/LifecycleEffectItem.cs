using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifecycleEffectItem : Item
{
    private LifecycleEffect _effect;

    public override ItemData ItemData {
        get => base.ItemData;
        set {
            base.ItemData = value;
            _effect = ((LifecycleEffectItemData)ItemData).Effect;
        }
    }

    public bool IsUsed { get; private set; } = false;

    public void UseItem(Entity user) {
        if (IsUsed)
            return;
        
        user.AddEffect(_effect);
        IsUsed = true;
    }
}
