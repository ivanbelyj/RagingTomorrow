using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridSectionInventory : IInventory
{
    GridSection GridSection { get; }
}
