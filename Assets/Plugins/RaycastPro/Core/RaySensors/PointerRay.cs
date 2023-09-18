namespace RaycastPro.RaySensors
{
    using UnityEngine;

#if ENABLE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
#endif

#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif
    
    [AddComponentMenu("RaycastPro/Rey Sensors/"+nameof(PointerRay))]
    public sealed class PointerRay : RaySensor, IRadius
    {
        public Camera mainCamera;
        
        [SerializeField] private float radius = .4f;
        public float Radius
        {
            get => radius;
            set => radius = Mathf.Max(0,value);
        }
        
        [Tooltip("Force main ray to casting from camera pivot.")]
        public bool rayFromCamera;
        
        public Camera.MonoOrStereoscopicEye eyeType = Camera.MonoOrStereoscopicEye.Mono;
        private Ray mouseRay;
        
        public override Vector3 Tip {

            get
            {
#if UNITY_EDITOR
                if (InEditMode)
                {
                    var _t = rayFromCamera ? mainCamera.transform : transform;
                    return _t.position + _t.forward * direction.magnitude;
                }
                else
                {
                    return mouseRay.origin + mouseRay.direction * direction.magnitude;
                }
#else
            return mouseRay.origin + mouseRay.direction * direction.magnitude;
#endif
            }
            
        }

        public override float RayLength => TipLength;
        public override Vector3 Base => rayFromCamera ? mainCamera.transform.position : transform.position;

        private Vector3 input;
        private RaycastHit mouseHit;
        
        protected override void OnCast()
        {
            if (!mainCamera) return;
#if UNITY_EDITOR
            GizmoGate = null;
#endif

#if ENABLE_INPUT_SYSTEM
            input = Mouse.current.position.ReadValue();
#else
            input = Input.mousePosition;
#endif
            mouseRay = mainCamera.ScreenPointToRay(input, eyeType);
            
            if (!rayFromCamera) // Ray From Object
            {
                // detect point of impact for calculating direction
                Physics.Raycast(mouseRay.origin, mouseRay.direction, out mouseHit, Mathf.Infinity, detectLayer.value, triggerInteraction);

                if (!mouseHit.transform)
                {
                    hit = default;
                    return;
                }

                var mainRay = mouseHit.point - transform.position;
                
                if (radius > 0)
                {
                    Physics.SphereCast(transform.position, radius, mainRay, out hit, direction.z, detectLayer.value, triggerInteraction);
                }
                else
                {
                    Physics.Raycast(transform.position, mainRay, out hit, direction.z, detectLayer.value, triggerInteraction);
                }
                
                // divide gizmo
#if UNITY_EDITOR
                GizmoGate += () =>
                {
                    if (mouseHit.transform)
                    {
                        GUI.color = HelperColor;

                        var _mt = transform;
                        var p1 = _mt.transform.position;
                        var p2 = Tip;
                        DrawLine(p1+_mt.right*radius, p2+_mt.right*radius);
                        DrawLine(p1+_mt.up*radius, p2+_mt.up*radius);
                        DrawLine(p1-_mt.right*radius, p2-_mt.right*radius);
                        DrawLine(p1-_mt.up*radius, p2-_mt.up*radius);
                    
                        Handles.DrawWireDisc((p1 + p2) / 2, p2 - p1, radius);
                    
                        Handles.DrawWireDisc(p2, p2 - p1, radius);

                        if (Application.isPlaying) DrawDetectLine(transform.position, p2, hit, Performed);
                        
                        if (hit.transform) DrawNormal(hit);
                    }
                };
#endif
            }
            else // Ray From Camera
            {
                if (radius > 0)
                {
                    Physics.SphereCast(mouseRay.origin, radius, mouseRay.direction, out hit, direction.z, detectLayer.value, triggerInteraction);
                }
                else
                {
                    Physics.Raycast(mouseRay.origin, mouseRay.direction, out hit, direction.z, detectLayer.value, triggerInteraction);
                }


#if UNITY_EDITOR
                GizmoGate += () =>
                {
                    if (hit.transform) DrawNormal(hit);
                    GizmoColor = Performed ? DetectColor : DefaultColor;
                    var _mt = mainCamera.transform;
                    var p1 = mouseRay.origin;
                    var p2 = Vector3.zero;
                    if (InEditMode)
                    {
                        p1 = transform.position;
                        p2 = transform.position + direction.normalized;
                    }
                    else
                    {
                        p2 = mouseRay.origin + mouseRay.direction*direction.z;
                    }
                    
                    DrawLine(p1+_mt.right*radius, p2+_mt.right*radius);
                    DrawLine(p1+_mt.up*radius, p2+_mt.up*radius);
                    DrawLine(p1-_mt.right*radius, p2-_mt.right*radius);
                    DrawLine(p1-_mt.up*radius, p2-_mt.up*radius);
                    
                    Handles.DrawWireDisc((p1 + p2) / 2, p2 - p1, radius);
                    Handles.DrawWireDisc(p2, p2 - p1, radius);

                    if (Application.isPlaying) DrawDetectLine(p1, p2, hit, Performed);
                    else
                    {
                        GizmoColor = HelperColor;
                        DrawLine(p1, p2, true);
                    }
                };
#endif
            }

        }
        
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "A mouse location tracker that is able to emit from the desired object and is used to immediately launch this feature."+HAccurate+HIRadius+HDependent;
#pragma warning restore CS0414
        internal override void OnGizmos() => EditorUpdate();
        private bool InEditMode => IsSceneView || !Application.isPlaying;
        
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                BeginHorizontal();
                var propCamera = _so.FindProperty(nameof(mainCamera));
                EditorGUILayout.PropertyField(propCamera);
                if (!mainCamera && GUILayout.Button("Main", GUILayout.Width(50f)))
                {
                    propCamera.objectReferenceValue = Camera.main;
                }
                EndHorizontal();
                DirectionField(_so);
                RadiusField(_so);
                if (mainCamera && transform != mainCamera.transform)
                {
                    EditorGUILayout.PropertyField(_so.FindProperty(nameof(rayFromCamera)));
                }
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(eyeType)));
            }
            
            if (hasGeneral) GeneralField(_so);
            if (hasEvents) EventField(_so);
            if (hasInfo) InformationField();
        }
#endif
    }
}
