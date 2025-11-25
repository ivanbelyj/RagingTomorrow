/* ================================================================
   ----------------------------------------------------------------
   Project   :   Aurora FPS Engine
   Publisher :   Renowned Games
   Developer :   Tamerlan Shakirov
   ----------------------------------------------------------------
   Copyright 2022 Renowned Games All rights reserved.
   ================================================================ */

using AuroraFPSRuntime.CoreModules.ValueTypes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AuroraFPSRuntime.CoreModules.Mathematics
{
    public struct ClipPlanePoints
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }

    public static class Math
    {
        /// <summary>
        /// Clamp camera rotation around axis X.
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Quaternion ClampRotation(Quaternion q, float min, float max)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;
            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
            angleX = Mathf.Clamp(angleX, min, max);
            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        /// <summary>
        /// Normalized the angle. between -180 and 180 degrees
        /// </summary>
        /// <param Name="eulerAngle">Euler angle.</param>
        public static Vector3 NormalizeAngle(this Vector3 eulerAngle)
        {
            var delta = eulerAngle;

            if (delta.x > 180) delta.x -= 360;
            else if (delta.x < -180) delta.x += 360;

            if (delta.y > 180) delta.y -= 360;
            else if (delta.y < -180) delta.y += 360;

            if (delta.z > 180) delta.z -= 360;
            else if (delta.z < -180) delta.z += 360;

            return new Vector3(delta.x, delta.y, delta.z); //round values to angle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="otherVector"></param>
        /// <returns></returns>
        public static Vector3 Difference(this Vector3 vector, Vector3 otherVector)
        {
            return otherVector - vector;
        }

        public static float Distance2D(Vector3 a, Vector3 b)
        {
            Vector3 result = b - a;
            result.y = 0;
            return result.magnitude;
        }

        public static void SetActiveChildren(this GameObject gameObject, bool value)
        {
            for (int i = 0, length = gameObject.transform.childCount; i < length; i++)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float ClampAngle(float angle, float min, float max)
        {
            do
            {
                if (angle < -360)
                    angle += 360;
                if (angle > 360)
                    angle -= 360;
            } while (angle < -360 || angle > 360);

            return Mathf.Clamp(angle, min, max);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="pos"></param>
        /// <param name="clipPlaneMargin"></param>
        /// <returns></returns>
        public static ClipPlanePoints NearClipPlanePoints(this Camera camera, Vector3 pos, float clipPlaneMargin)
        {
            var clipPlanePoints = new ClipPlanePoints();

            var transform = camera.transform;
            var halfFOV = (camera.fieldOfView / 2) * Mathf.Deg2Rad;
            var aspect = camera.aspect;
            var distance = camera.nearClipPlane;
            var height = distance * Mathf.Tan(halfFOV);
            var width = height * aspect;
            height *= 1 + clipPlaneMargin;
            width *= 1 + clipPlaneMargin;
            clipPlanePoints.LowerRight = pos + transform.right * width;
            clipPlanePoints.LowerRight -= transform.up * height;
            clipPlanePoints.LowerRight += transform.forward * distance;

            clipPlanePoints.LowerLeft = pos - transform.right * width;
            clipPlanePoints.LowerLeft -= transform.up * height;
            clipPlanePoints.LowerLeft += transform.forward * distance;

            clipPlanePoints.UpperRight = pos + transform.right * width;
            clipPlanePoints.UpperRight += transform.up * height;
            clipPlanePoints.UpperRight += transform.forward * distance;

            clipPlanePoints.UpperLeft = pos - transform.right * width;
            clipPlanePoints.UpperLeft += transform.up * height;
            clipPlanePoints.UpperLeft += transform.forward * distance;

            return clipPlanePoints;
        }



        /// <summary>
        /// Calculate center of the vectors
        /// </summary>
        public static Vector3 CenterOfVectors(params Vector3[] vectors)
        {
            Vector3 center = Vector3.zero;

            if (vectors.Length == 0)
            {
                return center;
            }

            int length = vectors.Length;
            for (int i = 0; i < length; i++)
            {
                center += vectors[i];
            }

            return center / length;
        }

        /// <summary>
        /// Calculate center of the vectors
        /// </summary>
        public static Vector3 CenterOfVectors(params Transform[] transforms)
        {
            Vector3 center = Vector3.zero;

            if (transforms.Length == 0)
            {
                return center;
            }

            int length = transforms.Length;
            for (int i = 0; i < length; i++)
            {
                center += transforms[i].position;
            }

            return center / length;
        }

        /// <summary>
        /// Calculate center of the vectors
        /// </summary>
        public static Vector3 HorizontalCenterOfVectors(params Vector3[] vectors)
        {
            Vector3 center = Vector3.zero;
            if (vectors.Length == 0)
            {
                return center;
            }

            int length = vectors.Length;
            for (int i = 0; i < length; i++)
            {
                Vector3 position = vectors[i];
                position.y = 0;
                center += position;
            }

            return center / length;
        }

        /// <summary>
        /// Calculate center of the vectors
        /// </summary>
        public static Vector3 HorizontalCenterOfVectors(params Transform[] transforms)
        {
            Vector3 center = Vector3.zero;
            if (transforms.Length == 0)
            {
                return center;
            }

            int length = transforms.Length;
            for (int i = 0; i < length; i++)
            {
                Vector3 position = transforms[i].position;
                position.y = 0;
                center += position;
            }

            return center / length;
        }

        /// <summary>
        /// Calculate center of the vectors
        /// </summary>
        public static Vector3 CenterOfVectors(List<Transform> transforms)
        {
            Vector3 center = Vector3.zero;

            if (transforms.Count == 0)
            {
                return center;
            }

            int length = transforms.Count;
            for (int i = 0; i < length; i++)
            {
                center += transforms[i].position;
            }

            return center / length;
        }

        /// <summary>
        /// Calculate center of the vectors
        /// </summary>
        public static Vector3 HorizontalCenterOfVectors(List<Transform> transforms)
        {
            Vector3 center = Vector3.zero;
            if (transforms.Count == 0)
            {
                return center;
            }

            int length = transforms.Count;
            for (int i = 0; i < length; i++)
            {
                Vector3 position = transforms[i].position;
                position.y = 0;
                center += position;
            }

            return center / length;
        }

        /// <summary>
        /// Calculate center of the vectors
        /// </summary>
        public static Vector3 CenterOfVectors(List<Vector3> vectors)
        {
            Vector3 center = Vector3.zero;

            if (vectors.Count == 0)
            {
                return center;
            }

            int length = vectors.Count;
            for (int i = 0; i < length; i++)
            {
                center += vectors[i];
            }

            return center / length;
        }

        /// <summary>
        /// Calculate center of the vectors
        /// </summary>
        public static Vector3 HorizontalCenterOfVectors(List<Vector3> vectors)
        {
            Vector3 center = Vector3.zero;
            if (vectors.Count == 0)
            {
                return center;
            }

            int length = vectors.Count;
            for (int i = 0; i < length; i++)
            {
                Vector3 position = vectors[i];
                position.y = 0;
                center += position;
            }

            return center / length;
        }

        public static Vector3 GetNearestVector(Vector3 center, params Vector3[] vectors)
        {
            Vector3 nearest = Vector3.zero;
            if (vectors == null || vectors.Length == 0)
            {
                return nearest;
            }

            float minDistance = Mathf.Infinity;
            for (int i = 0, length = vectors.Length; i < length; i++)
            {
                Vector3 vector = vectors[i];
                if (Vector3.Distance(center, vector) < minDistance)
                {
                    nearest = vector;
                }
            }
            return nearest;
        }

        public static Vector3 GetNearestVector(Vector3 center, List<Vector3> vectors)
        {
            Vector3 nearest = Vector3.zero;
            if (vectors == null || vectors.Count == 0)
            {
                return nearest;
            }

            float minDistance = Mathf.Infinity;
            for (int i = 0, length = vectors.Count; i < length; i++)
            {
                Vector3 vector = vectors[i];
                if (Vector3.Distance(center, vector) < minDistance)
                {
                    nearest = vector;
                }
            }
            return nearest;
        }

        public static Transform GetNearestTransform(Vector3 center, params Transform[] transforms)
        {
            Transform nearest = null;
            if (transforms == null || transforms.Length == 0)
            {
                return nearest;
            }

            float minDistance = Mathf.Infinity;
            for (int i = 0, length = transforms.Length; i < length; i++)
            {
                Transform transform = transforms[i];
                if (Vector3.Distance(center, transform.position) < minDistance)
                {
                    nearest = transform;
                }
            }
            return nearest;
        }

        public static Transform GetNearestTransform(Vector3 center, List<Transform> transforms)
        {
            Transform nearest = null;
            if (transforms == null || transforms.Count == 0)
            {
                return nearest;
            }

            float minDistance = Mathf.Infinity;
            for (int i = 0, length = transforms.Count; i < length; i++)
            {
                Transform transform = transforms[i];
                if (Vector3.Distance(center, transform.position) < minDistance)
                {
                    nearest = transform;
                }
            }
            return nearest;
        }

        /// <summary>
        /// Camera culling RayCast.
        /// </summary>
        public static bool CullingRayCast(Vector3 from, ClipPlanePoints _to, out RaycastHit hitInfo, float distance, LayerMask cullingLayer, Color color)
        {

            return (Physics.Raycast(from, _to.LowerLeft - from, out hitInfo, distance, cullingLayer, QueryTriggerInteraction.Ignore) ||
                Physics.Raycast(from, _to.LowerRight - from, out hitInfo, distance, cullingLayer, QueryTriggerInteraction.Ignore) ||
                Physics.Raycast(from, _to.UpperLeft - from, out hitInfo, distance, cullingLayer, QueryTriggerInteraction.Ignore) ||
                Physics.Raycast(from, _to.UpperRight - from, out hitInfo, distance, cullingLayer, QueryTriggerInteraction.Ignore));
        }

        /// <summary>
        /// Generate random position in the circle with specific radius
        /// </summary>
        public static Vector3 RandomPositionInCircle(Vector3 center, float radius)
        {
            Vector2 randomPos = Random.insideUnitCircle * radius;
            return new Vector3(center.x + randomPos.x, center.y, center.z + randomPos.y);
        }

        /// <summary>
        /// Generate random position in the rectangle
        /// </summary>
        /// <param name="length">Rectangle length</param>
        /// <param name="weight">Rectangle weight</param>
        /// <returns>Return Vector3</returns>
        public static Vector3 RandomPositionInRectangle(Vector3 center, float length, float weight)
        {
            Vector3 position;
            position.x = Random.Range(center.x - weight / 2, center.x + weight / 2);
            position.y = center.y;
            position.z = Random.Range(center.z - length / 2, center.z + length / 2);
            return position;
        }

        /// <summary>
        /// Get random destination on a Navmesh surface.
        /// </summary>
        public static Vector3 GetRandomDestination(Vector3 center, float maxDistance)
        {
            Vector3 randomPosition = Random.insideUnitSphere * maxDistance + center;

            NavMeshHit hitInfo;
            NavMesh.SamplePosition(randomPosition, out hitInfo, maxDistance, NavMesh.AllAreas);

            return hitInfo.position;
        }

        public static float RandomValue(float value, float min, float max, float tollerance)
        {
            float lastValue = value;
            while (Approximately(value, lastValue, tollerance))
                value = Random.Range(min, max);
            return value;
        }

        public static int RandomValue(int value, int min, int max)
        {
            if (min == max)
                return max;

            int tryCount = 3;
            int lastValue = value;
            while (lastValue == value || tryCount > 0)
            {
                value = Random.Range(min, max);
                tryCount -= 1;
            }
            return value;
        }

        public static float Clamp(float value, RangedFloat range)
        {
            return Mathf.Clamp(value, range.GetMin(), range.GetMax());
        }

        public static float Clamp(float value, Vector2 range)
        {
            return Mathf.Clamp(value, range.x, range.y);
        }

        /// <summary>
        /// Get percent from this value.
        /// </summary>
        public static float GetPersent(float value, float maxValue)
        {
            return (100f / maxValue) * value;
        }

        /// <summary>
        /// Get percent from this value.
        /// </summary>
        public static int GetPersent(int value, int maxValue)
        {
            return Convert.ToInt32(System.Math.Round(((decimal)value / maxValue) * 100, 0));
        }

        /// <summary>
        /// Comparison of the two float values.
        /// </summary>
        /// <param name="a">Value A</param>
        /// <param name="b">Value B</param>
        /// <param name="tolerance ">Max tolerance for A and B values</param>
        /// <returns></returns>
        public static bool Approximately(float a, float b, float tolerance = 0.01f)
        {
            return Mathf.Abs(a - b) < tolerance;
        }

        /// <summary>
        /// Loop value in specific range.
        /// </summary>
        public static float Loop(float value, float min, float max)
        {
            if(value < min)
                return max;
            else if(value > max)
                return min;
            else
                return value;
        }

        /// <summary>
        /// Loop value in specific range.
        /// </summary>
        public static int Loop(int value, int min, int max)
        {
            if (value < min)
                return max;
            else if (value > max)
                return min;
            else
                return value;
        }

        /// <summary>
        /// Allocate part of the number.
        /// </summary>
        public static float AllocatePart(float value, float part = 100)
        {
            return Mathf.Round(value * part) / part;
        }

        /// <summary>
        /// Return true if value between min and max values;
        /// </summary>
        public static bool InRange(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Return true if value between min and max values;
        /// </summary>
        public static bool InRange(int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Return true if value between x and y values;
        /// </summary>
        public static bool InRange(float value, Vector2 range)
        {
            return value >= range.x && value <= range.y;
        }

        public static bool IsDivisble(int x, int n)
        {
            return (x % n) == 0;
        }

        public static int GetSettedBitCount(long value)
        {
            int count = 0;
            while (value != 0)
            {
                value = value & (value - 1);
                count++;
            }
            return count;
        }
    }
}