

namespace RaycastPro.RaySensors
{
#if UNITY_EDITOR
    using UnityEditor;
    using System.Threading.Tasks;
#endif

    using UnityEngine;

    [AddComponentMenu("RaycastPro/Rey Sensors/" + nameof(BasicRay))]
    public sealed class BasicRay : RaySensor
    {
        protected override void OnCast() => Physics.Raycast(transform.position, local ? LocalDirection : direction, out hit,
            DirectionLength, detectLayer.value, triggerInteraction);

#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Emit single line Ray in the specified direction and return the Hit information." +
                                     HAccurate + HDirectional;
#pragma warning restore CS0414
        
        /// <summary>
        /// Hint: This command will make your references missing.
        /// </summary>
        [ContextMenu("Convert To PipeRay")]
        private async void ConvertToPipeRay()
        {
            var _ray = Undo.AddComponent<PipeRay>(gameObject);
            _ray.direction = direction;
            await Task.Delay(1);
            Undo.DestroyObjectImmediate(this);
        }
        [ContextMenu("Convert To BoxRay")]
        private async void ConvertToBoxRay()
        {
            var _ray = Undo.AddComponent<BoxRay>(gameObject);
            _ray.direction = direction;
            await Task.Delay(1);
            Undo.DestroyObjectImmediate(this);
        }

        private Vector3 _p;
        internal override void OnGizmos()
        {
            EditorUpdate();
            
            _p = transform.position;
            Gizmos.color = Performed ? DetectColor : DefaultColor;
            if (IsManuelMode)
            {
                Gizmos.DrawRay(transform.position, Direction);
            }
            else
            {
                DrawBlockLine(_p, _p + Direction, hit);
            }
            DrawNormal(hit);
        }

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain) DirectionField(_so);
            if (hasGeneral) GeneralField(_so);
            if (hasEvents) EventField(_so);
            if (hasInfo) InformationField();
        }
#endif
        public override Vector3 Tip => transform.position + Direction;
        public override float RayLength => direction.magnitude;
        public override Vector3 Base => transform.position;
    }
}