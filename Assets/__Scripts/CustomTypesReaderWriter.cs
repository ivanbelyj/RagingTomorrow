using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public static class CustomTypesReaderWriter
{
    public static void WriteLifecycleEffect(this NetworkWriter writer,
        LifecycleEffect effect) {

        writer.WriteFloat(effect.duration);
        writer.WriteBool(effect.isInfinite);
        writer.WriteBool(effect.recoverToInitial);
        writer.WriteFloat(effect.speed);

        writer.WriteByte((byte)effect.targetParameterIndex);
        // writer.WriteBool(effect.isActive);
        writer.WriteDouble(effect.startTime);
        // writer.WriteInt(effect.UpdateIterations);
        // writer.WriteInt(effect.UpdateIterationsCurrent);
    }

    public static LifecycleEffect ReadLifecycleEffect(this NetworkReader reader) {
        
        float duration = reader.ReadFloat();
        bool isInfinite = reader.ReadBool();
        bool recoverToInitial =  reader.ReadBool();
        float speed = reader.ReadFloat();

        byte targetParameterIndex = reader.ReadByte();
        // effect.isActive = reader.ReadBool();
        double startTime = reader.ReadDouble();

        LifecycleEffect effect = new LifecycleEffect() {
            speed = speed,
            isInfinite = isInfinite,
            recoverToInitial = recoverToInitial, 
            targetParameterIndex = (EntityParameterEnum)targetParameterIndex,
            duration = duration,
            startTime = startTime
        };
        return effect;
    }
}
