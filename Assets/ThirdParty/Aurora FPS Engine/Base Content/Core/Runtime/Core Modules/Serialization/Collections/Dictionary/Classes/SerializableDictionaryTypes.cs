///* ================================================================
//   ----------------------------------------------------------------
//   Project   :   Aurora FPS Engine
//   Publisher :   Renowned Games
//   Developer :   Tamerlan Shakirov
//   ----------------------------------------------------------------
//   Copyright 2022 Renowned Games All rights reserved.
//   ================================================================ */

//using AuroraFPSRuntime.AIModules;
//using AuroraFPSRuntime.SystemModules;
//using System.Collections.Generic;
//using UnityEngine;

//namespace AuroraFPSRuntime.CoreModules.Serialization.Collections
//{
//    #region [Represents Of Serialized Dictionary Classes]
//    /// <summary>
//    /// Represents base class for serialized Dictionary(string, string).
//    /// </summary>
//    [System.Serializable]
//    public class DictionaryStringToString : SerializableDictionary<string, string>
//    {

//    }

//    /// <summary>
//    /// Represents base class for serialized Dictionary(string, KeyCode).
//    /// </summary>
//    [System.Serializable]
//    public class DictionaryStringToKeyCode : SerializableDictionary<string, KeyCode>
//    {

//    }

//    /// <summary>
//    /// Represents base class for serialized Dictionary(Object, AudioClipArrayStorage).
//    /// </summary>
//    [System.Serializable]
//    public class DictionaryObjectToArrayAudioClip : SerializableDictionary<Object, AudioClipStorage>
//    {

//    }

//    /// <summary>
//    /// Represents base class for serialized Dictionary(string, PoolContainer).
//    /// </summary>
//    [System.Serializable]
//    public class DictionaryStringToPoolContainer : SerializableDictionary<string, PoolContainer>
//    {

//    }


//    /// <summary>
//    /// Represents base class for serialized Dictionary(string, AIBehaviour).
//    /// </summary>
//    [System.Serializable]
//    public class DictionaryStringToAIBehaviour : SerializableDictionary<string, AIBehaviour>
//    {

//    }

  
//    #endregion

//    #region [Represents Of Serialized Storage Classes]
//    /// <summary>
//    /// Represents base class for serialized array storage AudioClip.
//    /// </summary>
//    [System.Serializable]
//    public class AudioClipStorage : SerializationStorage<AudioClip[]> { }

//    #endregion
//}