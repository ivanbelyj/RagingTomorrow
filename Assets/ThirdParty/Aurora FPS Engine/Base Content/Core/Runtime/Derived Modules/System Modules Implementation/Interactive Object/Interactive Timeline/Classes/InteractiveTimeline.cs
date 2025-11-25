/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;
using AuroraFPSRuntime.Attributes;
using UnityEngine.Playables;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/System Modules/Interactive/Interactive Timeline")]
    [DisallowMultipleComponent]
    public class InteractiveTimeline : InteractiveObject
    {
        [SerializeField]
        [NotNull]
        private PlayableDirector playableDirector;

        [SerializeField]
        private bool executeOnce = false;

        // Stored required properties.
        private bool isExecuted;

        public override bool Execute(Transform other)
        {
            if(executeOnce)
            {
                if (isExecuted)
                {
                    return false;
                }
                isExecuted = true;
            }

            playableDirector.Play();
            return true;
        }

        protected override void CalculateMessageCode(Transform other, out int messageCode)
        {
            messageCode = isExecuted ? 1 : 0;
        }

        public void ResetExecution()
        {
            isExecuted = false;
        }

        #region [Getter / Setter]
        public PlayableDirector GetPlayableDirector()
        {
            return playableDirector;
        }

        public void SetPlayableDirector(PlayableDirector value)
        {
            playableDirector = value;
        }

        public bool ExecuteOnce()
        {
            return executeOnce;
        }

        public void ExecuteOnce(bool value)
        {
            executeOnce = value;
        }
        #endregion
    }
}
