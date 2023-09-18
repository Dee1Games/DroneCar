namespace RaycastPro
{
    using System.Linq;
    using UnityEngine;
#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif

    [AddComponentMenu("RaycastPro/Utility/" + nameof(RayManager))]
    public class RayManager : RaycastCore
    {
        public Transform referenceObject;
        [SerializeField] private RaycastCore[] cores;

        [SerializeField] private bool[] _bools;

        public override bool Performed
        {
            get => cores.All(r => r.Performed);
            protected set { }
        }

        protected void Reset()
        {
            if (!referenceObject) referenceObject = transform;
            cores = referenceObject.GetComponentsInChildren<RaycastCore>(true).Where(c => !(c is RayManager)).ToArray();
            _bools = new bool[cores.Length];
        }

        protected override void OnCast() { }

#if UNITY_EDITOR
        
        protected override void AfterValidate()
        {
            Reset();
        }

#pragma warning disable CS0414
        private static string Info = "The ray control and management tool automatically detects children rays."+HUtility+HDependent;
#pragma warning restore CS0414
        internal override void OnGizmos() { }

        private GUIStyle foldoutHeadStyle;

        [SerializeField]
        private bool showMain = true;
        [SerializeField]
        private bool showGeneral = false;

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(referenceObject)));
            if (GUILayout.Button("Refresh"))
            {
                Reset();
            }
            BeginVerticalBox();
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(showMain)));
            EditorGUILayout.PropertyField(_so.FindProperty(nameof(showGeneral)));
            EndVertical();
            if (!referenceObject) return;

            for (var index = 0; index < cores.Length; index++)
            {
                var core = cores[index];
                EditorGUILayout.BeginVertical(new GUIStyle
                {
                    margin = new RectOffset(0, 0, 2, 2),
                    padding = new RectOffset(0, 0, 0, 2),
                    stretchWidth = true,
                    wordWrap = true,
                });
                
                foldoutHeadStyle = new GUIStyle(RCProEditor.HeaderFoldout)
                {
                    margin = new RectOffset(18, 4,0, 0),
                    padding = new RectOffset(0, 0, 0, 0),
                    clipping = TextClipping.Clip,
                    stretchWidth = false,
                    fixedWidth = EditorGUIUtility.currentViewWidth/3,
                };

                BeginVerticalBox();
                
                EditorGUILayout.BeginHorizontal(new GUIStyle
                {
                    margin = new RectOffset(1, 1, 4, 4),
                    padding = new RectOffset(5, 5, 4, 4),
                    alignment = TextAnchor.MiddleCenter, wordWrap = true
                }, GUILayout.ExpandWidth(false));

                _bools[index] = EditorGUILayout.BeginFoldoutHeaderGroup(_bools[index], core.name,
                    foldoutHeadStyle, rect => { });

                
                cores[index].gameObject.SetActive(EditorGUILayout.Toggle(cores[index].gameObject.activeInHierarchy, GUILayout.Width(20)));
                cores[index].enabled = EditorGUILayout.Toggle(cores[index].enabled, GUILayout.Width(20));

                if (cores[index].gameObject.activeInHierarchy)
                {
                    var _cSO = new SerializedObject(cores[index]);
                    _cSO.Update();
                    var prop = _cSO.FindProperty("gizmosUpdate");
                    
                    if (GUILayout.Button(cores[index].gizmosUpdate.ToString(), GUILayout.Width(60f)))
                    {
                        switch (cores[index].gizmosUpdate)
                        {
                            case GizmosMode.Select:
                                prop.enumValueIndex = 0;
                                break;
                            case GizmosMode.Auto:
                                prop.enumValueIndex = 2;
                                break;
                            case GizmosMode.Fix:
                                prop.enumValueIndex = 3;
                                break;
                            case GizmosMode.Off:
                                prop.enumValueIndex = 1;
                                break;
                        }
                        _cSO.ApplyModifiedProperties();
                    }
                }
                else
                {
                    GUILayout.Box("Off", RCProEditor.BoxStyle, GUILayout.Width(60), GUILayout.Height(20));
                }

                GUI.backgroundColor = (core.Performed ? DetectColor : BlockColor).ToAlpha(1);
                if (GUILayout.Button("Cast", GUILayout.Width(60f)))
                {
                    core.TestCast();
                }

                
                //GUILayout.Box(raySensor.Performed ? "<color=#61FF38>✓</color>" : "<color=#FF3822>x</color>", RCProEditor.BoxStyle, GUILayout.Width(40), GUILayout.Height(20));
                EndHorizontal();
                GUI.backgroundColor = RCProEditor.Violet;
                
            if (_bools[index])
            {
                var _cSO = new SerializedObject(core);
                _cSO.Update();
                EditorGUI.BeginChangeCheck();
                core.EditorPanel(_cSO, showMain, showGeneral, false, false);
                if (EditorGUI.EndChangeCheck()) _cSO.ApplyModifiedProperties();
            }
            
            EditorGUILayout.EndFoldoutHeaderGroup();
            EndVertical();
            EndVertical();
            }
        }
#endif
    }
}