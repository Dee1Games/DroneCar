

namespace RaycastPro
{
    using UnityEngine;

#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif

    public abstract class BasePlanar : RaycastCore
    {
        public struct TransitionData
        {
            public Vector3 position;
            public Quaternion rotation;
        }
        
        public float offset = .05f;

        public enum LengthControl { Continues, Constant, Sync, }

        public LengthControl lengthControl = LengthControl.Continues;

        public float length = 1;

        public enum OuterType { Auto, Reference, Clone }

        public OuterType outerType = OuterType.Auto;

        public enum DirectionOutput
        {
            /// <summary>
            /// Influence CloneRay Rotation by _Planar.transform.forward Direction as base Rotation.
            /// </summary>
            PlanarForward,
            /// <summary>
            /// Influence CloneRay Rotation by Sensor Local Direction as base Rotation.
            /// </summary>
            SensorLocal,
            /// <summary>
            /// Influence CloneRay Rotation by -Hit.Normal as base Rotation.
            /// </summary>
            NegativeHitNormal,
            /// <summary>
            /// Influence CloneRay Rotation by HitDirection as base Rotation.
            /// </summary>
            HitDirection,
        }

        public DirectionOutput baseDirection = DirectionOutput.PlanarForward;
        
        // public void Update() { if (autoUpdate == UpdateMode.Normal) OnCast(); }
        // public void LateUpdate() { if (autoUpdate == UpdateMode.Late) OnCast(); }
        // protected void FixedUpdate() { if (autoUpdate == UpdateMode.Fixed) OnCast(); }

        protected override void OnCast() { } // NOTHING FOR NOW

#if UNITY_EDITOR
        
        protected void OuterField(SerializedProperty property, SerializedProperty _outerRay)
        {
            BeginVerticalBox();
            
            PropertyEnumField(property, 3, "Outer Type".ToContent("Outer Type"), new GUIContent[]
            {
                "Auto".ToContent("Auto"),
                "Reference".ToContent("Reference"),
                "Clone".ToContent("Clone"),
            });
            
            if (outerType == OuterType.Reference) EditorGUILayout.PropertyField(_outerRay);
            
            EndVertical();
        }
        protected void BaseDirectionField(SerializedObject _so)
        {
            BeginVerticalBox();
            PropertyEnumField(_so.FindProperty(nameof(baseDirection)), 2, "Base Direction".ToContent("Base Direction"), new GUIContent[]
            {
                "Planar Forward".ToContent("Planar Forward"),
                "Sensor Local".ToContent("Sensor Local"),
                "-Hit Normal".ToContent("-Hit Normal"),
                "Hit Direction".ToContent("Hit Direction"),
            });
            
            EndVertical();
        }

        protected void LengthControlField(SerializedObject _so)
        {
            BeginVerticalBox();
            PropertyEnumField(_so.FindProperty(nameof(lengthControl)), 3, CLengthControl.ToContent(TLengthControl), new GUIContent[]
            {
                "Continues".ToContent("Continues"),
                "Constant".ToContent("Constant"),
                "Sync".ToContent("Sync"),
            });
            PropertyMaxField(_so.FindProperty(nameof(length)), (lengthControl == LengthControl.Constant) ? CLength.ToContent() : CMultiplier.ToContent());
            EndVertical();
        }
#endif
    }
}