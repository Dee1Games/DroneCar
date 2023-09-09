namespace RaycastPro
{
    #if UNITY_EDITOR
        using Editor;
        using UnityEditor;
    #endif

    using UnityEngine.Events;

    public abstract class BaseSensor : RaycastCore
    {
        public UnityEvent onDetect;
#if UNITY_EDITOR
        protected void EventsField()
        {
            EventFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(EventFoldout, CEvents.ToContent(TEvents),
                RCProEditor.HeaderFoldout());
            if (EventFoldout)
            {
                RCProEditor.EventField(new SerializedObject(this), new[]
                {
                    nameof(onDetect)
                });
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
#endif
    }
}