/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [HideScriptField]
    [AddComponentMenu(null)]
    [DisallowMultipleComponent]
    public abstract class LootObject : InteractiveObject
    {
        private enum DisposeMode
        {
            Hide,
            Destroy,
            PushInPool
        }

        [SerializeField]
        [Order(-599)]
        private DisposeMode disposeMode = DisposeMode.Destroy;

        [SerializeField]
        [VisibleIf("disposeMode", "PushInPool")]
        [NotNull]
        [Indent(1)]
        [Order(-598)]
        private PoolObject poolObject;

        /// <summary>
        /// Called when character being loot object.
        /// </summary>
        /// <param name="other">Reference of loot object transform.</param>
        /// <returns>The success of looting the specified object.</returns>
        protected abstract bool OnLoot(Transform other);

        public sealed override bool Execute(Transform other)
        {
            if (OnLoot(other))
            {
                switch (disposeMode)
                {
                    case DisposeMode.Hide:
                        gameObject.SetActive(false);
                        break;
                    case DisposeMode.Destroy:
                        Destroy(gameObject);
                        break;
                    case DisposeMode.PushInPool:
                        poolObject.Push();
                        break;
                }
                return true;
            }
            return false;
        }

        #region [Getter / Setter]
        private DisposeMode GetDisposeMode()
        {
            return disposeMode;
        }

        private void SetDisposeMode(DisposeMode value)
        {
            disposeMode = value;
        }

        public PoolObject GetPoolObject()
        {
            return poolObject;
        }

        public void SetPoolObject(PoolObject value)
        {
            poolObject = value;
        }
        #endregion
    }
}