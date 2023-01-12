using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICountableItem
{
    public int Count { get; }
    public ItemData ItemData { get; }
}
