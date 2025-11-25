/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.HealthModules
{
    public struct DamageInfo
    {
        public readonly Transform sender;
        public readonly Vector3 point;
        public readonly Vector3 normal;

        public DamageInfo(Transform sender)
        {
            this.sender = sender;
            point = Vector3.zero;
            normal = Vector3.zero;
        }

        public DamageInfo(Transform sender, Vector3 point, Vector3 normal) : this(sender)
        {
            this.point = point;
            this.normal = normal;
        }

        public override string ToString()
        {
            return $"Damage Info (Sender: {sender.name}, Point: {point}, Normal: {normal})";
        }
    }
}