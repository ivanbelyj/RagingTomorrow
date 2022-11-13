using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridInventorySection : InventorySection, IItemsGrid
{
    public int Width { get => _gridLimitation.Width; }
    public int Height { get => _gridLimitation.Height; }
    public event IItemsGrid.GridChangedEventHandler GridChanged;
    public IEnumerable<GridItemData> GridItems { get => _gridLimitation.Items; }

    [SerializeField]
    private GridLimitation _gridLimitation;

    protected override void Awake()
    {
        base.Awake();
        _limitations.Add(_gridLimitation);
        _gridLimitation.Initialize(_itemStaticDataManager);
    }
}
