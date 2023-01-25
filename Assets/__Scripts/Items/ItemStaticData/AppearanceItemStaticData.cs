using System.Collections;
using System.Collections.Generic;
using AppearanceCustomization3D;
using UnityEngine;

/// <summary>
/// С некоторыми предметами могут быть ассоциированы модели, которые отображаются системой кастомизации
/// </summary>
public class AppearanceItemStaticData : ItemStaticData
{
    [SerializeField]
    private uint[] _characterAppearanceElementsLocalIds;
    /// <summary>
    /// Локальные для типа кастомизации, персонажа, id элементов кастомизации. Эти данные могут быть
    /// использованы для активирования / скрытия соотв. элементов составной модели
    /// </summary>
    private uint[] CharacterAppearanceElementsLocalIds => _characterAppearanceElementsLocalIds;

}
