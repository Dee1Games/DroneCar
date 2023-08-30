namespace RaycastPro
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using RaySensors2D;
    using UnityEngine;
    using Object = UnityEngine.Object;

    internal static class IListExtensions {
        public static void Swap<T>(
            this IList<T> list,
            int firstIndex,
            int secondIndex
        ) {
            if (firstIndex == secondIndex) return;
            if (firstIndex < 0 || secondIndex < 0) return;
            if (firstIndex > list.Count-1 || secondIndex > list.Count-1) return;
            
            (list[firstIndex], list[secondIndex]) = (list[secondIndex], list[firstIndex]);
        }
    }
    
    public static class RaycastProExtension
    {
        internal static void CloneDestroy(this RaySensor2D sensor)
        {
            if (!(sensor && sensor.gameObject)) return;

            if (sensor.cloneRaySensor) CloneDestroy(sensor.cloneRaySensor);
            
            Object.Destroy(sensor.gameObject);
        }

        internal static Quaternion ToRotation2D(this Vector2 direction)
        {
            var angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
            
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }
        internal static Quaternion ToRotation2D(this Vector3 direction)
        {
            var angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
            
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }
        internal static Quaternion ToRotation3D(this Vector3 direction, Vector3 up) => Quaternion.LookRotation(direction, up);
        internal static Vector2 To2D(this Vector3 vector3) => new Vector2(vector3.x, vector3.y);
        internal static Vector3 ToXFlip(this Vector3 vector3) => new Vector3(-vector3.x, vector3.y, vector3.z);
        internal static Vector3 ToYFlip(this Vector3 vector3) => new Vector3(vector3.x, -vector3.y, vector3.z);
        internal static Vector3 ToZFlip(this Vector3 vector3) => new Vector3(vector3.x, vector3.y, -vector3.z);
        internal static Vector3 ToDepth(this Vector3 vector3, float depth = 0) => new Vector3(vector3.x, vector3.y, depth);
        internal static Vector3 ToDepth(this Vector2 vector2, float depth = 0) => new Vector3(vector2.x, vector2.y, depth);
        internal static void SetSlicedPosition(this LineRenderer liner, IEnumerable<Vector3> path, Vector3 hitPoint, int detectIndex = -1)
        {
            var list = path.ToList();
            if (detectIndex > -1)
            {
                liner.positionCount = detectIndex+2;
                
                for (var i = 0; i <= detectIndex; i++)
                {
                    liner.SetPosition(i, list[i]);
                }
            
                liner.SetPosition(detectIndex+1, hitPoint);
                        
                return;
            }
                    
            liner.positionCount = list.Count;
                        
            liner.SetPositions(list.ToArray());
        }
        internal static bool InColorTolerance(this Color color, Color targetColor, Color tolerance)
        {
            return Mathf.Abs(targetColor.r - color.r) <= tolerance.r
                   && Mathf.Abs(targetColor.b - color.b) <= tolerance.b
                   && Mathf.Abs(targetColor.g - color.g) <= tolerance.g
                   && Mathf.Abs(targetColor.a - color.a) <= tolerance.a;
        }
        internal static Vector3 GetDirection(this List<Vector3> points, int i) => points[i] - points[i - 1];
        internal static Vector2 GetDirection(this List<Vector2> points, int i) => points[i] - points[i - 1];
        internal static Vector3[] ToDepth(this Vector2[] points, float depth)
        {
            var newPoints = new Vector3[points.Length];
            
            for (var i = 0; i < points.Length; i++)  newPoints[i] = new Vector3(points[i].x, points[i].y, depth);
            
            return newPoints;
        }
        internal static Vector3[] ToDepth(this Vector3[] points, float depth = 0)
        {
            for (var i = 0; i < points.Length; i++)  points[i] = new Vector3(points[i].x, points[i].y, depth);
            
            return points;
        }
        internal static List<Vector3> ToDepth(this List<Vector3> points, float depth = 0)
        {
            for (var i = 0; i < points.Count; i++)  points[i] = new Vector3(points[i].x, points[i].y, depth);
            
            return points;
        }
        internal static List<Vector3> ToDepth(this List<Vector2> points, float depth = 0)
        {
            var newPoints = new List<Vector3>();
            
            for (var i = 0; i < points.Count; i++)  newPoints.Add(new Vector3(points[i].x, points[i].y, depth));
            
            return newPoints;
        }
        internal static Vector2[] To2D(this Vector3[] points)
        {
            var newPoints = new Vector2[points.Length];

            for (var i = 0; i < points.Length; i++)  newPoints[i] = new Vector2(points[i].x, points[i].y);
            
            return newPoints;
        }
        internal static Vector3 ToUp(this Vector3 points, float up) => new Vector3(points.x, points.y + up, points.z);
        internal static Vector3[] ToLocal(this Vector3[] points, Transform _t)
        {
            var newPoints = new Vector3[points.Length];
            
            for (var i = 0; i < points.Length; i++)  newPoints[i] = _t.TransformPoint(points[i]);
            
            return newPoints;
        }
        internal static Vector2[] ToLocal(this Vector2[] points, Transform _t)
        {
            var newPoints = new Vector2[points.Length];
            
            for (var i = 0; i < points.Length; i++)  newPoints[i] = _t.TransformPoint(points[i]);
            
            return newPoints;
        }
        internal static Vector3[] ToRelative(this Vector3[] points)
        {
            var newPoints = new Vector3[points.Length];
            
            for (var i = 0; i < points.Length; i++)
            {
                var sum = Vector3.zero;

                for (var j = 0; j <= i; j++) sum += points[j];

                newPoints[i] = sum;
            }

            return newPoints;
        }
        internal static Vector2[] ToRelative(this Vector2[] points)
        {
            var newPoints = new Vector2[points.Length];
            
            for (var i = 0; i < points.Length; i++)
            {
                var sum = Vector2.zero;

                for (var j = 0; j <= i; j++) sum += points[j];

                newPoints[i] = sum;
            }

            return newPoints;
        }
        
        internal static Vector3 LastDirection(this List<Vector3> points, Vector3 defaultDir) => points.Count > 1 ?
            points[points.Count - 1] - points[points.Count - 2] : defaultDir;
        internal static Vector2 LastDirection(this List<Vector2> points, Vector2 defaultDir) => points.Count > 1 ?
            points[points.Count - 1] - points[points.Count - 2] : defaultDir; 
        internal static T LastOrBase<T>(this List<T> objects, T baseObject) => objects.Count > 0 ? objects[objects.Count - 1] : baseObject;
        
        internal static T LastOrBase<T>(this List<T> objects, T baseObject, int lastIndex = 0) => objects.Count > lastIndex ? objects[objects.Count - (1 + lastIndex)] : baseObject;
        
        internal static string ToRegex(this string text) => Regex.Replace(text, "(\\B[A-Z])", " $1").Replace("2 D", "2D");
        internal static void RemoveChildren(this Transform t) { foreach (Transform child in t) Object.Destroy(child.gameObject);}
        internal static bool InLayer(this LayerMask mask, GameObject obj) => mask == (mask | (1 << obj.layer));
        internal static Vector3 PortalPoint(this Transform t, Transform from, Vector3 point)
        {
            var p = from.InverseTransformPoint(point);

            return t.TransformPoint(p);
        }
        internal static float GetPathLength(this IEnumerable<Vector3> points)
        {
            var distance = 0f;
            var enumerable = points.ToList();
            for (var i = 0; i < enumerable.Count-1; i++)
            {
                distance += (enumerable[i+1] - enumerable[i]).magnitude;
            }
            return distance;
        }
        internal static float GetPathLength(this IEnumerable<Vector2> points)
        {
            var distance = 0f;

            var enumerable = points.ToList();
            
            for (var i = 0; i < enumerable.Count-1; i++)
            {
                distance += (enumerable[i+1] - enumerable[i]).magnitude;
            }
            
            return distance;
        }
        internal static float GetPathLength(this IEnumerable<Vector3> points, int index)
        {
            var list = points.ToList();
            
            return (list[index] - list[index - 1]).magnitude;
        }
        internal static float GetPathLength(this IEnumerable<Vector2> points, int index)
        {
            var list = points.ToList();
            
            return (list[index] - list[index - 1]).magnitude;
        }
        internal static float GetPathLength(this IEnumerable<Vector3> points, int startIndex, int lastIndex)
        {
            var distance = 0f;

            var list = points.ToList();
            
            for (var i = startIndex; i < lastIndex; i++)
            {
                distance += (list[i+1] - list[i]).magnitude;
            }
            
            return distance;
        }
        internal static Vector3 GetPathInfo(this IEnumerable<Vector3> points, float pos)
        {
            var list = points.ToList();
            var posM = pos * list.GetPathLength();
            var p = Vector3.zero;
            for (var i = 1; i < list.Count; i++)
            {
                var lineDistance = list.GetPathLength(i);
                if (posM <= lineDistance) return Vector3.Lerp(list[i - 1], list[i], posM / lineDistance);
                posM -= lineDistance;
            }
            return p;
        }
        internal static (Vector3 point, int index) GetPathInfo(this List<Vector3> path, float value)
        {
            var posM = value * path.GetPathLength();

            for (var i = 1; i < path.Count; i++)
            {
                var lineDistance = path.GetPathLength(i);
                
                if (posM <= lineDistance) return (Vector3.Lerp(path[i - 1], path[i], posM / lineDistance), i);

                posM -= lineDistance;
            }
            return (path.Last(), path.Count-1);
        }

        internal static Vector3 GetPositionOnPath(this List<Vector3> path, float pos)
        {
            pos = Mathf.Clamp01(pos);
            var posM = pos * path.GetPathLength();

            var p = Vector3.zero;
            for (var i = 1; i < path.Count; i++)
            {
                var lineDistance = path.GetPathLength(i);
                if (posM <= lineDistance)
                {
                    p = Vector3.Lerp(path[i - 1], path[i], posM / lineDistance);
                    break;
                }
                posM -= lineDistance;
            }
            return p;
        }
        internal static (Vector2 point, int index) GetPathInfo(this List<Vector2> path, float value)
        {
            var posM = value * path.GetPathLength();

            for (var i = 1; i < path.Count; i++)
            {
                var lineDistance = path.GetPathLength(i);
                
                if (posM <= lineDistance) return (Vector3.Lerp(path[i - 1], path[i], posM / lineDistance), i);

                posM -= lineDistance;
            }
            return (path.Last(), path.Count-1);
        }
        internal static Color ToAlpha(this Color color, float alpha) => new Color(color.r, color.g, color.b, alpha);
        internal static GUIContent ToContent(this string label, string tooltip) => new GUIContent(label, tooltip);
        internal static GUIContent ToContent(this string label) => new GUIContent(label, label);
        internal static GUIContent[] ToContents(this string[] label, string[] tooltip)
        {
            var contents = new GUIContent[label.Length];

            for (var i = 0; i < contents.Length; i++)
            {
                contents[i] = label[i].ToContent(tooltip[i]);
            }

            return contents;
        }
        
        internal static GUIContent[] ToContent(this string[] label, string[] tooltip)
        {
            var contents = new GUIContent[label.Length];
            for (var i = 0; i < label.Length; i++)
            {
                contents[i].text = label[i];
                contents[i].tooltip = tooltip[i];
            }
            return contents;
        }
        internal static IEnumerable<Type> GetInheritedTypes(this Type BaseClass)
        {
            var subclassTypes = Assembly
                .GetAssembly(BaseClass)
                .GetTypes()
                .Where(type => type.IsSubclassOf(BaseClass));
            return subclassTypes;
        }
    }
}