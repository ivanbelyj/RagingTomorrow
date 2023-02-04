using UnityEngine;
using UnityEngine.Events;

namespace AFPC {

    /// <summary>
    /// This class allows the user to move.
    /// </summary>
    [System.Serializable]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Movement {

        public bool isDebugLog;

        [Header("Inputs")]
        public Vector3 movementInputValues;
        public bool runningInputValue;
        public bool jumpingInputValue;

        [Header("Acceleration")]
        public float referenceAcceleration = 2.66f;
        private float currentAcceleration = 2.5f;
        private float movementSmoothing = 0.3f;
        private Vector3 vector3_Reference;
        private Vector3 vector3_Target;
        private Vector3 delta;
        private bool isMovementAvailable = true;
        private bool releaseAcceleration = true;

        [Header("Running")]
        public float runningAcceleration = 5.32f;
        private bool isRunningAvaiable = true;

        [Header("Endurance")]
        public float referenceEndurance = 20.0f;
        private float endurance = 20.0f;

        [Header("Jumping")]
        public float jumpForce = 7.5f;
        private bool isJumpingAvailable = true;
        private bool isAirControl = true;
        private Vector3 groungCheckPosition;
        private bool isLandingActionPerformed;
        private UnityAction landingAction;
	
        [Header("Physics")]
        public bool isGeneratePhysicMaterial = true;
        public float mass = 70.0f;
        public float drag = 3.0f;
	    [Tooltip ("For Initialize()")] public float height = 1.6f;

        [Header("Looking for ground")]
        public LayerMask groundMask = 1;
        private bool isGrounded;

        [Header("References")]
        public Rigidbody rb;
        public CapsuleCollider cc;

        private float epsilon = 0.01f;

        /// <summary>
        /// Initialize the movement. Generate physic material if needed. Prepare the rigidbody.
        /// </summary>
        public virtual void Initialize () {
            rb.freezeRotation = true;
            rb.mass = mass;
            rb.drag = drag;
            cc.height = height;
            if (isGeneratePhysicMaterial) {
                PhysicMaterial physicMaterial = new PhysicMaterial {
                    name = "Generated Material",
                    bounciness = 0.01f,
                    dynamicFriction = 0.0f,
                    staticFriction = 0.0f,
                    frictionCombine = PhysicMaterialCombine.Minimum,
                    bounceCombine = PhysicMaterialCombine.Minimum
                };
                cc.material = physicMaterial;
            }
        }

        /// <summary>
        /// Allow the user to move.
        /// </summary>
        public virtual void AllowMovement () {
            isMovementAvailable = true;
            if (isDebugLog) Debug.Log (rb.gameObject.name + ": Allow Movement");
        }


        /// <summary>
        /// Ban the user to move. Optional, immediately stop the rigidbody.
        /// </summary>
        /// <param name="isStopImmediately"></param>
        public virtual void BanMovement (bool isStopImmediately = false) {
            isMovementAvailable = true;
            if (isDebugLog) Debug.Log (rb.gameObject.name + ": Ban Movement");
            if (isStopImmediately) {
                rb.velocity = Vector3.zero;
                if (isDebugLog) Debug.Log (rb.gameObject.name + ": Stop Movement");
            }
        }

        /// <summary>
        /// Allow the user to move faster.
        /// </summary>
        public virtual void AllowRunning () {
            isRunningAvaiable = true;
            if (isDebugLog) Debug.Log (rb.gameObject.name + ": Allow Running");
        }

        /// <summary>
        /// Ban the uset to move faster.
        /// </summary>
        public virtual void BanRunning () {
            isRunningAvaiable = false;
            if (isDebugLog) Debug.Log (rb.gameObject.name + ": Ban Running");
        }

        /// <summary>
        /// Allow the user to jump up.
        /// </summary>
        public virtual void AllowJumping () {
            isJumpingAvailable = true;
            if (isDebugLog) Debug.Log (rb.gameObject.name + ": Allow Jumping");
        }

        /// <summary>
        /// Ban the user from jumping up.
        /// </summary>
        public virtual void BanJumping () {
            isJumpingAvailable = false;
            if (isDebugLog) Debug.Log (rb.gameObject.name + ": Ban Jumping");
        }

        /// <summary>
        /// Perform an action when the character was landed.
        /// </summary>
        /// <param name="action"></param>
        public void AssignLandingAction (UnityAction action) {
            landingAction = action;
        }

        public void ClearLandingAction () {
            landingAction = null;
        }

        /// <summary>
        /// Allow the user to change movement direction in the air.
        /// </summary>
        public virtual void AllowAirControl () {
            isAirControl = true;
            if (isDebugLog) Debug.Log (rb.gameObject.name + ": Allow Air Control");
        }

        /// <summary>
        /// Ban the user to change movement direction in the air.
        /// </summary>
        public virtual void BanAirControl () {
            isAirControl = false;
            if (isDebugLog) Debug.Log (rb.gameObject.name + ": Ban Air Control");
        }

        /// <summary>
        /// Current endurance value.
        /// </summary>
        /// <returns></returns>
        public float GetEnduranceValue () {
            return endurance;
        }

        /// <summary>
        /// Is this controller on the ground?
        /// </summary>
        /// <returns></returns>
        public bool IsGrounded () {
            return isGrounded;
        }

        /// <summary>
        /// Physical movement. Better use it in FixedUpdate.
        /// </summary>
        public virtual void Accelerate () {
            LookingForGround ();
            MoveTorwardsAcceleration ();
            if (!isMovementAvailable) return;
            if (!rb) return;
            if (System.Math.Abs(movementInputValues.x) < epsilon & System.Math.Abs(movementInputValues.y) < epsilon) return;
            if (!isAirControl) {
                if (!isGrounded) return;
            }
            if (rb.velocity.magnitude > 1.0f) {
                rb.interpolation = RigidbodyInterpolation.Extrapolate;
            }
            else {
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
            delta = new Vector3 (movementInputValues.x, 0, movementInputValues.y);
            delta = Vector3.ClampMagnitude (delta, 1);
            delta = rb.transform.TransformDirection (delta) * currentAcceleration;
            vector3_Target = new Vector3 (delta.x, rb.velocity.y, delta.z);
            rb.velocity = Vector3.SmoothDamp (rb.velocity, vector3_Target, ref vector3_Reference, Time.smoothDeltaTime * movementSmoothing);
        }

        /// <summary>
        /// Jumping state. Better use it in Update.
        /// </summary>
	    public virtual void Jumping () {
		    if (!isJumpingAvailable) return;
		    if (isGrounded) {
			    if (jumpingInputValue) {
                    rb.velocity = new Vector3 (rb.velocity.x, jumpForce, rb.velocity.z);
                }
		    }
	    }
        
        /// <summary>
        /// Running state. Better use it in Update.
        /// </summary>
	    public virtual void Running () {
		    if (!isRunningAvaiable) return;
		    if (!isGrounded) return;
		    if (runningInputValue && endurance > 0.05f) {
                releaseAcceleration = false;
			    endurance -= Time.deltaTime * 2;
			    currentAcceleration = Mathf.MoveTowards (currentAcceleration, runningAcceleration, Time.deltaTime * 10);
		    }
		    else {
                releaseAcceleration = true;
			    if (System.Math.Abs(endurance - referenceEndurance) > epsilon) {
                    endurance = Mathf.MoveTowards (endurance, referenceEndurance, Time.deltaTime);
                }
		    }
	    }

        private void LookingForGround () {
            groungCheckPosition = new Vector3 (cc.transform.position.x, cc.transform.position.y - height / 2, cc.transform.position.z);
            if (Physics.CheckSphere (groungCheckPosition, 0.1f, groundMask, QueryTriggerInteraction.Ignore)) {
                isGrounded = true;
                if (!isLandingActionPerformed) {
                    isLandingActionPerformed = true;
                    landingAction?.Invoke ();
                }
                rb.drag = drag;
            }
            else {
                isGrounded = false;
                isLandingActionPerformed = false;
                rb.drag = 0.5f;
            }
        }

        private void MoveTorwardsAcceleration () {
            if (!releaseAcceleration) return;
            if (System.Math.Abs(currentAcceleration - referenceAcceleration) > epsilon) {
                currentAcceleration = Mathf.MoveTowards (currentAcceleration, referenceAcceleration, Time.deltaTime * 10);
            }
        }
    }
}