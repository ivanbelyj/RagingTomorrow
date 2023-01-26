
using UnityEngine;

public interface IInventoryInfoProvider {
    public delegate void InventoryInfoChangedEventHandler(InventoryInfo newInventoryInfo);
    InventoryInfo InventoryInfo { get; }
    event InventoryInfoChangedEventHandler InventoryInfoChanged;
}