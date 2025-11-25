/* ==================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================== */

using UnityEditor;
using UnityEngine;
using AuroraFPSEditor.Attributes;

namespace AuroraFPSEditor
{
    internal static class ControllerMenu
    {
        private const string CharacterControllerLocation = "Base Content/Core/Editor/Editor Resources/Templates/Controller/Character Controller.prefab";
        private const string RigidbodyControllerLocation = "Base Content/Core/Editor/Editor Resources/Templates/Controller/Rigidbody Controller.prefab";

        [MenuItem("Aurora FPS Engine/Create/Controller/Based on Character Controller", false, 101)]
        [MenuItem("GameObject/Aurora FPS Engine/Controller/Based on Character Controller", false, 0)]
        private static void CreateBasedOnCharacterController()
        {

            Object controller = AssetDatabase.LoadAssetAtPath<Object>(System.IO.Path.Combine(ApexSettings.Current.GetRootPath(), CharacterControllerLocation));
            if(controller != null)
            {
                Object controllerClone = Object.Instantiate(controller, Vector3.zero, Quaternion.identity);
                controllerClone.name = "New Player Based on Character Controller";
                EditorGUIUtility.PingObject(controllerClone);
            }
            else
            {
                Debug.LogError($"Controller cannot be created, because of location (Path: {CharacterControllerLocation}) with template is empty!");
            }
        }

        [MenuItem("Aurora FPS Engine/Create/Controller/Based on Rigidbody", false, 102)]
        [MenuItem("GameObject/Aurora FPS Engine/Controller/Based on Rigidbody", false, 0)]
        private static void CreateBasedOnRigidbodyController()
        {
            Object controller = AssetDatabase.LoadAssetAtPath<Object>(System.IO.Path.Combine(ApexSettings.Current.GetRootPath(), RigidbodyControllerLocation));
            if (controller != null)
            {
                Object controllerClone = Object.Instantiate(controller, Vector3.zero, Quaternion.identity);
                controllerClone.name = "New Player Based on Rigidbody Controller";
                EditorGUIUtility.PingObject(controllerClone);
            }
            else
            {
                Debug.LogError($"Controller cannot be created, because of location (Path: {RigidbodyControllerLocation}) with template is empty!");
            }
        }
    }
}