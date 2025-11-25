/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using UnityEngine;
using ControllerState = AuroraFPSRuntime.SystemModules.ControllerSystems.ControllerState;

namespace AuroraFPSRuntime.UIModules.UIElements.Crosshair
{
    [HideScriptField]
    [AddComponentMenu("Aurora FPS Engine/UI Modules/UI Elements/Crosshair/Crosshair")]
    [DisallowMultipleComponent]
    public class Crosshair : MonoBehaviour
    {
        [System.Serializable]
        public class SpreadEvent : CallbackEvent<CrosshairSpread> { }

        public enum SpreadUpdateFunction
        {
            Static,
            SmoothStep,
            Lerp,
            MoveTowerds
        }

        [SerializeReference] 
        [Label("Preset")]
        [DropdownReference]
        [TabGroup("Crosshair Tab", "Main Crosshair")]
        private CrosshairPreset crosshairPreset;

        [SerializeField]
        [ReorderableList(ElementLabel = "State {niceIndex}", DisplayHeader = false)]
        [TabGroup("Crosshair Tab", "Main Crosshair")]
        [Foldout("States", Style = "Header")]
        private CrosshairState[] crosshairStates = new CrosshairState[5] 
        {
            new CrosshairState(ControllerState.Idle, new CrosshairSpread(30.0f, 7.0f)),
            new CrosshairState(ControllerState.Walking, new CrosshairSpread(50.0f, 7.0f)),
            new CrosshairState(ControllerState.Running, new CrosshairSpread(80.0f, 7.0f)),
            new CrosshairState(ControllerState.Sprinting, new CrosshairSpread(120.0f, 7.0f)),
            new CrosshairState(ControllerState.Crouched, new CrosshairSpread(15.0f, 7.0f)),
        };

        [SerializeField]
        [TabGroup("Crosshair Tab", "Main Crosshair")]
        [Foldout("Fire Settings", Style = "Header")]
        private SpreadEvent fireEvent;

        [SerializeReference]
        [Label("Preset")]
        [DropdownReference]
        [TabGroup("Crosshair Tab", "Hit Effect")]
        private CrosshairPreset hitPreset;

        [SerializeField]
        [HideExpandButton]
        [TabGroup("Crosshair Tab", "Hit Effect")]
        [Foldout("Hit Settings", Style = "Header")]
        private SpreadEvent hitEvent;

        [SerializeField]
        [TabGroup("Crosshair Tab", "Hit Effect")]
        [Foldout("Hit Settings", Style = "Header")]
        private float hitHideSpeed;

        [SerializeField]
        [TabGroup("Crosshair Tab", "Hit Effect")]
        [Foldout("Hit Settings", Style = "Header")]
        private float hitHideValue;

        [SerializeReference]
        [Label("Preset")]
        [DropdownReference]
        [TabGroup("Crosshair Tab", "Kill Effect")]
        private CrosshairPreset killPreset;

        [SerializeField]
        [HideExpandButton]
        [TabGroup("Crosshair Tab", "Kill Effect")]
        [Foldout("Kill Settings", Style = "Header")]
        private SpreadEvent killEvent;

        [SerializeField]
        [TabGroup("Crosshair Tab", "Kill Effect")]
        [Foldout("Kill Settings", Style = "Header")]
        private float killHideSpeed;

        [SerializeField]
        [TabGroup("Crosshair Tab", "Kill Effect")]
        [Foldout("Kill Settings", Style = "Header")]
        private float killHideValue;

        // Stored required properties.
        private PlayerController controller;

