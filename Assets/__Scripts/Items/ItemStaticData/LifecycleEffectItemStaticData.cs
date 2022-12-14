using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LifecycleEffectItem",
    menuName = "Item Data/Lifecycle Effect Item Data",
    order = 51)]
public class LifecycleEffectItemStaticData : ItemStaticData
{
    [SerializeField]
    private LifecycleEffect _effect;

    public LifecycleEffect Effect => _effect;
}
