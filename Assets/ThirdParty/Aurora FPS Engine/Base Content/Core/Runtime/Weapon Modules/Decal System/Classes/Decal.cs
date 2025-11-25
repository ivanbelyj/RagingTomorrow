/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules;
using AuroraFPSRuntime.SystemModules;
using UnityEngine;

namespace AuroraFPSRuntime
{
    public static class Decal
    {
        private static TerrainManager TerrainManagerInstance;
        private static PoolManager PoolManagerInstance;

        static Decal()
        {
            TerrainManagerInstance = TerrainManager.GetRuntimeInstance();
            PoolManagerInstance = PoolManager.GetRuntimeInstance();
        }

        /// <summary>
        /// Spawn decal from decal mapping by raycast hit info.
        /// </summary>
        /// <param name="mapping">Decal mapping reference.</param>
        /// <param name="hitInfo">Raycast hit info.</param>
        /// <returns>Reference to spawned decal object.</returns>
        public static PoolObject Spawn(DecalMapping mapping, RaycastHit hitInfo)
        {
            if (TryGetDecal(out PoolObject decal, mapping, hitInfo.collider, hitInfo.point))
            {
                return PoolManagerInstance.CreateOrPop(decal, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), hitInfo.transform);
            }
            return null;
        }

        /// <summary>
        /// Spawn decal from decal mapping by contact point info.
        /// </summary>
        /// <param name="mapping">Decal mapping reference.</param>
        /// <param name="contactPoint">Collider collision contact point.</param>
        /// <returns>Reference to spawned decal object.</returns>
        public static PoolObject Spawn(DecalMapping mapping, ContactPoint contactPoint)
        {
            if (TryGetDecal(out PoolObject decal, mapping, contactPoint.otherCollider, contactPoint.point))
            {
                return PoolManagerInstance.CreateOrPop(decal, contactPoint.point, Quaternion.LookRotation(contactPoint.normal), contactPoint.otherCollider.transform);
            }
            return null;
        }

        /// <summary>
        /// Try to get random decal from decal mapping in PoolObject representation.
        /// </summary>
        /// <param name="PoolObject">Out decal if is finded in decal mapping. Otherwise null.</param>
        /// <param name="mapping">Decal mapping reference.</param>
        /// <param name="other">Overlapped transform reference.</param>
        /// <param name="point">Hit point on overlapped transform.</param>
        public static bool TryGetDecal(out PoolObject decal, DecalMapping mapping, Collider other, Vector3 point)
        {
            if (mapping != null)
            {
                if (other.CompareTag(TNC.Terrain) && TerrainManagerInstance.GetTerrainTextureDetectors() != null)
                {
                    for (int i = 0, length = TerrainManagerInstance.GetTerrainTextureDetectorsLength(); i < length; i++)
                    {
                        TerrainTextureDetector terrainTextureDetector = TerrainManagerInstance.GetTerrainTextureDetector(i);
                        if (terrainTextureDetector.GetTerrain().transform == other.transform)
                        {
                            Texture2D terrainTexture = terrainTextureDetector.GetActiveTexture(point);
                            if (terrainTexture != null && mapping.ContainsKey(terrainTexture))
                            {
                                PoolObject[] decals = mapping.GetValue(terrainTexture).GetStorageData();
                                if(decals != null)
                                {
                                    decal = decals[Random.Range(0, decals.Length)];
                                    return decal != null;
                                }
                            }
                        }
                    }
                }
                else
                {
#if UNITY_6000_0_OR_NEWER
                    PhysicsMaterial physicMaterial = other.sharedMaterial;
#else
                    PhysicMaterial physicMaterial = other.sharedMaterial;
#endif
                    if (physicMaterial != null && mapping.ContainsKey(physicMaterial))
                    {
                        PoolObject[] decals = mapping.GetValue(physicMaterial).GetStorageData();
                        if (decals != null)
                        {
                            decal = decals[Random.Range(0, decals.Length)];
                            return decal != null;
                        }
                    }
                }
            }
            decal = null;
            return false;
        }

        /// <summary>
        /// Try to get decals from decal mapping in PoolObject representation.
        /// </summary>
        /// <param name="PoolObject">Out decals if is finded in decal mapping. Otherwise null.</param>
        /// <param name="mapping">Decal mapping reference.</param>
        /// <param name="other">Overlapped transform reference.</param>
        /// <param name="point">Hit point on overlapped transform.</param>
        public static bool TryGetDecals(out PoolObject[] decals, DecalMapping mapping, Collider other, Vector3 point)
        {
            if (mapping != null)
            {
                if (other.CompareTag(TNC.Terrain) && TerrainManagerInstance.GetTerrainTextureDetectors() != null)
                {
                    for (int i = 0, length = TerrainManagerInstance.GetTerrainTextureDetectorsLength(); i < length; i++)
                    {
                        TerrainTextureDetector terrainTextureDetector = TerrainManagerInstance.GetTerrainTextureDetector(i);
                        if (terrainTextureDetector.GetTerrain().transform == other.transform)
                        {
                            Texture2D terrainTexture = terrainTextureDetector.GetActiveTexture(point);
                            if (terrainTexture != null && mapping.ContainsKey(terrainTexture))
                            {
                                decals = mapping.GetValue(terrainTexture).GetStorageData();
                                return decals != null;
                            }
                        }
                    }
                }
                else
                {
#if UNITY_6000_0_OR_NEWER
                    PhysicsMaterial physicMaterial = other.sharedMaterial;
#else
                    PhysicMaterial physicMaterial = other.sharedMaterial;
#endif
                    if (physicMaterial != null && mapping.ContainsKey(physicMaterial))
                    {
                        decals = mapping.GetValue(physicMaterial).GetStorageData();
                        return decals != null;
                    }
                }
            }
            decals = null;
            return false;
        }
    }
}