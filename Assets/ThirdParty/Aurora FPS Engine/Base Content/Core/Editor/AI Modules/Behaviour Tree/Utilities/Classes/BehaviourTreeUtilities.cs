/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Davleev Zinnur
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace AuroraFPSEditor.AIModules.BehaviourTree
{
    public static class BehaviourTreeUtilities
    {
        public static T[] FindObjectsOfType<T>() where T : UnityEngine.Object
        {
            string[] allDirectories = GetAllDirectories();
            List<T> foundObjects = new List<T>();
            for (int i = 0; i < allDirectories.Length; i++)
            {
                string[] paths = Directory.GetFiles(allDirectories[i]);
                for (int j = 0; j < paths.Length; j++)
                {
                    if (Path.GetExtension(paths[j]) == ".asset")
                    {
                        T found = AssetDatabase.LoadAssetAtPath<T>(paths[j]);
                        if (found != null)
                        {
                            foundObjects.Add(found);
                        }
                    }
                }
            }

            return foundObjects.ToArray();
        }

        public static string[] GetAllDirectories()
        {
            List<string> allDirectories = new List<string>();
            Queue<string> queue = new Queue<string>();
            queue.Enqueue("Assets");

            while (queue.Count > 0)
            {
                string path = queue.Dequeue();

                string[] directories = Directory.GetDirectories(path);
                for (int i = 0; i < directories.Length; i++)
                {
                    queue.Enqueue(directories[i]);
                }

                allDirectories.Add(path);
            }

            return allDirectories.ToArray();
        }
    }
}