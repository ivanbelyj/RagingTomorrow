
using UnityEngine;

public interface IInventoryInfoProvider {
    public delegate void InfoChangedEventHandler(InventoryInfo newInventoryInfo);
    InventoryInfo InventoryInfo { get; }
    event InfoChangedEventHandler InventoryInfoChanged;
}