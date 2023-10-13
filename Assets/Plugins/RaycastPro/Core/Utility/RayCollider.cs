using System;

namespace RaycastPro.Sensor
{
    using System.Collections.Generic;
    using RaySensors;
    using RaySensors2D;
    using UnityEngine;
#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif
    
    [AddComponentMenu("RaycastPro/Utility/" + nameof(RayCollider))]
    public sealed class RayCollider : BaseUtility, IRadius
    {
        public RaySensor raySensor;
        public MeshCollider meshCollider;
        public int segments = 8;
        public AnimationCurve clumpCurve = AnimationCurve.Linear(0, 1, 1, 1);
        
        [SerializeField] private float radius;
        public float Radius
        {
            get => radius;
            set => radius = Mathf.Max(0,value);
        }

        private List<Vector3> localPath = new List<Vector3>();
        private int[] triangles = Array.Empty<int>();
        
        public override bool Performed
        {
            get => raySensor.Performed;
            protected set
            {
            }
        }

        private void Reset()
        {
            meshCollider = GetComponent<MeshCollider>();
        }

        public Mesh CreateCylinder(List<Vector3> path)
        {
            Mesh mesh = new Mesh();

            int pathCount = path.Count;
            int vertexCount = pathCount * (segments + 1);
            int triangleCount = pathCount * segments * 6;

            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[triangleCount];
            Vector3[] normals = new Vector3[vertexCount];
            Vector2[] uv = new Vector2[vertexCount];

            for (int i = 0; i < pathCount; i++)
            {
                Vector3 point = path[i];
                Vector3 direction = Vector3.zero;

                if (i > 0)
                    direction = (path[i] - path[i - 1]).normalized;

                Vector3 side = Vector3.Cross(Vector3.up, direction).normalized;

                for (int j = 0; j <= segments; j++)
                {
                    float angle = (float)j / segments * 2 * Mathf.PI;
                    Vector3 position = point + side * (radius * Mathf.Cos(angle)) + Vector3.up * (radius * Mathf.Sin(angle));
                    int vertexIndex = i * (segments + 1) + j;

                    vertices[vertexIndex] = position;
                    normals[vertexIndex] = (position - point).normalized;
                    
                    // Calculate UV coordinates
                    float u = (float)j / segments;
                    float v = (float)i / (pathCount - 1);
                    uv[vertexIndex] = new Vector2(u, v);
                }
            }

            int triangleIndex = 0;
            for (int i = 0; i < pathCount - 1; i++)
            {
                for (int j = 0; j < segments; j++)
                {
                    int vertexIndex = i * (segments + 1) + j;

                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + 1;
                    triangles[triangleIndex + 2] = vertexIndex + segments + 1;

                    triangles[triangleIndex + 3] = vertexIndex + 1;
                    triangles[triangleIndex + 4] = vertexIndex + segments + 2;
                    triangles[triangleIndex + 5] = vertexIndex + segments + 1;

                    triangleIndex += 6;
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uv;



            return mesh;
        }

        protected override void OnCast()
        {
           
        }
#if UNITY_EDITOR
        
#pragma warning disable CS0414
        private static string Info = "Bake a Mesh Collider on ray Path." + HDependent + HUtility + HPreview;
#pragma warning restore CS0414
        internal override void OnGizmos()
        {
            // if (meshCollider?.sharedMesh)
            // {
            //     GizmoColor = DefaultColor;
            //     Gizmos.DrawWireMesh(meshCollider.sharedMesh);
            // }
        }

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(raySensor)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(meshCollider)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(segments)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(clumpCurve)));
                RadiusField(_so);

                if (hasGeneral)
                {
                    BaseField(_so);
                }

                if (hasInfo)
                {
                }

                if (GUILayout.Button("Bake Mesh Liner"))
                {
                    raySensor.GetPath(ref localPath);
                    var _cMesh = CreateCylinder(localPath);
                    _cMesh.name = raySensor.name + " Collider";
                    meshCollider.sharedMesh = _cMesh;
                    var vertices = meshCollider.sharedMesh.vertices;
                    var offset = raySensor.transform.position;
                    for (var i = 0; i < vertices.Length; i++)
                        
                    {
                       // meshCollider.sharedMesh.vertices[i] -= meshCollider.sharedMesh.vertices[0];
                       vertices[i] -= offset;
                    }
                    meshCollider.sharedMesh.vertices = vertices;


                    GetComponent<MeshFilter>().mesh = meshCollider.sharedMesh;
                }
            }

        }
#endif
    }
}