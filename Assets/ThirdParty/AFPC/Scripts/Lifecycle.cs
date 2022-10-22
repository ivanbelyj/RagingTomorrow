using UnityEngine;
using UnityEngine.Events;

namespace AFPC {

    /// <summary>
    /// This class contain health-damage-death cycle.
    /// </summary>
    [System.Serializable]
    public class Lifecycle {

        public bool isDebugLog;

        public string ID = "AFPC";
        private bool isAvailable = true;

        [Header("Health")]
        private bool isHealthRecovery = true;
	    public float referenceHealth = 100.0f;
	    private float health = 1;
	    private int healthRecoveryRate = 60;
	
        [Header("Shield")]
        private bool isShieldRecovery = true;
	    public float referenceShield = 100.0f;
	    private float shield = 1;
	    private int shieldRecoveryRate = 60;

        private bool isFrenzy;
        private float frenzyThreshold = 20.0f;

        private float epsilon = 0.01f;
        private UnityAction healAction;
        private UnityAction damageAction;
        private UnityAction deathAction;

        /// <summary>
        /// Set maximum health and shield in the start.
        /// </summary>
        public virtual void Initialize () {
	        SetMaximumHealthAndShield();
        }

        /// <summary>
        /// Check the availability of this character.
        /// </summary>
        /// <returns></returns>
        public bool Availability() {
            return isAvailable;
        }

        /// <summary>
        /// Activate the character.
        /// </summary>
        public virtual void Activate () {
            isAvailable = true;
            if (isDebugLog) Debug.Log (ID + ": Active.");
        }

        /// <summary>
        /// Deactivate the character.
        /// </summary>
        public virtual void Deactivate () {
            isAvailable = false;
            if (isDebugLog) Debug.Log (ID + ": Not active.");
        }

        /// <summary>
        /// Restore the health and shield to the maximum.
        /// </summary>
        public virtual void SetMaximumHealthAndShield () {
            health = referenceHealth;
            shield = referenceShield;
            if (isDebugLog) Debug.Log (ID + ": Set Maximum Health and Shield.");
        }

        /// <summary>
        /// Drive the health and shield values to the 1.
        /// </summary>
        public virtual void SetMinimumHealthAndShield () {
            health = 1;
            shield = 1;
            if (isDebugLog) Debug.Log (ID + ": Set Minimum Health and Shield.");
        }

        /// <summary>
        /// Current health of the character.
        /// </summary>
        /// <returns></returns>
        public float GetHealthValue () {
            return health;
        }

        /// <summary>
        /// The health of the character will increase in 1 every "value" frames.
        /// </summary>
        /// <param name="value"></param>
        public void SetHealthRecoveryRate (int value) {
            healthRecoveryRate = value;
        }

        /// <summary>
        /// Allow this character to recover health.
        /// </summary>
        public virtual void AllowHealthRecovery () {
            isHealthRecovery = true;
            if (isDebugLog) Debug.Log (ID + ": Allow Health Recovery.");
        }

        /// <summary>
        /// Ban this character to recover health.
        /// </summary>
        public virtual void BanHealthRecovery () {
            isHealthRecovery = false;
            if (isDebugLog) Debug.Log (ID + ": Ban Health Recovery.");
        }

        /// <summary>
        /// Current shield of the character.
        /// </summary>
        /// <returns></returns>
        public float GetShieldValue () {
            return shield;
        }

        /// <summary>
        /// The shield of the character will increase in 1 every "value" frames.
        /// </summary>
        /// <param name="value"></param>
        public void SetShieldRecoveryRate (int value) {
            shieldRecoveryRate = value;
        }

        /// <summary>
        /// Allow this character to recover health.
        /// </summary>
        public virtual void AllowShieldRecovery () {
            isShieldRecovery = true;
            if (isDebugLog) Debug.Log (ID + ": Allow Shiled Recovery.");
        }

        /// <summary>
        /// Ban this character to recover health.
        /// </summary>
        public virtual void BanShieldRecovery () {
            isShieldRecovery = false;
            if (isDebugLog) Debug.Log (ID + ": Ban Shiled Recovery.");
        }

