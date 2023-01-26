using System.Collections;
using System.Collections.Generic;
using AppearanceCustomization3D;
using UnityEngine;

[RequireComponent(typeof(CustomizableAppearance))]
[RequireComponent(typeof(CharacterDataProvider))]
public class CharacterAppearance : MonoBehaviour
{
    [SerializeField]
    private CustomizableAppearance _appearance;
    private void Start() {
        var characterDataProvider = GetComponent<CharacterDataProvider>();
        _appearance.InstantiateByAppearanceData(characterDataProvider.CharacterData.AppearanceData);
    }
}
