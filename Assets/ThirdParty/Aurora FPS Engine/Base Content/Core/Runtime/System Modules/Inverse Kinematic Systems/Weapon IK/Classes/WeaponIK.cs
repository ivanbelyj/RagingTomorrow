/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.CoreModules.Coroutines;
using AuroraFPSRuntime.CoreModules.ValueTypes;
using System.Collections;
using UnityEngine;

namespace AuroraFPSRuntime.SystemModules
{
    [RequireComponent(typeof(Animator))]
    public abstract class WeaponIK : MonoBehaviour
    {
        [System.Serializable]
        protected sealed class WeaponSettings
        {
            [SerializeField]
            [NotNull]
            private GameObject weapon;

            [SerializeField]
            private Transform rightHand;

            [SerializeField]
            private Transform leftHand;

            #region [Getter / Setter]
            public GameObject GetWeapon()
            {
                return weapon;
            }

            public void SetWeapon(GameObject value)
            {
                weapon = value;
            }

            public Transform GetLeftHand()
            {
                return leftHand;
            }

            public void SetLeftHand(Transform value)
            {
                leftHand = value;
            }

            public Transform GetRightHand()
            {
                return rightHand;
            }

            public void SetRightHand(Transform value)
            {
                rightHand = value;
            }
            #endregion
        }

        [SerializeField]
        protected Transform body;

        [SerializeField]
        [NotNull]
        protected Transform targetsHolder;

        [SerializeField]
        [NotNull]
        protected Transform lookTarget;

        [SerializeField]
        protected AnimatorState selectState = "Select";

        [SerializeField]
        [Slider(1, 5)]
        protected float selectTime = 3.5f;

        // Stored required components.
        protected Animator animator;

        // Stored required properties.
        protected CoroutineObject coroutineObject;
        protected Transform rightShoulder;
        protected float handWeights = 1f;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();

            coroutineObject = new CoroutineObject(this);
            rightShoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            SholderHandler();
            HandIK();
        }

        #region [Hand IK]
        protected abstract void HandIK();

        protected void HandIKHandle(WeaponSettings settings)
        {
            Transform leftHand = settings.GetLeftHand();
            if (leftHand != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handWeights);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handWeights);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHand.rotation);
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
            }

            Transform rightHand = settings.GetRightHand();
            if (rightHand != null)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handWeights);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handWeights);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);
            }
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
            }
        }

        private void SholderHandler()
        {
            Vector3 offset = Vector3.zero;
            if (body != null)
            {
                offset = transform.position - body.position;
            }

            targetsHolder.position = rightShoulder.position + offset;
            targetsHolder.rotation = Quaternion.LookRotation(lookTarget.forward);
        }

        private IEnumerator IKSmoothWeight()
        {
            float duration = animator.GetCurrentAnimatorStateInfo(1).length;

            handWeights = 0f;

            float time = 0;
            float speed = 1f / duration;
            while (time < 1f)
            {
                yield return null;
                time += speed * Time.deltaTime;
                handWeights = Mathf.Pow(time, selectTime);
            }

            handWeights = 1f;
        }
        #endregion

        #region [Active/Disactive Weapon Methods]
        protected void ActiveWeapon(WeaponSettings settings)
        {
            DiactivateAllWeapons();

            settings.GetWeapon().SetActive(true);

            if (!string.IsNullOrEmpty(selectState))
            {
                animator.CrossFadeInFixedTime(selectState);

                coroutineObject.Start(IKSmoothWeight, true);
            }
        }

        protected void DisableWeapon(WeaponSettings settings)
        {
            settings.GetWeapon().SetActive(false);
        }

        protected abstract void DiactivateAllWeapons();
        #endregion
    }
}