using System;
using System.Linq;
using RaycastPro.RaySensors;
using UnityEngine;

namespace RaycastPro
{
#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif


    public class RayManager : RaycastCore
    {
        [SerializeField] private RaySensor[] raySensors;

        [SerializeField] private bool[] _bools;

        public override bool Performed
        {
            get => raySensors.All(r => r.Performed);
            protected set { }
        }

        protected void Reset()
        {
            raySensors = GetComponentsInChildren<RaySensor>();
            _bools = new bool[raySensors.Length];
        }

        protected override void OnCast() { }

#if UNITY_EDITOR

        protected override void AfterValidate()
        {
            
        }

#pragma warning disable CS0414
        private static string Info = "The ray control and management tool automatically detects children rays.";
#pragma warning restore CS0414
        internal override void OnGizmos()
        {
        }

        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            for (var index = 0; index < raySensors.Length; index++)
            {
                var raySensor = raySensors[index];

                EditorGUILayout.BeginVertical(new GUIStyle
                {
                    padding = new RectOffset(5, 5, 2, 2)
                });

                EditorGUILayout.BeginHorizontal(new GUIStyle
                {
                    margin = new RectOffset(1, 1, 1, 1),
                    padding = new RectOffset(5, 5, 2, 2),
                    alignment = TextAnchor.MiddleCenter, wordWrap = true
                });

                var style = EditorStyles.foldoutHeader;
                style.margin = new RectOffset(8, 8, 0, 0);
                
                _bools[index] = EditorGUILayout.BeginFoldoutHeaderGroup(_bools[index], raySensor.name,
                    style, rect => { });
                
                raySensors[index].gameObject.SetActive(EditorGUILayout.Toggle(raySensors[index].gameObject.activeInHierarchy, GUILayout.Width(20f)));

                if (raySensors[index].gameObject.activeInHierarchy)
                {
                    if (GUILayout.Button(raySensors[index].gizmosUpdate.ToString(), GUILayout.Width(60f)))
                    {
                        switch (raySensors[index].gizmosUpdate)
                        {
                            case GizmosMode.Select:
                            case GizmosMode.Auto:
                                raySensors[index].gizmosUpdate = GizmosMode.Fix;
                                break;
                            case GizmosMode.Fix:
                                raySensors[index].gizmosUpdate = GizmosMode.Off;
                                break;
                            case GizmosMode.Off:
                                raySensors[index].gizmosUpdate = GizmosMode.Auto;
                                break;
                        }
                    }
                }
                else
                {
                    GUILayout.Box("Manuel", RCProEditor.BoxStyle, GUILayout.Width(60f),
                    GUILayout.Height(20));
                }
                GUI.backgroundColor = (raySensor.Performed ? DetectColor : BlockColor).ToAlpha(1);
                GUILayout.Box(raySensor.Performed ? "<color=#61FF38>D</color>" : "<color=#FF3822>N</color>", RCProEditor.BoxStyle, GUILayout.Width(40f),
                    GUILayout.Height(20));
                EndHorizontal();

                GUI.backgroundColor = RCProEditor.Violet;

                if (_bools[index]) raySensor.EditorPanel(_so, hasMain: hasMain, hasGeneral: true, hasEvents: false, hasInfo: false);

                EditorGUILayout.EndFoldoutHeaderGroup();

                EndVertical();
            }
        }
#endif
    }
}