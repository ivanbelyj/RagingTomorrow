/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.AIModules.BehaviourTree.Attributes;
using AuroraFPSRuntime.AIModules.BehaviourTree.Variables;
using AuroraFPSRuntime.Attributes;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Compare", "Conditions/Compare")]
    [HideScriptField]
    public class CompareNode : ActionNode
    {
        private enum ComparisonOperator
        {
            Equal,
            NotEqual,
            Less,
            LessOrEqual,
            More,
            MoreOrEqual
        }

        [SerializeField]
        [TreeVariable(typeof(int), typeof(float))]
        private string value1Variable;

        [SerializeField]
        private ComparisonOperator comparisonOperator;

        [SerializeField]
        private float value2;

        protected override State OnUpdate()
        {
            if (TryGetVariableValue(out float value))
            {
                switch (comparisonOperator)
                {
                    case ComparisonOperator.Equal:
                        if (value == value2)
                        {
                            return State.Success;
                        }
                        break;
                    case ComparisonOperator.NotEqual:
                        if (value != value2)
                        {
                            return State.Success;
                        }
                        break;
                    case ComparisonOperator.Less:
                        if (value < value2)
                        {
                            return State.Success;
                        }
                        break;
                    case ComparisonOperator.LessOrEqual:
                        if (value <= value2)
                        {
                            return State.Success;
                        }
                        break;
                    case ComparisonOperator.More:
                        if (value > value2)
                        {
                            return State.Success;
                        }
                        break;
                    case ComparisonOperator.MoreOrEqual:
                        if (value >= value2)
                        {
                            return State.Success;
                        }
                        break;
                }
            }

            return State.Failure;
        }

        private bool TryGetVariableValue(out float value)
        {
            if (tree.TryGetVariable(value1Variable, out TreeVariable variable))
            {
                IntVariable intVariable = variable as IntVariable;
                if (intVariable != null)
                {
                    value = intVariable;
                    return true;
                }

                FloatVariable floatVariable = variable as FloatVariable;
                if (floatVariable != null)
                {
                    value = floatVariable;
                    return true;
                }
            }

            value = 0;
            return false;
        }
    }
}