using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InventoryDragAndDrop : NetworkBehaviour
{
    private readonly SyncList<DraggedItemData> _syncDragged = new SyncList<DraggedItemData>();
    private List<DraggedItemData> _dragged;

    #region Sync
    [Server]
    private void AddDragged(DraggedItemData data) {
        _syncDragged.Add(data);
    }

    [Server]
    private void RemoveDragged(DraggedItemData data) {
        // Debug.Log("Remove on server");
        _syncDragged.Remove(data);
    }

    [Command]
    private void CmdAddDragged(DraggedItemData data) {
        // Debug.Log("From CmdAddItem: item is " + data);
        AddDragged(data);
    }

    [Command]
    private void CmdRemoveDragged(DraggedItemData data) {
        RemoveDragged(data);
    }

    private void SyncDragged(SyncList<DraggedItemData>.Operation op, int index,
        DraggedItemData oldItem, DraggedItemData newItem) {
        switch (op) {
            case SyncList<DraggedItemData>.Operation.OP_ADD:
            {
                _dragged.Add(newItem);
                break;
            }
            case SyncList<DraggedItemData>.Operation.OP_REMOVEAT:
            {
                _dragged.Remove(oldItem);
                break;
            }
        }
    }
    #endregion

    private void Awake() {
        _syncDragged.Callback += SyncDragged;
        _dragged = new List<DraggedItemData>();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // При старте на сервере уже могут быть элементы
        for (int i = 0; i < _syncDragged.Count; i++)
        {
            _dragged.Add(_syncDragged[i]);
        }
    }

    public void Drag(DraggedItemData data) {
        if (IsAlreadyDragged(data)) {
            // Значит, тот же самый предмет начал перетаскивать кто-то другой
            // и нужно сбросить предыдущее перетаскивание
            if (isServer) {
                RemoveDragged(data);
            } else {
                CmdRemoveDragged(data);
            }
            ResetDragAndDrop(data);
        }

        if (isServer) {
            AddDragged(data);
        } else {
            CmdAddDragged(data);
        }
    }

    private bool IsAlreadyDragged(DraggedItemData item) {
        return _dragged.Find(x => x.Equals(item)) is not null;
    }

    /// <summary>
    /// Сбрасывает перетаскиваемый предмет для перетаскивающего игрока
    /// </summary>
    private void ResetDragAndDrop(DraggedItemData item) {
        // Todo
    }
}
