using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AFPC;
using Mirror;

[RequireComponent(typeof(EntityLifecycle))]
public class Entity : NetworkBehaviour
{
    // public Lifecycle lifecycle;
    public EntityLifecycle lifecycle;

    public Movement movement;

    private void Awake () {
        /* Initialize lifecycle and add Damage FX */
        // lifecycle.Initialize();

        lifecycle = GetComponent<EntityLifecycle>();
        
        // lifecycle.Initialize();

        /* Initialize movement and add camera shake when landing */
        movement.Initialize();
    }

    private void Update () {
        // В оригинале было в LateUpdate
        // if (!lifecycle.Availability()) return;

        /* Control the speed */
        movement.Running();

        /* Control the jumping, ground search... */
        movement.Jumping();

        /* Control the health and shield recovery */
        // lifecycle.Runtime();
    }

    private void FixedUpdate () {
        // if (!lifecycle.Availability()) return;

        /* Physical movement */
        movement.Accelerate();
    }
}