        // Stored required properties.
        private float spread;
        private float storedHitSpread;
        private float storedKillSpread;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        protected virtual void Awake()
        {
            controller = transform.GetComponentInParent<PlayerController>();
            crosshairPreset?.Initialize(controller);
            hitPreset?.Initialize(controller);
            killPreset?.Initialize(controller);
            fireEvent.RegisterCallback(ApplyCustomSpread);
            hitEvent.RegisterCallback(ShowHitEffect);
            killEvent.RegisterCallback(ShowKillEffect);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        protected virtual void Update()
        {
            OnBasePresetGUI();
            OnHitPresetGUI();
            OnKillPresetGUI();
        }

        /// <summary>
        /// Main crosshair GUI processing.
        /// </summary>
        protected virtual void OnBasePresetGUI()
        {
            if(crosshairPreset != null)
            {
                crosshairPreset.DrawElements(GetSpreadValue());
            }
        }

        protected virtual void OnHitPresetGUI()
        {
            if(hitPreset != null)
            {
                if (storedHitSpread >= hitHideValue)
                {
                    storedHitSpread = Mathf.SmoothStep(storedHitSpread, 0, hitHideSpeed * Time.deltaTime);
                    hitPreset.SetVisibility(true);
                    hitPreset.DrawElements(storedHitSpread);
                }
                else
                {
                    hitPreset.SetVisibility(false);
                }
            }
        }

        protected virtual void OnKillPresetGUI()
        {
            if(killPreset != null)
            {
                if (storedKillSpread >= killHideValue)
                {
                    storedKillSpread = Mathf.SmoothStep(storedKillSpread, 0, killHideSpeed * Time.deltaTime);
                    killPreset.SetVisibility(true);
                    killPreset.DrawElements(storedKillSpread);
                }
                else
                {
                    killPreset.SetVisibility(false);
                }
            }
        }

        /// <summary>
        /// Processing crosshair spread value relative controller states.
        /// </summary>
        protected float GetSpreadValue()
        {
            if (crosshairStates != null && crosshairStates.Length > 0 && controller != null)
            {
                for (int i = 0, length = crosshairStates.Length; i < length; i++)
                {
                    CrosshairState crosshairState = crosshairStates[i];
                    if (controller.CompareState(crosshairState.GetState()))
                    {
                        CrosshairSpread crosshairSpread = crosshairState.GetCrosshairSpread();
                        spread = Mathf.Lerp(spread, crosshairSpread.GetValue(), crosshairSpread.GetSpeed() * Time.deltaTime);
                    }
                }
            }
            return spread;
        }

        /// <summary>
        /// Override current crosshair spread and apply fire spread value.
        /// </summary>
        public void ApplyCustomSpread(CrosshairSpread crosshairSpread)
        {
            spread = Mathf.Lerp(spread, crosshairSpread.GetValue(), crosshairSpread.GetSpeed() * Time.deltaTime);
        }

        /// <summary>
        /// Show crosshair hit effect.
        /// </summary>
        public void ShowHitEffect(CrosshairSpread crosshairSpread)
        {
            storedHitSpread = Mathf.Lerp(storedHitSpread, crosshairSpread.GetValue(), crosshairSpread.GetSpeed() * Time.deltaTime);
        }

        /// <summary>
        /// Show crosshair kill effect.
        /// </summary>
        public void ShowKillEffect(CrosshairSpread crosshairSpread)
        {
            storedHitSpread = -1;
            storedKillSpread = Mathf.Lerp(storedKillSpread, crosshairSpread.GetValue(), crosshairSpread.GetSpeed() * Time.deltaTime);
        }

        #region [Getter / Setter]
        public PlayerController GetController()
        {
            return controller;
        }

        protected void SetController(PlayerController value)
        {
            controller = value;
        }

        public CrosshairPreset GetCrosshairPreset()
        {
            return crosshairPreset;
        }

        public void SetCrosshairPreset(CrosshairPreset value)
        {
            crosshairPreset = value;
        }

        public CrosshairState[] GetCrosshairStates()
        {
            return crosshairStates;
        }

        public CrosshairState GetCrosshairState(int index)
        {
            return crosshairStates[index];
        }

        public void SetCrosshairStates(CrosshairState[] value)
        {
            crosshairStates = value;
        }

        public void SetCrosshairState(int index, CrosshairState state)
        {
            crosshairStates[index] = state;
        }

        public int GetCrosshairStatesLength()
        {
            return crosshairStates?.Length ?? 0;
        }

        public float GetSpread()
        {
            return spread;
        }

        public void SetSpread(float value)
        {
            spread = value;
        }

        public CrosshairPreset GetHitPreset()
        {
            return hitPreset;
        }

        public void SetHitPreset(CrosshairPreset value)
        {
            hitPreset = value;
        }

        public float GetHitHideSpeed()
        {
            return hitHideSpeed;
        }

        public void SetHitHideSpeed(float value)
        {
            hitHideSpeed = value;
        }

        public float GetHitHideValue()
        {
            return hitHideValue;
        }

        public void SetHitHideValue(float value)
        {
            hitHideValue = value;
        }

        public CrosshairPreset GetKillPreset()
        {
            return killPreset;
        }

        public void SetKillPreset(CrosshairPreset value)
        {
            killPreset = value;
        }

        public float GetKillHideSpeed()
        {
            return killHideSpeed;
        }

        public void SetKillHideSpeed(float value)
        {
            killHideSpeed = value;
        }

        public float GetKillHideValue()
        {
            return killHideValue;
        }

        public void SetKillHideValue(float value)
        {
            killHideValue = value;
        }
        #endregion
    }
}