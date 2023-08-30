
namespace RaycastPro.RaySensors
{
    using UnityEngine;

#if UNITY_EDITOR
    using System.Threading.Tasks;
    using UnityEditor;
#endif

    [AddComponentMenu("RaycastPro/Rey Sensors/" + nameof(BoxRay))]
    public sealed class BoxRay : RaySensor
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info =
#pragma warning restore CS0414
            "Emit a box shape line in the specified direction with defined extents and return the Hit information." +
            HAccurate + HDirectional;
#endif
        
        public Vector3 extents = new Vector3(.4f, .4f, .4f);
        protected override void OnCast()
        {
            Physics.BoxCast(transform.position, extents / 2, Direction, out hit, transform.rotation,
                direction.magnitude, detectLayer.value, triggerInteraction);
        }

#if UNITY_EDITOR
        
        /// <summary>
        /// Hint: This command will make your references missing.
        /// </summary>
        [ContextMenu("Convert To PipeRay")]
        private async void ConvertToPipeRay()
        {
            var _ray = Undo.AddComponent<PipeRay>(gameObject);
            
            _ray.direction = direction;
            _ray.Radius = extents.x;
            _ray.Height = extents.y;

            await Task.Delay(1);
            Undo.DestroyObjectImmediate (this);
        }
        /// <summary>
        /// Hint: This command will make your references missing.
        /// </summary>
        [ContextMenu("Convert To BaseRay")]
        private async void ConvertToBasicRay()
        {
            var _ray = Undo.AddComponent<BasicRay>(gameObject);
            
            _ray.direction = direction;

            await Task.Delay(1);
            
            Undo.DestroyObjectImmediate (this);
        }

        internal override void OnGizmos()
        {
            EditorUpdate();
            GizmoColor = Performed ? DetectColor : DefaultColor;
            DrawBoxLine(transform.position, transform.position + Direction, extents, true);
            DrawNormal(hit);
        }

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                DirectionField(_so);
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(extents)));
            }
            if (hasGeneral) GeneralField(_so);
            if (hasEvents) EventField(_so);
            if (hasInfo) InformationField();
        }
#endif

        private Vector3 ExtentLengthZ => LocalDirection.normalized * extents.z / 2;
        public override Vector3 Tip => transform.position + Direction ;
        public override float RayLength => direction.magnitude + extents.z;
        public override Vector3 BasePoint => transform.position - ExtentLengthZ;
        public Vector3 ExtentBase => BasePoint - ExtentLengthZ;
        public Vector3 ExtentTip => Tip + ExtentLengthZ;
    }
}