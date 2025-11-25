/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AuroraFPSRuntime.AIModules.BehaviourTree.Variables
{
    [HideScriptField]
    public sealed class GlobalVariablesStorage : ScriptableObject
    {
        [SerializeField]
        private VariablesContainer variables = new VariablesContainer();

        public void AddVariable(string name, TreeVariable variable)
        {
            variables[name] = variable;
        }

        public void AddVariable<T>(string name, T variable) where T : TreeVariable
        {
            if (variable is T var)
            {
                variables[name] = var;
            }
        }

        public bool TryGetVariable(string name, out TreeVariable variable)
        {
            return variables.TryGetValue(name, out variable);
        }

        public bool TryGetVariable<T>(string name, out T variable) where T : TreeVariable
        {
            if (variables.TryGetValue(name, out TreeVariable value))
            {
                variable = value as T;
                if (variable != null)
                {
                    return true;
                }
            }

            variable = null;
            return false;
        }

        public TreeVariable GetVariable(string name)
        {
            if (variables.TryGetValue(name, out TreeVariable variable))
            {
                return variable;
            }

            return null;
        }

        public T GetVariable<T>(string name) where T : TreeVariable
        {
            if (variables.TryGetValue(name, out TreeVariable variable))
            {
                return variable as T;
            }

            return null;
        }

        public void RemoveVariable(string name)
        {
            variables.Remove(name);
        }

        public TreeVariable this[string key]
        {
            get
            {
                return variables[key];
            }

            set
            {
                variables[key] = value;
            }
        }

        public IEnumerable<KeyValuePair<string, TreeVariable>> Variables
        {
            get
            {
                foreach (var item in variables)
                {
                    yield return item;
                }
            }
        }


        #region [Static Members]
        private static GlobalVariablesStorage _Current;
        public static GlobalVariablesStorage Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = Resources.LoadAll<GlobalVariablesStorage>(string.Empty).FirstOrDefault();
                    Debug.Assert(_Current != null, string.Format("<b><color=#FF0000>Global Variables Storage not found!\nCreate or move the current Global Variables Storage to resources folder in your project.</color></b>"));
                }
                return _Current;
            }
        }
        #endregion
    }
}