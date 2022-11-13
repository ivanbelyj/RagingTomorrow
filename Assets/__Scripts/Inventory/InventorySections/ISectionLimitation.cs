using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISectionLimitation
{
    void Initialize(ItemStaticDataManager itemStaticDataManager);
    bool IsAllowedToAdd(InventoryItem itemStack);
    void AcceptToSection(InventoryItem itemStack);
    void RemoveFromSection(Guid itemId);
}
