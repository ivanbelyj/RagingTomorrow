/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

namespace AuroraFPSRuntime.AIModules.Behaviour
{
    [AIBehaviourMenu("Idle", "Idle", Hide = true)]
    internal sealed class AIIdleBehaviour : AIBehaviour
    {
        #region [Const Properties]
        public const string IDLE_BEHAVIOUR = "Idle";
        #endregion

        protected override void Update() { }
    }
}