/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEditor;

namespace AuroraFPSEditor.Attributes
{
    public abstract class ApexSerializedField : ApexField
    {
        public readonly SerializedProperty TargetSerializedProperty;
        public readonly int Order;

        public ApexSerializedField(SerializedProperty source)
        {
            this.TargetSerializedProperty = source;
            Order = -1;
        }

        public ApexSerializedField(SerializedProperty source, int order) : this(source)
        {
            this.Order = order;
        }
    }
}