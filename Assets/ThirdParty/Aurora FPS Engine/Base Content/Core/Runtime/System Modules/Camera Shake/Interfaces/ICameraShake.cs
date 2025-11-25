/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    public interface ICameraShake
    {
        /// <summary>
        /// CameraShaker calls this when the shake is added to the list of active shakes.
        /// </summary>
        void Initialize(Vector3 cameraPosition, Quaternion cameraRotation);

        /// <summary>
        /// CameraShaker calls this every frame on active shakes.
        /// </summary>
        void Update(float deltaTime, Vector3 cameraPosition, Quaternion cameraRotation);

        /// <summary>
        /// Shake system will dispose the shake on the first frame when this is true.
        /// </summary>
        bool IsFinished();

        /// <summary>
        /// Represents current position and rotation of the camera according to the shake.
        /// </summary>
        Displacement GetCurrentDisplacement();
    }
}
