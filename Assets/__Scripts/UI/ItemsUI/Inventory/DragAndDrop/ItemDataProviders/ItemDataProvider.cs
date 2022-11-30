using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

// public abstract class ItemDataProvider : IDraggedDataProvider<DraggedItemData>
// {
//     private NetworkIdentity _playersIdentity;
//     private NetworkIdentity _invSectionIdentity;
//     private uint _itemLocalId;

//     public ItemDataProvider(NetworkIdentity playersIdentity, NetworkIdentity invSectionIdentity,
//         uint itemLocalId) {
//         _playersIdentity = playersIdentity;
//         _invSectionIdentity = invSectionIdentity;
//         _itemLocalId = itemLocalId;
//     }

//     public DraggedItemData GetDraggedData()
//     {
//         return new DraggedItemData() {
//             InventorySectionNetId = _invSectionIdentity.netId,
//             DraggingPlayerNetId = _playersIdentity.netId,
//             ItemLocalId = _itemLocalId
//         };
//     }
// }
