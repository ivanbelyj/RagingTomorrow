using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компоненты, которые могут предоставлять IInventoryItem 
/// </summary>
public interface IInventoryItemProvider
{
    IInventoryItem InventoryItem { get; }
}
