using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour//, IDamageable
{
    [SerializeField]
    private DamageableBody body;

    private void OnCollisionEnter(Collision other)
    {
        Damage(new DamageData() { DamageType = DamageType.Punch });
    }

    public void Damage(DamageData damage)
    {
        Debug.Log("Body part is damaged. Added effect");
        var damageEffect = new LifecycleEffect()
        {
            duration = 1,
            speed = 1,
            targetParameter = LifecycleParameterEnum.Health
        };
        body.Lifecycle.AddEffect(damageEffect);
    }
}