        /// <summary>
        /// Check the Frenzy state.
        /// The Frenzy state is used to give your users a special state when his health level is low.
        /// </summary>
        /// <returns></returns>
        public bool IsFrenzy () {
            return isFrenzy;
        }

        /// <summary>
        /// Set a minimum health threshold for the frenzy state.
        /// </summary>
        /// <param name="value"></param>
        public void SetFrenzyThreshold (float value) {
            frenzyThreshold = value;
            if (isDebugLog) Debug.Log (ID + ": Frenzy threshold is: " + value);
        }

        /// <summary>
        /// Recovering health and shield.
        /// </summary>
	    public virtual void Runtime () {
		    HealthRecovery ();
		    ShieldRecovery ();
	    }

	    private void HealthRecovery () {
		    if (!isHealthRecovery) return;
		    if (healthRecoveryRate == 0) return;
		    if (Time.frameCount % healthRecoveryRate == 0 && Mathf.Abs(health - referenceHealth) > epsilon) {
			    if (health < referenceHealth) {
				    if (!isHealthRecovery) return;
				    health += 1;
				    CheckFrenzy ();
			    }
			    else {
				    health = referenceShield;
			    }
		    }
	    }

	    private void ShieldRecovery () {
		    if (!isShieldRecovery) return;
		    if (shieldRecoveryRate == 0) return;
		    if (Time.frameCount % shieldRecoveryRate == 0 && Mathf.Abs(shield - referenceShield) > epsilon) {
			    if (shield < referenceShield) {
				    if (!isShieldRecovery) return;
				    shield += 1;
			    }
			    else {
				    shield = referenceShield;
			    }
		    }
	    }

        /// <summary>
        /// Damage the character. The shield will be damaged first.
        /// </summary>
        /// <param name="value"></param>
        public virtual void Damage (float value) {
            if (!isAvailable) return;
            float shieldDamage = Mathf.Min (shield, value);
            float healthDamage = Mathf.Min (health, value - shieldDamage);
            shield -= shieldDamage;
            health -= healthDamage;
            if (Mathf.Abs(health) < epsilon) {
                Death ();
            }
            damageAction?.Invoke();
            if (isDebugLog) Debug.Log (ID + ": Damaged: " + value);
        }

        /// <summary>
        /// Perform an action when the character was damaged.
        /// </summary>
        /// <param name="action"></param>
        public void AssignDamageAction (UnityAction action) {
            damageAction = action;
        }

        /// <summary>
        /// Heal the character.
        /// </summary>
        /// <param name="value"></param>
        public virtual void Heal (float value) {
            if (!isAvailable) return;
            float healthHeal = Mathf.Min (referenceHealth - health, value);
            float shieldHeal = Mathf.Min (referenceShield - shield, value - healthHeal);
            health += healthHeal;
            shield += shieldHeal;
            healAction?.Invoke();
            if (isDebugLog) Debug.Log (ID + ": Healed: " + value);
        }

        /// <summary>
        /// Perform an action when the character was healed.
        /// </summary>
        /// <param name="action"></param>
        public void AssignHealAction (UnityAction action) {
            healAction = action;
        }
        
	    private bool CheckFrenzy () {
		    isFrenzy = false;
            isFrenzy |= health < (referenceHealth / 100) * frenzyThreshold;
		    return isFrenzy;
	    }

        /// <summary>
        /// Activate the character and restore health and shield.
        /// </summary>
        public virtual void Respawn() {
            if (isAvailable) return;
            Activate();
            AllowHealthRecovery();
            AllowShieldRecovery();
            SetMaximumHealthAndShield();
            CheckFrenzy ();
            if (isDebugLog) Debug.Log (ID + ": Respawn");
        }

        /// <summary>
        /// Deactivate the character and set health and shield to the minimum.
        /// </summary>
        public virtual void Death () {
            if (!isAvailable) return;
            Deactivate();
		    isFrenzy = false;
		    BanHealthRecovery();
            BanShieldRecovery();
		    SetMinimumHealthAndShield();
            deathAction?.Invoke();
            if (isDebugLog) Debug.Log (ID + ": Death");
        }

        /// <summary>
        /// Perform an action when the character dies.
        /// </summary>
        /// <param name="action"></param>
        public void AssignDeathAction (UnityAction action) {
            deathAction = action;
        }
    }
}
