using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityLifecycle))]
public class DamageableBody : MonoBehaviour
{
    private EntityLifecycle lifecycle;
    public EntityLifecycle Lifecycle => lifecycle;
    private void Awake() {
        this.lifecycle = GetComponent<EntityLifecycle>();
    }
}
