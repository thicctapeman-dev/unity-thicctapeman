using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThiccTapeman.Bezier
{
    [System.Serializable]
    public class CubicBezier
    {
        public Vector3 pointA;
        public Vector3 handleA;
        public Vector3 pointB;
        public Vector3 handleB;

        public Vector3 CalculateCubicBezierPoint(float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * pointA;
            p += 3 * uu * t * handleA;
            p += 3 * u * tt * handleB;
            p += ttt * pointB;

            return p;
        }

        public Vector3 CalculateCubicBezierPointNormalOffset(float t, float offset)
        {
            Vector3 point = CalculateCubicBezierPoint(t);
            Vector3 normalOffset = CalculateCubicBezierTangent(t) * offset;

            return point + normalOffset;
        }

        private Vector3 CalculateCubicBezierTangent(float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 tangent = -3 * uu * pointA;
            tangent += 3 * (uu - 2 * u * t) * handleA;
            tangent += 3 * (t * (2 * u - t)) * handleB;
            tangent += 3 * tt * pointB;

            return tangent;
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA, handleA);
            Gizmos.DrawLine(handleB, pointB);

            Gizmos.color = Color.green;
            Vector3 lastPoint = pointA;
            for (float t = 0.05f; t <= 1; t += 0.05f)
            {
                Vector3 point = CalculateCubicBezierPoint(t);
                Gizmos.DrawLine(lastPoint, point);
                lastPoint = point;
            }
        }
    }
}

