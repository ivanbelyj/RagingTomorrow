using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// С предметами, с которыми произошло взаимодействие, можно поступить разными способами.
/// Поместить в инвентарь, отобразить всплывающую подсказку, и т.д.
/// </summary>
public interface IInteractorStrategy
{
    // TInteractObject GetInteractObject(Collider collider);
    bool CanInteract(Collider col);
    void InteractObject(Collider col);
    void LookToObject(Collider col);
}
