/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using UnityEngine;

namespace AuroraFPSRuntime.CoreModules
{
    /// <summary>
    /// Layer name conventions of the all Aurora FPS Engine layers.
    /// </summary>
    public static class LNC
    {

        public const string Player = "Player";
        public const string Weapon = "Weapon";
        public const string AI = "AI";
        public const string AIFriendly = "AI Friendly";
        public const string AIEnemy = "AI Enemy";
        public const string RemoteBody = "Remote Body";

        public readonly static LayerMask IgnorePlayer = ~(1 << LayerMask.NameToLayer(Player));

        public readonly static string[] AllLayers = new string[] { Player, Weapon, AI, AIFriendly, AIEnemy, RemoteBody };

        /// <summary>
        /// All layer array in representation "{Layer Name}: {Index}".
        /// </summary>
        public readonly static string[] LayersWithIndex = new string[]
        {
            LayerIndex(21, Player),
            LayerIndex(22, Weapon),
            LayerIndex(23, AI),
            LayerIndex(24, AIFriendly),
            LayerIndex(25, AIEnemy),
            LayerIndex(26, RemoteBody),
        };

        private static string LayerIndex(int index, string layer)
        {
            return string.Format("{0}: {1}", layer, index);
        }
    }
}