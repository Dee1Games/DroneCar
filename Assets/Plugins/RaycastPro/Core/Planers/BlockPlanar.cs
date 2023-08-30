namespace RaycastPro.Planers
{
    using RaySensors;
    using UnityEngine;

#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif
    [AddComponentMenu("RaycastPro/Planers/" + nameof(BlockPlanar))]
    public sealed class BlockPlanar : Planar
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Receiving and blocking impact from any Planar Sensitive Ray";
#pragma warning restore CS0414

        internal override void OnGizmos() => DrawPlanar();
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                DetectLayerField(_so);
            }
            if (hasGeneral)
            {
                BaseField(_so, hasInteraction: false, hasUpdateMode: false);
            }
            
            if (hasEvents)
            {
                EventFoldout =
                    EditorGUILayout.BeginFoldoutHeaderGroup(EventFoldout, CEvents, RCProEditor.HeaderFoldout());

                if (EventFoldout)
                {
                    RCProEditor.EventField(new SerializedObject(this), new[]
                    {
                        nameof(onReceiveRay), nameof(onBeginReceiveRay), nameof(onEndReceiveRay)
                    });
                }
            }
        }
#endif

        internal override TransitionData[] GetTransitionData(RaycastHit hit, Vector3 direction)
        { 
            return new TransitionData[] { };
        }
        internal override void OnBeginReceiveRay(RaySensor sensor) {}
        internal override void OnReceiveRay(RaySensor sensor) { }
    }
}