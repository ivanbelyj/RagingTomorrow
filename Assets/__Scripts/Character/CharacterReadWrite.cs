using System.Collections;
using System.Collections.Generic;
using AppearanceCustomization3D;
using Mirror;
using UnityEngine;

public static class CharacterReadWrite
{
    public static void WriteCharacterData(this NetworkWriter writer, CharacterData data) {
        writer.WriteAppearanceData(data.AppearanceData);
        writer.WriteString(data.Name);
        // writer.WriteString(data.Subtitle);
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

    public static CharacterData ReadCharacterData(this NetworkReader reader) {
        CharacterData res = new CharacterData();
        res.AppearanceData = reader.ReadAppearanceData();
        res.Name = reader.ReadString();
        res.Subtitle = "Deserialized subtitle"; // reader.ReadString();
        
        return res;
    }
}
