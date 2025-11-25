/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.SystemModules.ControllerModules
{
    [System.Obsolete]
    [System.Serializable]
    public class LadderClimbLocomotion : CustomLocomotion
    {
        public enum ClimbDirection
        {
            Vertical,
            Free
        }

        [SerializeField] private ClimbDirection climbDirection = ClimbDirection.Vertical;
        [SerializeField] private float speed = 1.25f;
        [SerializeField] private float offset = 0.25f;

        // Stored required properties.
        private InteractiveLadder ladder;
        private Collider ladderCollider;

        /// <summary>
        /// Calculate ladder climb locomotion direction.
        /// </summary>
        /// <param name="smoothInputVector">Calculated and smoothed input data Vector2 representation.</param>
        /// <returns>Calculated controller move direction.</returns>
        public override Vector3 CalculateDirection(Vector2 smoothInputVector)
        {
            Bounds ladderBounds = ladderCollider.bounds;
            Vector3 ladderHigherPoint = new Vector3(ladderBounds.center.x, ladderBounds.max.y, ladderBounds.center.z);
            Vector3 offsetPoint = ladderHigherPoint - (Vector3.forward * offset);
            Vector3 desiredDirection = offsetPoint - controller.transform.position;
            desiredDirection += ladder.centerOffset;
            Debug.DrawRay(controller.transform.position, desiredDirection, Color.white);
            if (climbDirection == ClimbDirection.Free)
                desiredDirection = Vector3.right * smoothInputVector.x;
            desiredDirection.y = smoothInputVector.y * Mathf.Sign(desiredDirection.y);
            return desiredDirection;
        }

        /// <summary>
        /// Calculate ladder climb locomotion speed.
        /// </summary>
        /// <returns>Calculated controller move speed.</returns>
        public override float CalculateSpeed()
        {
            return speed;
        }


        /// <summary>
        /// Set ladder collider which controller need to climb.
        /// </summary>
        /// <param name="ladderCollider">Collider component of the ladder.</param>
        public void SetLadder(InteractiveLadder ladder, Collider ladderCollider)
        {
            this.ladder = ladder;
            this.ladderCollider = ladderCollider;
        }

        public Vector3 GetTargetPosition()
        {
            if (ladderCollider != null)
            {
                Bounds ladderBounds = ladderCollider.bounds;
                Vector3 ladderHigherPoint = new Vector3(ladderBounds.center.x, ladderBounds.max.y, ladderBounds.center.z);
                Vector3 offsetPoint = ladderHigherPoint - (ladderCollider.transform.forward * offset);
                return offsetPoint;
            }
            return controller.transform.position;
        }
    }
}

