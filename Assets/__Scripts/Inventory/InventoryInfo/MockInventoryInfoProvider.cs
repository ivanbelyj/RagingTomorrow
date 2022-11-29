using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockInventoryInfoProvider : IInventoryInfoProvider
{
    public InventoryInfo InventoryInfo { get; set; }

#pragma warning disable CS0067
    public event IInventoryInfoProvider.InfoChangedEventHandler InventoryInfoChanged;
#pragma warning restore CS0067
}
