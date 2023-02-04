using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public static class LifecycleEffectReadWrite
{
    public static void WriteLifecycleEffect(this NetworkWriter writer,
        LifecycleEffect effect) {

        writer.WriteFloat(effect.duration);
        writer.WriteBool(effect.isInfinite);
        // writer.WriteBool(effect.recover);
        writer.WriteFloat(effect.speed);

        writer.WriteByte((byte)effect.targetParameter);
        // writer.WriteBool(effect.isActive);
        writer.WriteDouble(effect.StartTime);
        // writer.WriteUShort(effect.effectId);
        // writer.WriteInt(effect.UpdateIterations);
        // writer.WriteInt(effect.UpdateIterationsCurrent);
    }

    public static LifecycleEffect ReadLifecycleEffect(this NetworkReader reader) {
        
        float duration = reader.ReadFloat();
        bool isInfinite = reader.ReadBool();
        // bool recoverToInitial =  reader.ReadBool();
        float speed = reader.ReadFloat();

        byte targetParameterIndex = reader.ReadByte();
        // effect.isActive = reader.ReadBool();
        double startTime = reader.ReadDouble();
        // ushort effectId = reader.ReadUShort();

        LifecycleEffect effect = new LifecycleEffect() {
            speed = speed,
            isInfinite = isInfinite,
            // recover = recoverToInitial, 
            targetParameter = (LifecycleParameterEnum)targetParameterIndex,
            duration = duration,
            StartTime = startTime,
            // effectId = effectId
        };
        return effect;
    }
}
