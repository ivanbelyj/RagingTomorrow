using System.Collections;
using System.Collections.Generic;
using AppearanceCustomization3D;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    [SerializeField]
    private string _name;
    public string Name { get => _name; set => _name = value; }

    [SerializeField]
    private string _subtitle;
    public string Subtitle { get => _subtitle; set => _subtitle = value; }
    
    [SerializeField]
    private AppearanceData _appearanceData;
    public AppearanceData AppearanceData { get => _appearanceData; set => _appearanceData = value; }

    public override string ToString()
    {
        return $"CharacterData. Name: {Name}; Subtitle: {Subtitle}; AppearanceData: {AppearanceData}";
    }
}
