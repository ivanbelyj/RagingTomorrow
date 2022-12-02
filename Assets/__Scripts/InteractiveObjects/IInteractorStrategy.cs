using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// С предметами, с которыми произошло взаимодействие, можно оперировать разными способами.
/// Поместить в инвентарь, отобразить всплывающую подсказку, и т.д.
/// </summary>
public interface IInteractorStrategy
{
    // TInteractObject GetInteractObject(Collider collider);
    /// <summary>
    /// true, если с предметом, имеющим данный коллайдер, возможно взаимодействовать
    /// </summary>
    bool CanInteract(Collider col);

    /// <summary>
    /// Непосредственное взаимодействие с объектом, имеющим данный коллайдер. Вызывается
    /// в Interactor, только если с предметом можно взаимодействовать,
    /// т.е. CanInteract вернул true.
    /// </summary>
    void InteractObject(Collider col);

    /// <summary>
    /// Вызывается один раз в Interactor, когда он "посмотрел" на предмет, с которым можно
    /// взаимодействовать
    /// т.е. CanInteract вернул true.
    /// </summary>
    void LookedAtObject(Collider col);

    /// <summary>
    /// Вызывается один раз в Interactor, когда он "отвел взгляд" с предмета, с которым можно
    /// взаимодействовать
    /// т.е. CanInteract вернул true.
    /// </summary>
    void LookedAwayFromObject();
}
