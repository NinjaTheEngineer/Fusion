using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NinjaTools {
    public static class NMaths {
        public static Vector3[] CreateBezierPath(Vector3[] points, int pathSize = 100) {
            var logId = "CreateBezierPath";
            if (points.Length != 4) {
                Utils.logw(logId, "Points array doesn't have the required Size of 4.");
                return null;
            }
            return CreateBezierPath(points[0], points[1], points[2], points[3], pathSize);
        }
        /*
        startPoint = Path's start point
        cp1 = First Control point, point where the curve starts to rise
        cp2 = Second Control point, highest point in the curve
        endPoint = Path's end point
        */
        public static Vector3[] CreateBezierPath(Vector3 startPoint, Vector3 cp1, Vector3 cp2, Vector3 endPoint, int pathSize = 100) {
            Vector3[] path = new Vector3[pathSize];
            for (int i = 0; i < pathSize; i++) {
                float t = i / (float)(pathSize - 1);
                float u = 1 - t;
                float tt = t * t;
                float uu = u * u;
                float uuu = uu * u;
                float ttt = tt * t;

                Vector3 p = uuu * startPoint;
                p += 3 * uu * t * cp1;
                p += 3 * u * tt * cp2;
                p += ttt * endPoint;

                path[i] = p;
            }
            return path;
        }

        public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, float height) {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0; // (1-t)^3 * p0
            p += 3 * uu * t * (p0 + Vector3.down * height); // 3 * (1-t)^2 * t * (p0 + height * down vector)
            p += 3 * u * tt * (p1 + Vector3.down * height); // 3 * (1-t) * t^2 * (p1 + height * down vector)
            p += ttt * p1; // t^3 * p1

            return p;
        }
    }
}
