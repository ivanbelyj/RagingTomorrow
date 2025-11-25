/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using UnityEngine;

namespace AuroraFPSRuntime.CoreModules.Serialization.Collections
{
    #region [Represents of Serialized Stack classes]
    /// <summary>
    /// Represents base class for serialized Stack(GameObject).
    /// </summary>
    [System.Serializable]
    public class StackGameObject : SerializableStack<GameObject>
    {
        /// <summary>
        /// Initializes a new instance of the serializable Stack(GameObject) class that is empty and has the default initial capacity.
        /// </summary>
        public StackGameObject() : base() { }

        /// <summary>
        /// Initializes a new instance of the serializable Stack(GameObject) class that contains elements copied from the specified collection
        /// and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">Specified collection will copied to stack.</param>
        public StackGameObject(IEnumerable<GameObject> collection) : base(collection) { }

        /// <summary>
        /// Initializes a new instance of the serializable Stack(GameObject) class that is empty and has the specified initial capacity
        /// or the default initial capacity, whichever is greater.
        /// </summary>
        /// <param name="capacity">Initial stack capacity.</param>
        public StackGameObject(int capacity) : base(capacity) { }
    }
    #endregion

}