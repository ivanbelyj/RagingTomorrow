/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.SystemModules.CameraSystems;
using AuroraFPSRuntime.SystemModules.ControllerSystems;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.WeaponModules
{
    [HideScriptField]
    [DisallowMultipleComponent]
    [AddComponentMenu("Aurora FPS Engine/Weapon Modules/Scope/Scope Utility")]
    [System.Obsolete]
    public sealed class ScopeUtility : MonoBehaviour
    {
        [SerializeField] 
        private Camera sightCamera;

        [SerializeField] 
        private GameObject renderTextureObj;

        [SerializeField] 
        private GameObject sightTextureObj;

        [SerializeField]
        private float durationUp;

        [SerializeField]
        private AnimationCurve upCurve;

        [SerializeField]
        private float durationDown;

        [SerializeField]
        private AnimationCurve downCurve;

        private PlayerCamera cameraControl;
        private Material renderTextureMaterial;
        private Material sightMaterial;

        private CoroutineObject<bool> sigthEaseInOutCoroutine;

        private void Awake()
        {
            PlayerController controller = transform.GetComponentInParent<PlayerController>();
            if (controller != null)
                cameraControl = controller.GetPlayerCamera();

            Renderer renderTextureObjRenderer = renderTextureObj.GetComponent<Renderer>();
            if (renderTextureObjRenderer != null)
                renderTextureMaterial = renderTextureObjRenderer.sharedMaterial;

            Renderer sightObjRenderer = sightTextureObj.GetComponent<Renderer>();
            if (sightObjRenderer != null)
                sightMaterial = sightObjRenderer.sharedMaterial;

            sigthEaseInOutCoroutine = new CoroutineObject<bool>(this);

        }

        private void OnEnable()
        {
            cameraControl.OnStartZoomCallback += ScopeEaseIn;
            cameraControl.OnStopZoomCallback += ScopeEaseOut;
            if (cameraControl.IsZooming())
                sigthEaseInOutCoroutine.Start(ScopeSwitcherProcessing, true, true);
        }

        private void OnDisable()
        {
            cameraControl.OnStartZoomCallback -= ScopeEaseIn;
            cameraControl.OnStopZoomCallback -= ScopeEaseOut;
            SightEnabled(false);
            sigthEaseInOutCoroutine.Stop();
        }

        private void ScopeEaseIn()
        {
            sigthEaseInOutCoroutine.Start(ScopeSwitcherProcessing, true, true);
        }

        private void ScopeEaseOut()
        {
            sigthEaseInOutCoroutine.Start(ScopeSwitcherProcessing, false, true);
        }

        private IEnumerator ScopeSwitcherProcessing(bool value)
        {
            if (value)
                SightEnabled(true);

            float time = 0;
            float speed = 1 / (value ? durationUp : durationDown);
            AnimationCurve curve = value ? upCurve : downCurve;
            Color targetColor = value ? Color.white : Color.black;

            while (time < 1.0f)
            {
                time += Time.deltaTime * speed;

                float smoothLerp = curve.Evaluate(time);
                renderTextureMaterial.color = Color.Lerp(renderTextureMaterial.color, targetColor, smoothLerp);
                yield return null;
            }

            if (!value)
                SightEnabled(false);
        }

        public void SightEnabled(bool active)
        {
            sightCamera.gameObject.SetActive(active);
            renderTextureObj.SetActive(active);
            sightTextureObj.SetActive(active);
        }

        

        #region [Getter / Setter]
        public Camera GetSightCamera()
        {
            return sightCamera;
        }

        public void SetSightCamera(Camera value)
        {
            sightCamera = value;
        }

        public GameObject GetRenderTextureObj()
        {
            return renderTextureObj;
        }

        public void SetRenderTextureObj(GameObject value)
        {
            renderTextureObj = value;
        }

        public GameObject GetSightTextureObj()
        {
            return sightTextureObj;
        }

        public void SetSightTextureObj(GameObject value)
        {
            sightTextureObj = value;
        }

        public float GetDurationUp()
        {
            return durationUp;
        }

        public void SetDurationUp(float value)
        {
            durationUp = value;
        }

        public AnimationCurve GetUpCurve()
        {
            return upCurve;
        }

        public void SetUpCurve(AnimationCurve value)
        {
            upCurve = value;
        }

        public float GetDurationDown()
        {
            return durationDown;
        }

        public void SetDurationDown(float value)
        {
            durationDown = value;
        }

        public AnimationCurve GetDownCurve()
        {
            return downCurve;
        }

        public void SetDownCurve(AnimationCurve value)
        {
            downCurve = value;
        }
        #endregion
    }
}