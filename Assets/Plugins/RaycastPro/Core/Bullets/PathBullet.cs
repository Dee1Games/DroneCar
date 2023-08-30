
namespace RaycastPro.Bullets
{
    using System.Collections.Generic;
    using UnityEngine;
    using RaySensors;


#if UNITY_EDITOR
    using UnityEditor;
#endif
    
    [AddComponentMenu("RaycastPro/Bullets/" + nameof(PathBullet))] 
    public sealed class PathBullet : Bullet, IPath<Vector3>
    {
        public List<Vector3> Path { get; internal set; } = new List<Vector3>();
        
        public float duration = 1;
        public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
        
        [SerializeField]
        private Rigidbody rigidBody;
        
        [SerializeField]
        private AxisRun axisRun = new AxisRun();

        private float pathLength;

        [SerializeField] private bool local = true;

        // Cached Variables
        private Vector3 _pos, _dir;
        
        protected override void OnCast()
        {
            if (raySource is PathRay _pathRay)
            {
                Path = local ? new List<Vector3>(_pathRay.PathPoints) : _pathRay.PathPoints;
            }
            else
            {
                Path = new List<Vector3> {raySource.BasePoint, raySource.Tip};
            }
            
            pathLength = Path.GetPathLength();
        }

        public override void RuntimeUpdate()
        {
            position = Mathf.Clamp01(position);
            float posM;

            if (moveType == MoveType.Curve)
            {
                posM = curve.Evaluate(position) * pathLength;
            }
            else
            {
                posM = position * pathLength;
            }
            var delta = GetModeDeltaTime(timeMode);
            UpdateLifeProcess(delta);
                
            switch (moveType)
            {
                case MoveType.Speed:
                    position += delta * speed / pathLength;
                    break;
                case MoveType.Duration:
                    position += delta / duration;
                    break;
                case MoveType.Curve:
                    position += delta / duration;
                    break;
            }

            if (position >= 1) OnEnd();
            
            for (var i = 1; i < Path.Count; i++)
            {
                var lineDistance = Path.GetPathLength(i);

                if (posM < lineDistance)
                {
                    _pos = Vector3.Lerp(Path[i - 1], Path[i], posM / lineDistance);
                    _dir = Path[i] - Path[i - 1];

                    break;
                }

                posM -= lineDistance;
            }
            if (rigidBody) rigidBody.MovePosition(_pos);
            else transform.position = _pos;
            
            CollisionRun(_dir, delta);
            
            if (axisRun.syncAxis) axisRun.SyncAxis(transform, _dir);
        }
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "A smart bullet that can recognize the path of the PathRay and move on it." + HAccurate + HDependent;
#pragma warning restore CS0414
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(local)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(rigidBody)));
                CastTypeField(
                    _so.FindProperty(nameof(moveType)),
                    _so.FindProperty(nameof(speed)), 
                    _so.FindProperty(nameof(duration)),
                    _so.FindProperty(nameof(curve)));
                axisRun.EditorPanel(_so.FindProperty(nameof(axisRun)));
            }

            if (hasGeneral) GeneralField(_so);

            if (hasEvents) EventField(_so);

            if (hasInfo) InformationField();
        }
#endif
    }
}