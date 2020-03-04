using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BezierSurcuitUtitlity
{
    public static class Bezier
    {
        public static Vector3 LerpCurve(Vector3 point1, Vector3 point2, float t)
        {
            return point1 + (point2 - point1) * t;
        }

        public static Vector3 QuadraticCurve(Vector3 point1, Vector3 point2, Vector3 point3, float t)
        {
            Vector3 mediumPoint1 = LerpCurve(point1, point2, t);
            Vector3 mediumPoint2 = LerpCurve(point2, point3, t);

            return LerpCurve(mediumPoint1, mediumPoint2, t);
        }

        public static Vector3 CubicCurve(Vector3 point1, Vector3 point2, Vector3 point3, Vector3 point4, float t)
        {
            Vector3 mediumPoint1 = QuadraticCurve(point1, point2, point3, t);
            Vector3 mediumPoint2 = QuadraticCurve(point2, point3, point4, t);

            return LerpCurve(mediumPoint1, mediumPoint2, t);
        }
    }
}
