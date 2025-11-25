/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules.RecoilSystem
{
    [HideScriptField]
    [CreateAssetMenu(fileName = "Recoil Config", menuName = "Aurora FPS Engine/Weapon/Recoil Config", order = 130)]
    public sealed class RecoilConfig : ScriptableObject
    {
        [System.Serializable]
        public class RecoilOverride
        {
            [SerializeField]
            private ControllerState state;

            [SerializeField]
            private bool completeMatch = false;

            [SerializeField]
            private CameraRecoil cameraRecoil;

            [SerializeField]
            private BulletRecoil bulletRecoil;

            #region [Getter / Setter]
            public ControllerState GetState()
            {
                return state;
            }

            public void SetState(ControllerState value)
            {
                state = value;
            }

            public bool CompleteMatch()
            {
                return completeMatch;
            }

            public void CompleteMatch(bool value)
            {
                completeMatch = value;
            }

            public CameraRecoil GetCameraRecoil()
            {
                return cameraRecoil;
            }

            public void SetCameraRecoil(CameraRecoil value)
            {
                cameraRecoil = value;
            }

            public BulletRecoil GetBulletRecoil()
            {
                return bulletRecoil;
            }

            public void SetBulletRecoil(BulletRecoil value)
            {
                bulletRecoil = value;
            }
            #endregion
        }

        [SerializeField]
        [HideExpandButton]
        [TabGroup("Default Recoil", "Camera Recoil")]
        private CameraRecoil defaultCameraRecoil;

        [SerializeField]
        [HideExpandButton]
        [TabGroup("Default Recoil", "Bullet Recoil")]
        private BulletRecoil defaultBulletRecoil;

        [SerializeField]
        [ReorderableList(GetElementLabelCallback = "GetOverrideLabel")]
        private List<RecoilOverride> recoilOverrides;

        public bool TryFindOverride(PlayerController controller, out CameraRecoil cameraRecoil, out BulletRecoil bulletRecoil)
        {
            for (int i = 0; i < recoilOverrides.Count; i++)
            {
                RecoilOverride recoilOverride = recoilOverrides[i];
                if ((recoilOverride.CompleteMatch() && controller.GetState() == recoilOverride.GetState()) ||
                    (!recoilOverride.CompleteMatch() && (controller.GetState() & recoilOverride.GetState()) != 0))
                {
                    cameraRecoil = recoilOverride.GetCameraRecoil();
                    cameraRecoil.SetShaker(controller.GetPlayerCamera().GetShaker());
                    bulletRecoil = recoilOverride.GetBulletRecoil();
                    return true;
                }
            }
            cameraRecoil = defaultCameraRecoil;
            cameraRecoil.SetShaker(controller.GetPlayerCamera().GetShaker());
            bulletRecoil = defaultBulletRecoil;
            return false;
        }

        [System.Obsolete]
        public bool TryFindOverride(ControllerState state, out CameraRecoil cameraRecoil, out BulletRecoil bulletRecoil)
        {
            for (int i = 0; i < recoilOverrides.Count; i++)
            {
                RecoilOverride recoilOverride = recoilOverrides[i];
                if ((recoilOverride.CompleteMatch() && state == recoilOverride.GetState()) ||
                    (!recoilOverride.CompleteMatch() && (state & recoilOverride.GetState()) != 0))
                {
                    cameraRecoil = recoilOverride.GetCameraRecoil();
                    bulletRecoil = recoilOverride.GetBulletRecoil();
                    return true;
                }
            }
            cameraRecoil = defaultCameraRecoil;
            bulletRecoil = defaultBulletRecoil;
            return false;
        }

        #region [Editor Section]
#if UNITY_EDITOR
        private string GetOverrideLabel(UnityEditor.SerializedProperty property, int index)
        {
            UnityEditor.SerializedProperty state = property.FindPropertyRelative("state");
            return ((ControllerState)state.intValue).ToString();
        }
#endif
        #endregion

        #region [Getter / Setter]
        public CameraRecoil GetDefaultCameraRecoil()
        {
            return defaultCameraRecoil;
        }

        public void SetDefaultCameraRecoil(CameraRecoil value)
        {
            defaultCameraRecoil = value;
        }

        public BulletRecoil GetDefaultBulletRecoil()
        {
            return defaultBulletRecoil;
        }

        public void SetDefaultBulletRecoil(BulletRecoil value)
        {
            defaultBulletRecoil = value;
        }

        public List<RecoilOverride> GetRecoilOverrides()
        {
            return recoilOverrides;
        }

        public void SetRecoilOverrides(List<RecoilOverride> value)
        {
            recoilOverrides = value;
        }

        public RecoilOverride GetRecoilOverride(int index)
        {
            return recoilOverrides[index];
        }

        public void SetRecoilOverride(int index, RecoilOverride value)
        {
            recoilOverrides[index] = value;
        }

        public int GetRecoilOverrideCount()
        {
            return recoilOverrides.Count;
        }
        #endregion
    }
}