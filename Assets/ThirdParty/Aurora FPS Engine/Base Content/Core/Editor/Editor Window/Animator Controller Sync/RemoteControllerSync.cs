/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.SystemModules.ControllerSystems;
using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace AuroraFPSEditor.Utilities
{
    public sealed class RemoteControllerSync : EditorWindow
    {
        private const float SyncRatio = 1.222222222f;

        public readonly static Vector2 WindowSize = new Vector2(375, 70);

        private PlayerController controller;
        private AnimatorController animatorController;

        private void OnGUI()
        {
            controller = (PlayerController)EditorGUILayout.ObjectField("Controller", controller, typeof(PlayerController), true);
            animatorController = (AnimatorController)EditorGUILayout.ObjectField("Animator Controller", animatorController, typeof(AnimatorController), true);

            EditorGUI.BeginDisabledGroup(controller == null || animatorController == null);
            Rect lastPosition = GUILayoutUtility.GetRect(0, 20);
            Rect buttonPosition = new Rect(300, lastPosition.y + 2.5f, 70, 20);
            if (GUI.Button(buttonPosition, "Sync"))
            {
                ExecuteSyncing(controller, animatorController);
                EditorUtility.DisplayDialog("Remote Controller Sync", "Success!", "Continue");
                Close();
            }
            EditorGUI.EndDisabledGroup();
        }

        public static void ExecuteSyncing(PlayerController controller, AnimatorController animatorController)
        {
            AnimatorStateMachine rootState = animatorController.layers[0].stateMachine.stateMachines[0].stateMachine;
            AnimatorState blendTreeState = rootState.states[0].state;
            BlendTree blendTree = (BlendTree)blendTreeState.motion;
            ChildMotion[] childMotions = blendTree.children;

            float speedValue = (float)Math.Round(controller.GetWalkSpeed() / SyncRatio, 2);
            childMotions[0].position = new Vector2(0, 0);
            childMotions[1].position = new Vector2(0, speedValue);
            childMotions[2].position = new Vector2(-speedValue, speedValue);
            childMotions[3].position = new Vector2(speedValue, speedValue);
            childMotions[4].position = new Vector2(0, -speedValue);
            childMotions[5].position = new Vector2(-speedValue, -speedValue);
            childMotions[6].position = new Vector2(speedValue, -speedValue);
            childMotions[7].position = new Vector2(-speedValue, 0);
            childMotions[8].position = new Vector2(speedValue, 0);

            speedValue = (float)Math.Round(controller.GetRunSpeed() / SyncRatio, 2);
            childMotions[9].position = new Vector2(0, speedValue);
            childMotions[10].position = new Vector2(-speedValue, speedValue);
            childMotions[11].position = new Vector2(speedValue, speedValue);
            childMotions[12].position = new Vector2(0, -speedValue);
            childMotions[13].position = new Vector2(-speedValue, -speedValue);
            childMotions[14].position = new Vector2(speedValue, -speedValue);
            childMotions[15].position = new Vector2(-speedValue, 0);
            childMotions[16].position = new Vector2(speedValue, 0);

            speedValue = (float)Math.Round(controller.GetSprintSpeed() / SyncRatio, 2);
            childMotions[17].position = new Vector2(0, speedValue);
            childMotions[18].position = new Vector2(-speedValue, speedValue);
            childMotions[19].position = new Vector2(speedValue, speedValue);
            childMotions[20].position = new Vector2(0, -speedValue);
            childMotions[21].position = new Vector2(-speedValue, -speedValue);
            childMotions[22].position = new Vector2(speedValue, -speedValue);
            childMotions[23].position = new Vector2(-speedValue, 0);
            childMotions[24].position = new Vector2(speedValue, 0);

            blendTree.children = childMotions;

            blendTreeState = rootState.states[1].state;
            blendTree = (BlendTree)blendTreeState.motion;
            childMotions = blendTree.children;

            speedValue = (float)Math.Round(controller.GetCrouchSpeed() / SyncRatio, 2);
            childMotions[0].position = new Vector2(0, 0);
            childMotions[1].position = new Vector2(0, speedValue);
            childMotions[2].position = new Vector2(-speedValue, speedValue);
            childMotions[3].position = new Vector2(speedValue, speedValue);
            childMotions[4].position = new Vector2(0, -speedValue);
            childMotions[5].position = new Vector2(-speedValue, -speedValue);
            childMotions[6].position = new Vector2(speedValue, -speedValue);
            childMotions[7].position = new Vector2(-speedValue, 0);
            childMotions[8].position = new Vector2(speedValue, 0);

            blendTree.children = childMotions;
        }

        [MenuItem("Aurora FPS Engine/Utilities/Remote Controller Sync", priority = 307)]
        public static void Open()
        {
            RemoteControllerSync window = EditorWindow.GetWindow<RemoteControllerSync>(true, "Remote Controller Sync", true);
            window.minSize = RemoteControllerSync.WindowSize;
            window.maxSize = RemoteControllerSync.WindowSize;
            window.Show();
            EditorUtility.DisplayDialog("Remote Controller Sync", "Please note: This utility only works with the standard Animator Controller for remote body.", "Continue");
        }
    }
}