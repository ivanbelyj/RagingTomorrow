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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Nodes
{
    [TreeNodeContent("Random Index", "Actions/Unity/Random Index")]
    [HideScriptField]
    public class RandomIndexNode : ActionNode
    {
        [SerializeField]
        [TreeVariable(typeof(List<>))]
        private string listVariable;

        [SerializeField]
        [TreeVariable(Variable = "listVariable")]
        private string elementVariable;

        protected override State OnUpdate()
        {
            object element = null;

            if (tree.TryGetVariable(listVariable, out TreeVariable variable1))
            {
                object valueObject = variable1.GetValueObject();
                IList list = valueObject as IList;
                if (list != null)
                {
                    ICollection collection = valueObject as ICollection;
                    if (collection != null)
                    {
                        int randomInt = Random.Range(0, collection.Count);
                        element = list[randomInt];
                    }
                }
            }

            if (element != null)
            {
                if (tree.TryGetVariable(elementVariable, out TreeVariable variable2))
                {
                    if (variable2.GetVariableType() == element.GetType())
                    {
                        variable2.SetValueObject(element);
                        return State.Success;
                    }
                    Debug.Assert(variable2.GetValueObject().GetType() == element.GetType(), $"<b>Variables do not match by type <i><color=#FF0000>{listVariable}</color></i> and <i><color=#FF0000>{elementVariable}</color></i>.</b>");
                }
            }

            return State.Failure;
        }
    }
}