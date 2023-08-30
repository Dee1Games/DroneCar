namespace RaycastPro.RaySensors
{
    using UnityEngine;

#if UNITY_EDITOR
    using System.Threading.Tasks;
    using UnityEditor;
#endif

    [AddComponentMenu("RaycastPro/Rey Sensors/" + nameof(PipeRay))]
    public sealed class PipeRay : RaySensor, IRadius
    {
        [SerializeField] private float radius = .4f;
        public float Radius
        {
            get => radius;
            set => radius = Mathf.Max(0,value);
        }

        [SerializeField] private float height;
        public float Height
        {
            get => height;
            set => height = Mathf.Max(0, value);
        }
        protected override void OnCast()
        {
            if (height > 0)
            {
                var up = transform.up * height/2;
                Physics.CapsuleCast(transform.position+up, transform.position-up, radius, Direction, out hit, direction.magnitude,
                    detectLayer.value, triggerInteraction);
            }
            else
            {
                Physics.SphereCast(transform.position, radius, Direction, out hit, direction.magnitude,
                    detectLayer.value, triggerInteraction);
            }
        }
        
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Emit a pipe ray in the specified direction and return the Hit information." + HAccurate + HDirectional + HIRadius;
#pragma warning restore CS0414
        
        /// <summary>
        /// Hint: This command will make your references missing.
        /// </summary>
        [ContextMenu("Convert To BasicRay")]
        private async void ConvertToBasicRay()
        {
            var _ray = Undo.AddComponent<BasicRay>(gameObject);
            
            _ray.direction = direction;
            
            await Task.Delay(1);
            
            Undo.DestroyObjectImmediate (this);
        }
        /// <summary>
        /// Hint: This command will make your references missing.
        /// </summary>
        [ContextMenu("Convert To BoxRay")]
        private async void ConvertToBoxRay()
        {
            var _ray = Undo.AddComponent<BoxRay>(gameObject);
            
            _ray.direction = direction;
            _ray.extents = new Vector3(radius, height+radius, radius);

            await Task.Delay(1);
            
            Undo.DestroyObjectImmediate (this);
        }

        internal override void OnGizmos()
        {
            EditorUpdate();

            var position = transform.position;

            Handles.color = Performed ? DetectColor : DefaultColor;
            
            DrawCapsuleLine(position, position + Direction, radius, height, _t: transform);
            
            Handles.color = DetectColor;

            if (Performed) DrawNormal(Hit);
        }

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                DirectionField(_so);
                RadiusField(_so);
                HeightField(_so);
            }

            if (hasGeneral) GeneralField(_so);

            if (hasEvents) EventField(_so);

            if (hasInfo) InformationField();
        }
#endif
        private Vector3 radiusVector => direction.normalized * radius;
        public override Vector3 Tip => transform.position + Direction;
        
        public Vector3 RadiusBase => BasePoint - radiusVector;
        
        public Vector3 RadiusTip => Tip + radiusVector;

        public override float RayLength => direction.magnitude + radius;
        public override Vector3 BasePoint => transform.position;
    }
}