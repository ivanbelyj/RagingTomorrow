using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockInventoryInfoProvider : IInventoryInfoProvider
{
    public InventoryInfo InventoryInfo { get; set; }

    public event IInventoryInfoProvider.InfoChangedEventHandler InventoryInfoChanged;
}
