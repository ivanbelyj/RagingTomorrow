using UnityEngine;

namespace AFPC {

    /// <summary>
    /// This class allows the user to look around and perform some POV effects.
    /// </summary>
    [System.Serializable]
    public class Overview {

        public bool isDebugLog;

        [Header("Inputs")]
        public Vector2 lookingInputValues;
        public bool aimingInputValue;

        [Header("Following")]
        public bool isFollowingInstant = true;
        public float damping = 10.0f;
        public Vector3 cameraOffset = new Vector3(0,0.8f,0);
	
        [Header ("Looking")]
        public bool isHorizontalInverted;
	    public bool isVerticalInverted = true;
	    private bool isLookingAvaialbe = true;
	    public float sensitivity = 4.0f;
	    public float horizontalRange, verticalRange = 50.0f;
        private Vector3 targetRotation;
	
        [Header ("Search")]
	    public LayerMask searchMask = 0;
        public float searсhDistance = 1;
	
        [Header ("Aiming")]
        public float defaultFOV = 80.0f;
	    public float aimingFOV = 40.0f;
	    private bool isAimingAvailable = true;
	
        [Header ("Shaking")]
        public float referenceShakingAmount;
	    private bool isShakingAvaiable = true;
	    private float shakingAmount;

        [Header("References")]
        public Camera camera;

        /// <summary>
        /// Allow the controller to read looking input values and rotate the camera.
        /// By default for common FPS games.
        /// </summary>
        public virtual void AllowLooking () {
            isLookingAvaialbe = true;
            if (isDebugLog) Debug.Log (camera.gameObject.name + ": Allow Looking");
        }

        /// <summary>
        /// Ban controller to read looking input values and rotate camera.
        /// Use this if you want to block the user's ability to look around.
        /// </summary>
        public virtual void BanLooking () {
            isLookingAvaialbe = false;
            if (isDebugLog) Debug.Log (camera.gameObject.name + ": Ban Looking");
        }

        /// <summary>
        /// Allow the user to change camera FOV to view far objects.
        /// </summary>
        public virtual void AllowAiming () {
            isAimingAvailable = true;
            if (isDebugLog) Debug.Log (camera.gameObject.name + ": Allow Aiming");
        }

        /// <summary>
        /// Ban the user to change camera FOV to view far objects.
        /// The camera FOV value moves forward to the "default FOV" value.
        /// </summary>
        public virtual void BanAiming () {
            isAimingAvailable = false;
            if (isDebugLog) Debug.Log (camera.gameObject.name + ": Ban Aiming");
        }

        /// <summary>
        /// Allow camera shaking by lens shifting. Required "Physical camera" mode on.
        /// </summary>
        public virtual void AllowShaking () {
            isShakingAvaiable = true;
            if (isDebugLog) Debug.Log (camera.gameObject.name + ": Allow Shaking");
        }

        /// <summary>
        /// Ban camera shaking by lens shifting.
        /// </summary>
        public virtual void BanShaking () {
            isShakingAvaiable = false;
            if (isDebugLog) Debug.Log (camera.gameObject.name + ": Ban Shaking");
        }

        /// <summary>
        /// Follow the camera to the controller with offset.
        /// </summary>
        /// <param name="target"></param>
        public void Follow (Vector3 target) {
            if (!camera) return;
            if (isFollowingInstant) {
                camera.transform.position = target + cameraOffset;
            }
            else {
                camera.transform.position = Vector3.MoveTowards (camera.transform.position, target + cameraOffset, (damping * 10) * Time.deltaTime);
            }
        }

        /// <summary>
        /// Rotate the camera with looking input values.
        /// Using it as a "Mouse look" in common cases.
        /// </summary>
        public virtual void Looking () {
		    if (!camera) return;
		    if (!isLookingAvaialbe) return;
            if (System.Math.Abs(lookingInputValues.x) < Physics.sleepThreshold & System.Math.Abs(lookingInputValues.y) < Physics.sleepThreshold) return;
		    if (isHorizontalInverted) lookingInputValues.x *= -1;
		    if (isVerticalInverted) lookingInputValues.y *= -1;

            targetRotation.x += lookingInputValues.y * sensitivity * Time.deltaTime;
            targetRotation.y += lookingInputValues.x * sensitivity * Time.deltaTime;
		    targetRotation.y = Mathf.Repeat (targetRotation.y, 360);

            if (System.Math.Abs(horizontalRange) > Physics.sleepThreshold) {
			    targetRotation.y = Mathf.Clamp (targetRotation.y, -horizontalRange, horizontalRange);
		    }
		    if (System.Math.Abs(verticalRange) > Physics.sleepThreshold) {
			    targetRotation.x = Mathf.Clamp (targetRotation.x, -verticalRange, verticalRange);
		    }
		    camera.transform.rotation = Quaternion.Euler (targetRotation);
	    }

        /// <summary>
        /// Changing the camera FOV value or return to the default FOV value;
        /// </summary>
        public virtual void Aiming () {
            if (!isAimingAvailable) return;
            if (!camera) return;
		    if (aimingInputValue) {
			    camera.fieldOfView = Mathf.MoveTowards (camera.fieldOfView, aimingFOV, 10);
		    }
		    else if (System.Math.Abs(camera.fieldOfView - defaultFOV) > Physics.sleepThreshold) {
			    camera.fieldOfView = Mathf.MoveTowards (camera.fieldOfView, defaultFOV, 10);
		    }
	    }

        /// <summary>
        /// Raycast in the forward direction to search some objects.
        /// Good practice to use it for shooting or interaction.
        /// </summary>
        /// <returns></returns>
        public GameObject Search () {
            if (Physics.Raycast(camera.transform.position + (camera.transform.forward * 0.5f), camera.transform.forward, out RaycastHit hit, searсhDistance, searchMask)) {
                if (isDebugLog) Debug.Log ("GameObject found: " + hit.collider.gameObject.name);
                return hit.collider.gameObject;
            }
            return null;
	    }

        /// <summary>
        /// Control the camera lens shift values.
        /// </summary>
	    public virtual void Shaking () {
		    if (!isShakingAvaiable) return;
		    if (System.Math.Abs(shakingAmount - referenceShakingAmount) > Physics.sleepThreshold) {
			    shakingAmount = Mathf.MoveTowards (shakingAmount, referenceShakingAmount, Time.deltaTime);
		    }
		    if (System.Math.Abs(shakingAmount) > Physics.sleepThreshold) {
			    camera.lensShift = Vector2.Lerp (Vector2.zero, AddRandromSphereVector (camera.lensShift, shakingAmount), Time.deltaTime);
		    }
	    }

        /// <summary>
        /// Shake the camera lens with value.
        /// </summary>
        /// <param name="value"></param>
        public virtual void Shake (float value) {
            shakingAmount = value;
            if (isDebugLog) Debug.Log (camera.gameObject.name + ": Shake camera with: " + value + " value.");
        }

        private Vector3 AddRandromSphereVector (Vector3 position, float amount) {
	        return position += Random.insideUnitSphere * amount;
	    }

        /// <summary>
        /// Rotate rigidbody to looking direction.
        /// </summary>
        /// <param name="rb"></param>
        public void RotateRigigbodyToLookDirection (Rigidbody rb) {
            if (!camera) return;
            rb.rotation = Quaternion.Euler (0, camera.transform.rotation.eulerAngles.y, 0);
        }
    }
}
