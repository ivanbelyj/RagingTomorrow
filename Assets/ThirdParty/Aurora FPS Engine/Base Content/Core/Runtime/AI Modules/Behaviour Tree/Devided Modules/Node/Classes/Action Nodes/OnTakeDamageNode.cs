using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree.Variables;
using AuroraFPSRuntime.Attributes;
using AuroraFPSRuntime.SystemModules.HealthModules;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("On Take Damage", "Conditions/On Take Damage")]
    [HideScriptField]
    public class OnTakeDamageNode : ActionNode
    {
        [SerializeField]
        [TreeVariable(typeof(bool))]
        private string storedVariable;

        [SerializeField]
        [TreeVariable(typeof(Transform))]
        private string senterVariable;

        [SerializeField]
        [TreeVariable(typeof(Vector3))]
        private string damagePointVariable;

        [SerializeField]
        [TreeVariable(typeof(float))]
        private string healthVariable;

        // Stored required components.
        private ObjectHealth objectHealth;

        // Stored required properties.
        private DamageInfo damageInfo;
        private bool takenDamage;

        protected override void OnInitialize()
        {
            objectHealth = owner.GetComponent<ObjectHealth>();
            objectHealth.OnTakeDamageCallback += OnTakeDamageCallback;
        }

        protected override State OnUpdate()
        {
            if (takenDamage)
            {
                takenDamage = false;

                if (tree.TryGetVariable<BoolVariable>(storedVariable, out BoolVariable boolVariable))
                {
                    boolVariable.SetValue(true);
                }

                if (tree.TryGetVariable<TransformVariable>(senterVariable, out TransformVariable transformVariable))
                {
                    transformVariable.SetValue(damageInfo.sender);
                }

                if (tree.TryGetVariable<Vector3Variable>(damagePointVariable, out Vector3Variable vector3Variable))
                {
                    vector3Variable.SetValue(damageInfo.sender.position);
                }

                if (tree.TryGetVariable<FloatVariable>(healthVariable, out FloatVariable floatVariable))
                {
                    float health = owner.GetComponent<ObjectHealth>().GetHealth();
                    floatVariable.SetValue(health);
                }

                return State.Success;
            }

            return State.Failure;
        }

        private void OnTakeDamageCallback(float amount, DamageInfo damageInfo)
        {
            this.damageInfo = damageInfo;
            takenDamage = true;
        }
    }
}