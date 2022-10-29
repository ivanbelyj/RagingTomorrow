using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour
{
    [SerializeField]
    public GridLayoutGroup _gridLayoutGroup;
    [SerializeField]
    private GameObject _slotPrefab;

    public int Cols {
        get => _gridLayoutGroup.constraintCount;
        set => _gridLayoutGroup.constraintCount = value;
    }

    [SerializeField]
    private int _rows;
    public int Rows {
        get => _rows;
        set => _rows = value;
    }

    private InventorySlot[,] _slots;

    private void Start() {
        SetSlots();
    }

    private void SetSlots() {
        _slots = new InventorySlot[Rows, Cols];
        for (int r = 0; r < Rows; r++) {
            for (int c = 0; c < Cols; c++) {
                SetSlot(r, c);
            }
        }
    }

    private void SetSlot(int row, int col) {
        GameObject slot = Instantiate(_slotPrefab);
        _slots[row, col] = slot.GetComponent<InventorySlot>();
        slot.transform.SetParent(_gridLayoutGroup.transform);
        slot.transform.localScale = Vector3.one;
    }
}
