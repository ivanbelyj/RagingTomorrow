using System.Collections;
using System.Collections.Generic;
using AFPC;
using UnityEngine;

[RequireComponent(typeof(EntityLifecycle))]
public class EntityMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private Movement movement;

    private EntityLifecycle lifecycle;

    private void Awake() {
        lifecycle = GetComponent<EntityLifecycle>();

        movement.Initialize();
    }

    private void Update() {
        if (!lifecycle.IsAlive)
            return;

        // Контролирует скорость
        movement.Running();

        // Контролирует прыжок, поиск земли...
        movement.Jumping();
    }

    private void FixedUpdate() {
        if (!lifecycle.IsAlive)
            return;
        
        movement.Accelerate();
    }

    public void SetInputValues(Vector3 val) {
        movement.movementInputValues = val;
    }

    public void SetJumpingInputValue(bool val) {
        movement.jumpingInputValue = val;
    }

    public void SetRunningInputValue(bool val) {
        bool isChanged = val != movement.runningInputValue;
        movement.runningInputValue = val;
        if (isChanged) {
            if (val)
                lifecycle.Run();
            else
                lifecycle.StopRun();
        }
        
    }
}
