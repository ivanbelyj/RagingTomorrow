using System.Collections;
using System.Collections.Generic;
using AppearanceCustomization3D;
using Mirror;
using UnityEngine;

public static class CharacterReadWrite
{
    public static void WriteCharacterData(this NetworkWriter writer, CharacterData data) {
        writer.WriteBool(data != null);
        if (data == null)
            return;
        
        Debug.Log("Serializing. Input: " + data.ToString());
        writer.WriteString(data.Name);
        writer.WriteString(data.Subtitle);
        writer.WriteAppearanceData(data.AppearanceData);
    }

    private static AppearanceData GetTestAppearanceData() {
        return new AppearanceData() {
            AppearanceTypeId = new AppearanceTypeId("CharacterAppearanceType"),
            AppearanceElementIds = new List<AppearanceElementLocalId>() {
                new AppearanceElementLocalId(0),
                new AppearanceElementLocalId(1)
            }
        };
    }

    private static CharacterData GetTestCharacterData() {
        CharacterData res = new CharacterData();
        res.AppearanceData = GetTestAppearanceData(); // reader.ReadAppearanceData();
        res.Name = "Deserialized title"; // reader.ReadString();
        res.Subtitle = "Deserialized subtitle"; // reader.ReadString();
        return res;
    }

    public static CharacterData ReadCharacterData(this NetworkReader reader) {
        if (!reader.ReadBool())
            return null;
        
        CharacterData res = new CharacterData();
        res.Name = reader.ReadString();
        res.Subtitle = reader.ReadString();
        res.AppearanceData = reader.ReadAppearanceData();
        Debug.Log("Deserialized: " + res.ToString());
        
        return res;
    }
}
