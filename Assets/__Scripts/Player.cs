using System.Collections;
using System.Collections.Generic;
using AFPC;
using UnityEngine;
using Mirror;
using TMPro;

[RequireComponent(typeof(Entity))]
public class Player : NetworkBehaviour
{
    [Header("Set in inspector")]
    public Entity entity;

    /* Overview class. Look, Aim, Shake... */
    public Overview overview;
    public GameObject floatingInfo;
    public TextMeshPro playerNameText;
    public TextMeshPro playerHealthText;

    [Header("Set dynamically")]
    public SimpleHUD HUD;

    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    private string playerName;

    private void Start() {
        /* a few apllication settings for more smooth. This is Optional. */
        QualitySettings.vSyncCount = 0;
        Cursor.lockState = CursorLockMode.Locked;

        /* Initialize lifecycle and add Damage FX */
        // lifecycle.Initialize();
        // entity.lifecycle.AssignDamageAction(DamageFX);

        /* Initialize movement and add camera shake when landing */
        // movement.Initialize();
        entity.movement.AssignLandingAction(() => overview.Shake(0.5f));

        BindHealthFloatingText();
    }

    private void BindHealthFloatingText() {
        SetHealthText(entity.lifecycle.health.Value);
        entity.lifecycle.health.OnValueChanged += (old, newVal) => {
            SetHealthText(newVal);
        };
        void SetHealthText(float val) {
            playerHealthText.text = string.Format("{0:F2}", System.Math.Round(val, 2));
        }
    }

    private void BindUI() {
        HUD = FindObjectOfType<SimpleHUD>();
        if (HUD is not null) {
            HUD.SetEntity(gameObject);
            Debug.Log("HUD is set");
        }
    }

    public override void OnStartLocalPlayer()
    {
        overview.camera = Camera.main;

        playerName = $"Player {Random.Range(100, 999)}";
        CmdSetupPlayer(playerName);

        BindUI();
    }

    private void Update() {
        if (!isLocalPlayer) {
            floatingInfo.transform.LookAt(Camera.main.transform);
            return;
        }

        if (!hasAuthority)
            return;

        /* Read player input before check availability */
        ReadInput();

        /* Block controller when unavailable */
        // if (!entity.lifecycle.Availability()) return;

        /* Mouse look state */
        overview.Looking();

        /* Change camera FOV state */
        overview.Aiming();

        /* Shake camera state. Required "physical camera" mode on */
        overview.Shaking();

        /* Control the speed */
        // movement.Running();

        /* Control the jumping, ground search... */
        // movement.Jumping();

        /* Control the health and shield recovery */
        // lifecycle.Runtime();
    }

    private void FixedUpdate() {

        /* Block controller when unavailable */
        // if (!entity.lifecycle.Availability()) return;

        /* Physical rotation with camera */
        overview.RotateRigigbodyToLookDirection(entity.movement.rb);
    }

    private void LateUpdate() {

        /* Block controller when unavailable */
        // if (!entity.lifecycle.Availability()) return;

        /* Camera following */
        overview.Follow(transform.position);
    }

    private void ReadInput() {
        // if (Input.GetKeyDown (KeyCode.R)) entity.lifecycle.Damage(50);
        // if (Input.GetKeyDown (KeyCode.H)) entity.lifecycle.Heal(50);
        // if (Input.GetKeyDown (KeyCode.T)) entity.lifecycle.Respawn();
        overview.lookingInputValues.x = Input.GetAxis("Mouse X");
        overview.lookingInputValues.y = Input.GetAxis("Mouse Y");

        overview.aimingInputValue = Input.GetMouseButton(1);
        entity.movement.movementInputValues.x = Input.GetAxis("Horizontal");
        entity.movement.movementInputValues.y = Input.GetAxis("Vertical");
        entity.movement.jumpingInputValue = Input.GetButtonDown("Jump");
        entity.movement.runningInputValue = Input.GetKey(KeyCode.LeftShift);

        LifecycleEffect damage = new LifecycleEffect(-1f, 10) {
            targetParameterIndex = 0  // health
        };

        if (Input.GetKeyDown(KeyCode.H)) {
            entity.lifecycle.CmdAddEffect(damage);
        }
    }

    private void DamageFX() {
        overview.Shake(0.75f);
    }

    #region Sync
    private void OnPlayerNameChanged(string oldName, string newName) {
        playerNameText.text = newName;
    }

    [Command]
    private void CmdSetupPlayer(string name) {
        playerName = name;
    }
    #endregion
}
