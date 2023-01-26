using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace AppearanceCustomization3D {
    public static class AppearanceReadWrite
    {
        public static void WriteAppearanceElementLocalId(this NetworkWriter writer,
            AppearanceElementLocalId localId) {
            writer.WriteUInt(localId.Value);
        }
        public static AppearanceElementLocalId ReadAppearanceElementLocalId(this NetworkReader reader) {
            return new AppearanceElementLocalId(reader.ReadUInt());
        }

        public static void WriteAppearanceTypeId(this NetworkWriter writer,
            AppearanceTypeId typeId) {
            writer.WriteString(typeId.Value);
        }
        public static AppearanceTypeId ReadAppearanceTypeId(this NetworkReader reader) {
            return new AppearanceTypeId(reader.ReadString());
        }

        public static void WriteOccupancyId(this NetworkWriter writer,
            OccupancyId id) {
            writer.WriteUInt(id.Value);
        }
        public static OccupancyId ReadOccupancyId(this NetworkReader reader) {
            return new OccupancyId(reader.ReadUInt());
        }

        public static void WriteAppearanceData(this NetworkWriter writer, AppearanceData data) {
            writer.WriteAppearanceTypeId(data.AppearanceTypeId);
            writer.WriteInt(data.AppearanceElementIds.Count);
            foreach (var id in data.AppearanceElementIds) {
                writer.WriteAppearanceElementLocalId(id);
            }
        }
        public static AppearanceData ReadAppearanceData(this NetworkReader reader) {
            var id = reader.ReadAppearanceTypeId();
            int localIdsCount = reader.ReadInt();
            var localIds = new List<AppearanceElementLocalId>(localIdsCount);
            for (int i = 0; i < localIdsCount; i++) {
                localIds.Add(reader.ReadAppearanceElementLocalId());
            }
            // var localIds = new List<AppearanceElementLocalId>() {
            //     new AppearanceElementLocalId(0),
            //     new AppearanceElementLocalId(1)
            // };
            return new AppearanceData() {
                AppearanceTypeId = id,
                AppearanceElementIds = localIds
            };
        }
    }
}
