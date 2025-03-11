using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThiccTapeman.Bezier
{
    public class CubicBezier
    {
        public Vector3 p0;
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;

        public Vector3 CalculateCubicBezierPoint(float t)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

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

            Vector3 tangent = -3 * uu * p0;
            tangent += 3 * (uu - 2 * u * t) * p1;
            tangent += 3 * (t * (2 * u - t)) * p2;
            tangent += 3 * tt * p3;

            return tangent;
        }

        public void DrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(p0, p1);
            Gizmos.DrawLine(p2, p3);

            Gizmos.color = Color.green;
            Vector3 lastPoint = p0;
            for (float t = 0.05f; t <= 1; t += 0.05f)
            {
                Vector3 point = CalculateCubicBezierPoint(t);
                Gizmos.DrawLine(lastPoint, point);
                lastPoint = point;
            }
        }
    }
}

