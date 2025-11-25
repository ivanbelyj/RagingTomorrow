/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.SystemModules;
using System.IO;
using UnityEditor;
using UnityEngine;

sealed class BeforeCreateAssetProcessor : UnityEditor.AssetModificationProcessor
{
    static void OnWillCreateAsset(string assetName)
    {
        string extension = Path.GetExtension(assetName);
        if (extension == ".prefab")
        {
            EditorApplication.delayCall += () => UpdatePoolObjectID(assetName);
        }
    }

    static void UpdatePoolObjectID(string assetName)
    {
        GameObject gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetName);
        if (gameObject != null && gameObject.TryGetComponent<PoolObject>(out PoolObject poolObject))
        {
            poolObject.GenerateGUID(16);
            EditorUtility.SetDirty(poolObject);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
