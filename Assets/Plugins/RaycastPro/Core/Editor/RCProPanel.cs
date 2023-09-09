#if UNITY_EDITOR
namespace RaycastPro.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Bullets;
    using Bullets2D;
    using Casters;
    using Casters2D;
    using Detectors;
    using Detectors2D;
    using Planers;
    using Planers2D;
    using RaySensors;
    using RaySensors2D;
    using UnityEditor;
    using UnityEngine;
    public sealed class RCProPanel : EditorWindow
    {
        internal const string KEY = "RaycastPro_Key : ";
        internal const string CResourcePath = "Resource_Path";
        internal const string CShowOnStart = "Show On Startup";
        private const int width = 460;
        private const int height = 600;

        internal static Mode mode = Mode.TwoD;
        internal static CoreMode coreMode = CoreMode.RaySensors;

        internal static bool showOnStart;
        [SavePreference] internal static bool realtimeEditor = true;
        
        [SavePreference] internal static Color DefaultColor = RCProEditor.Aqua;
        [SavePreference] internal static Color DetectColor = new Color(.3f, 1, .3f, 1f);
        [SavePreference] internal static Color HelperColor = new Color(1f, .7f, .0f, 1f);
        [SavePreference] internal static Color BlockColor = new Color(1f, .2f, .2f, 1f);

        [SavePreference] internal static float defaultValue = .4f;
        [SavePreference] internal static float normalDiscRadius = .2f;
        [SavePreference] internal static float elementDotSize = .05f;
        [SavePreference] internal static float alphaAmount = .2f;

        [SavePreference] internal static float raysStepSize = 4f;
        [SavePreference] internal static float normalFilterRadius = 1f;
        [SavePreference] internal static float linerMaxWidth = 1f;
        [SavePreference] internal static int linerMaxCapVertices = 6;

        [SavePreference] internal static bool DrawBlockLine = true;
        [SavePreference] internal static bool DrawDetectLine = true;
        [SavePreference] internal static bool DrawGuide = true;
        [SavePreference] internal static bool ShowLabels = true;
        [SavePreference] internal static int DrawGuideLimitCount = 50;
        
        [SavePreference] internal static int maxSubdivideTime = 6;

        [SavePreference] internal static bool drawHierarchyIcons = true;
        [SavePreference] internal static int hierarchyIconsOffset = 100;

        private static readonly bool[] settingFoldout = new bool[5];
        internal static Dictionary<Type, Texture2D> ICON_DICTIONARY = new Dictionary<Type, Texture2D>();
        internal static EditorWindow window;
        internal static bool LoadWhenOpen = false;
        private Texture2D headerTexture;
        private Vector2 scrollPos;
        /// DONT TOUCH ME ALERT :) This is not bug, You can browse the resource folder externally from panel :).
        internal static string ResourcePath => EditorPrefs.GetString(CResourcePath, "Assets/Plugins/RaycastPro/Resources");

        private void OnEnable()
        {
            LoadPreferences();

            showOnStart = EditorPrefs.GetBool(KEY + CShowOnStart, true);

            headerTexture = IconManager.GetHeader();

            RefreshIcons();
        }
        private void OnDisable()
        {
            SavePreferences();

            EditorPrefs.SetBool(KEY + CShowOnStart, showOnStart);
        }
        private void OnGUI()
        {
            GUI.color = Color.white;

            var boxStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.UpperCenter
            };
            if (headerTexture) GUILayout.Box(headerTexture, boxStyle, GUILayout.Width(width), GUILayout.Height(153));

            RCProEditor.GUILine(RCProEditor.Aqua);

            var labelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.UpperCenter,
                richText = true
            };

            GUILayout.Label("<b>RAYCAST_PRO V1.0</b> developed by <color=#2BC6D2>KIYNL</color>", labelStyle);

            RCProEditor.GUILine(RCProEditor.Aqua);
            #region Content Buttons
            GUI.contentColor = RCProEditor.Aqua;
            GUI.backgroundColor = RCProEditor.Violet;
            var enumInt = GUILayout.SelectionGrid((int) mode, new[] {"2D", "3D"}, 2);
            mode = (Mode) Enum.ToObject(typeof(Mode), enumInt);
            var coreInt = GUILayout.SelectionGrid((int) coreMode, Enum.GetNames(typeof(CoreMode)), 5);
            coreMode = (CoreMode) Enum.ToObject(typeof(CoreMode), coreInt);
            var cores = new List<Type>();
            if (mode == Mode.ThreeD)
                switch (coreMode)
                {
                    case CoreMode.RaySensors:
                        cores = new List<Type>
                        {
                            typeof(BasicRay), typeof(PipeRay), typeof(BoxRay), typeof(RadialRay), typeof(ChainRay),
                            typeof(ReflectRay),
                            typeof(TargetRay), typeof(PointerRay), typeof(WaveRay), typeof(ArcRay), typeof(HybridRay)
                        };
                        break;
                    case CoreMode.Detectors:
                        cores = new List<Type>
                        {
                            typeof(LineDetector), typeof(RangeDetector), typeof(BoxDetector), typeof(TargetDetector),
                            typeof(RadarDetector), typeof(SightDetector), typeof(SteeringDetector),
                            typeof(SoundDetector), typeof(LightDetector), typeof(PathDetector)
                        };
                        break;
                    case CoreMode.Planers:
                        cores = new List<Type>
                        {
                            typeof(BlockPlanar), typeof(ReflectPlanar), typeof(RefractPlanar), typeof(PortalPlanar),
                        };
                        break;
                    case CoreMode.Casters:
                        cores = new List<Type>
                            {typeof(BasicCaster), typeof(AdvanceCaster)};
                        break;
                    case CoreMode.Bullets:
                        cores = new List<Type>
                            {typeof(BasicBullet), typeof(InstantBullet), typeof(PhysicalBullet), typeof(TrackerBullet), typeof(PathBullet)};
                        break;
                }
            else if (mode == Mode.TwoD)
                switch (coreMode)
                {
                    case CoreMode.RaySensors:
                        cores = new List<Type>
                        {
                            typeof(BasicRay2D), typeof(PipeRay2D), typeof(BoxRay2D), typeof(RadialRay2D),
                            typeof(ChainRay2D), typeof(ReflectRay2D),
                            typeof(TargetRay2D), typeof(PointerRay2D), typeof(WaveRay2D), typeof(ArcRay2D), typeof(HybridRay2D)
                        };
                        break;
                    case CoreMode.Detectors:
                        cores = new List<Type>
                        {
                            typeof(LineDetector2D), typeof(RangeDetector2D), typeof(BoxDetector2D),
                            typeof(TargetDetector2D),
                            typeof(SteeringDetector2D), typeof(RadarDetector2D), typeof(PathDetector2D)
                        };
                        break;
                    case CoreMode.Planers:
                        cores = new List<Type>
                        {
                            typeof(BlockPlanar2D), typeof(ReflectPlanar2D), typeof(RefractPlanar2D),
                            typeof(PortalPlanar2D)
                        };
                        break;
                    case CoreMode.Casters:
                        cores = new List<Type>
                        {
                            typeof(BasicCaster2D), typeof(AdvanceCaster2D)
                        };
                        break;
                    case CoreMode.Bullets:
                        cores = new List<Type>
                        {
                            typeof(BasicBullet2D), typeof(InstantBullet2D), typeof(PhysicalBullet2D), typeof(TrackerBullet2D),
                            typeof(PathBullet2D)
                        };
                        break;
                }

            #endregion

            IconLayout(cores, 7);
            RCProEditor.GUILine(Color.white);
            GUILayout.Space(2);

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            // UNDO SYSTEM
            EditorGUI.BeginChangeCheck();

            settingFoldout[0] = EditorGUILayout.BeginFoldoutHeaderGroup(settingFoldout[0],
                "General Settings".ToContent(), RCProEditor.HeaderFoldout());
            if (settingFoldout[0])
            {
                EditorGUILayout.BeginVertical(RCProEditor.BoxStyle);
                
                EditorGUI.BeginChangeCheck();
                realtimeEditor = EditorGUILayout.Toggle("Realtime Editor", realtimeEditor);
                if (EditorGUI.EndChangeCheck())
                {
                    IconDrawer.SetEvent(drawHierarchyIcons);
                    SceneView.RepaintAll();
                    Debug.Log(RCProEditor.RPro + $"Realtime Editor <color=#B794FF>{(realtimeEditor ? "Enabled" : "Disabled")}</color>.");
                }
                
                EditorGUI.BeginChangeCheck();
                drawHierarchyIcons = EditorGUILayout.Toggle("Draw Hierarchy Icons", drawHierarchyIcons);
                if (EditorGUI.EndChangeCheck())
                {
                    IconDrawer.SetEvent(drawHierarchyIcons);
                    EditorApplication.RepaintHierarchyWindow();
                    Debug.Log(RCProEditor.RPro + $"Hierarchy Icons <color=#B794FF>{(drawHierarchyIcons ? "Enabled" : "Disabled")}</color>.");
                }
                
                if (drawHierarchyIcons)
                    hierarchyIconsOffset = EditorGUILayout.IntField("Icons Offset", hierarchyIconsOffset);
                
                EditorGUI.BeginChangeCheck();
                DefaultColor = EditorGUILayout.ColorField("Default Color", DefaultColor);
                DetectColor = EditorGUILayout.ColorField("Detect Color", DetectColor);
                HelperColor = EditorGUILayout.ColorField("Helper Color", HelperColor);
                BlockColor = EditorGUILayout.ColorField("Block Color", BlockColor);

                defaultValue = EditorGUILayout.FloatField("Default Value", defaultValue);
                raysStepSize = EditorGUILayout.FloatField("Dotted Rays Step", raysStepSize);
                normalDiscRadius = EditorGUILayout.FloatField("Normal Disc Radius", normalDiscRadius);
                elementDotSize = EditorGUILayout.FloatField("Element Dot Size", elementDotSize);
                if (EditorGUI.EndChangeCheck()) SceneView.RepaintAll();
                
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            GUILayout.Space(2);

            settingFoldout[1] = EditorGUILayout.BeginFoldoutHeaderGroup(settingFoldout[1], "RaySensors".ToContent(),
                RCProEditor.HeaderFoldout());
            if (settingFoldout[1])
            {
                EditorGUILayout.BeginVertical(RCProEditor.BoxStyle);
                normalFilterRadius =
                    EditorGUILayout.FloatField("Normal Filter Radius", Mathf.Max(0, normalFilterRadius));
                linerMaxWidth = EditorGUILayout.FloatField("Liner Max Width", Mathf.Max(0, linerMaxWidth));
                linerMaxCapVertices =
                    EditorGUILayout.IntField("Liner Max Cap vertices", Mathf.Max(1, linerMaxCapVertices));
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            GUILayout.Space(2);

            settingFoldout[2] = EditorGUILayout.BeginFoldoutHeaderGroup(settingFoldout[2], "Detectors".ToContent(),
                RCProEditor.HeaderFoldout());
            if (settingFoldout[2])
            {
                EditorGUILayout.BeginVertical(RCProEditor.BoxStyle);
                EditorGUI.BeginChangeCheck();
                DrawBlockLine = EditorGUILayout.Toggle("Draw Block Line", DrawBlockLine);
                if (EditorGUI.EndChangeCheck()) SceneView.RepaintAll();

                EditorGUI.BeginChangeCheck();
                DrawDetectLine = EditorGUILayout.Toggle("Draw Detect Line", DrawDetectLine);
                if (EditorGUI.EndChangeCheck()) SceneView.RepaintAll();

                EditorGUILayout.BeginHorizontal();
                
                EditorGUI.BeginChangeCheck();
                DrawGuide = EditorGUILayout.Toggle("Draw Guide", DrawGuide);
                if (EditorGUI.EndChangeCheck()) SceneView.RepaintAll();

                EditorGUI.BeginChangeCheck();
                DrawGuideLimitCount = EditorGUILayout.IntField("Limit Count", DrawGuideLimitCount);
                if (EditorGUI.EndChangeCheck()) SceneView.RepaintAll();
                EditorGUILayout.EndHorizontal();

                EditorGUI.BeginChangeCheck();
                ShowLabels = EditorGUILayout.Toggle("Draw Labels", ShowLabels);
                if (EditorGUI.EndChangeCheck()) SceneView.RepaintAll();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            GUILayout.Space(2);
            RCProEditor.GUILine(Color.white);
            if (GUILayout.Button("Browse Icons Folder"))
            {
                var path = EditorUtility.OpenFolderPanel("Select Resource folder", ResourcePath, "")
                    .Replace(Application.dataPath, "Assets");

                EditorPrefs.SetString(CResourcePath, path);
            }
            if (GUILayout.Button("Refresh Icons")) RefreshIcons();
            if (GUILayout.Button("Reset Settings")) ResetSettings();
            GUILayout.EndScrollView();
            GUILayout.Space(2);
            RCProEditor.GUILine(Color.white);
            GUILayout.Space(2);
            GUILayout.Label("Copyright all rights reserved", labelStyle);
            GUILayout.Space(2);
            RCProEditor.GUILine(Color.white);
            showOnStart = EditorGUILayout.Toggle("Show Panel When Start", showOnStart);
            RCProEditor.GUILine(Color.white);
        }

        [MenuItem("Tools/RaycastPro Panel", priority = -10000)]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            window = GetWindow(typeof(RCProPanel), true, "RaycastPro Panel", true);

            window.maxSize = new Vector2(width, height);
            window.minSize = new Vector2(width, height);

            window.Show();

            ICON_DICTIONARY = IconManager.GetIcons();

            mode = SceneView.lastActiveSceneView.in2DMode ? Mode.TwoD : Mode.ThreeD;
        }
        private static void ResetSettings()
        {
            DefaultColor = RCProEditor.Aqua;
            DetectColor = new Color(.3f, 1, .3f, 1f);
            HelperColor = new Color(1f, .7f, .0f, 1f);
            BlockColor = new Color(1f, .2f, .2f, 1f);

            drawHierarchyIcons = true;
            hierarchyIconsOffset = 100;

            normalFilterRadius = 5f;
            raysStepSize = 4f;

            defaultValue = .4f;
            normalDiscRadius = .2f;
            elementDotSize = .05f;
            alphaAmount = .2f;

            raysStepSize = 4f;
            normalFilterRadius = 1f;
            linerMaxWidth = 1f;
            linerMaxCapVertices = 6;

            DrawBlockLine = true;
            DrawDetectLine = true;
            DrawGuide = true;
            ShowLabels = true;
            maxSubdivideTime = 6;
        }
        public static void SavePreferences()
        {
            foreach (var fieldInfo in typeof(RCProPanel).GetFields())
            {
                if (fieldInfo.GetCustomAttribute(typeof(SavePreference)) == null) continue;

                if (fieldInfo.FieldType == typeof(bool))
                    EditorPrefs.SetBool(KEY + fieldInfo.Name, (bool) fieldInfo.GetValue(null));
                else if (fieldInfo.FieldType == typeof(float))
                    EditorPrefs.SetFloat(KEY + fieldInfo.Name, (float) fieldInfo.GetValue(null));
                else if (fieldInfo.FieldType == typeof(int))
                    EditorPrefs.SetInt(KEY + fieldInfo.Name, (int) fieldInfo.GetValue(null));
                else if (fieldInfo.FieldType == typeof(string))
                    EditorPrefs.SetString(KEY + fieldInfo.Name, (string) fieldInfo.GetValue(null));
                else if (fieldInfo.FieldType == typeof(Color))
                    SaveColor(KEY + fieldInfo.Name, (Color) fieldInfo.GetValue(null));
            }
        }
        public static void LoadPreferences(bool message = false)
        {
            if (message) Debug.Log(RCProEditor.RPro+"<color=#00FF00>Preferences Update.</color>");

            foreach (var fieldInfo in typeof(RCProPanel).GetFields())
            {
                if (fieldInfo.GetCustomAttribute(typeof(SavePreference)) == null) continue;

                if (!EditorPrefs.HasKey(KEY + fieldInfo.Name)) continue;

                if (fieldInfo.FieldType == typeof(bool))
                {
                    fieldInfo.SetValue(null, EditorPrefs.GetBool(KEY + fieldInfo.Name));

                    Debug.Log(fieldInfo.Name);
                }
                else if (fieldInfo.FieldType == typeof(float))
                {
                    fieldInfo.SetValue(null, EditorPrefs.GetFloat(KEY + fieldInfo.Name));
                }
                else if (fieldInfo.FieldType == typeof(int))
                {
                    fieldInfo.SetValue(null, EditorPrefs.GetInt(KEY + fieldInfo.Name));
                }
                else if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(null, EditorPrefs.GetString(KEY + fieldInfo.Name));
                }
                else if (fieldInfo.FieldType == typeof(Color))
                {
                    fieldInfo.SetValue(null, LoadColor(KEY + fieldInfo.Name));
                }
            }
        }
        private static void SaveColor(string key, Color color)
        {
            EditorPrefs.SetBool(key, true);
            EditorPrefs.SetFloat(key + "R", color.r);
            EditorPrefs.SetFloat(key + "G", color.g);
            EditorPrefs.SetFloat(key + "B", color.b);
            EditorPrefs.SetFloat(key + "A", color.a);
        }
        private static Color LoadColor(string key)
        {
            var col = new Color
            {
                r = EditorPrefs.GetFloat(key + "R"),
                g = EditorPrefs.GetFloat(key + "G"),
                b = EditorPrefs.GetFloat(key + "B"),
                a = EditorPrefs.GetFloat(key + "A")
            };
            return col;
        }
        public static void RefreshIcons()
        {
            ICON_DICTIONARY = IconManager.GetIcons();
            
            foreach (var type in ICON_DICTIONARY.Keys.ToList())
            {
                if (type.IsAbstract) continue;

                try
                {
                    var obj = new GameObject();
                    
                    var component = obj.AddComponent(type);
                    
                    component.SetIcon(ICON_DICTIONARY[type]);
                    
                    DestroyImmediate(obj);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

        }
        public static void IconLayout(List<Type> types, int columnWidth)
        {
            var rows = types.Count / columnWidth;

            var guiStyle = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter
            };

            EditorGUILayout.BeginVertical(guiStyle);

            for (var i = 0; i <= rows; i++)
            {
                EditorGUILayout.BeginHorizontal(guiStyle);
                for (var j = 0; j < columnWidth; j++)
                {
                    var index = i * columnWidth + j;
                    if (index > types.Count - 1)
                    {
                        if (j == 0) break;
                        
                        GUILayout.Box("", GUILayout.Width(60), GUILayout.Height(60));
                    }
                    else
                    {
                        Button(types[index]);
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
        public static bool Button(Type type)
        {
            var infoFiled = type.GetField("Info", BindingFlags.Static | BindingFlags.NonPublic);

            var info = infoFiled != null ? infoFiled.GetValue(null).ToString() : "No information";
            var content = ICON_DICTIONARY.Keys.Contains(type)
                ? new GUIContent(ICON_DICTIONARY[type], info)
                : new GUIContent(type.Name, info);

            var style = new GUIStyle(GUI.skin.button)
            {
                stretchWidth = false,
                border = new RectOffset(0, 0, 0,0),
                margin = new RectOffset(6, 6, 6, 6),
                padding = new RectOffset(4, 4, 4, 4),
                wordWrap = false,
            };
            
            EditorGUILayout.BeginVertical();
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
            
            var click = GUILayout.Button(content, style, GUILayout.Width(56), GUILayout.Height(56));

            var name = type.Name;

            if (type.IsSubclassOf(typeof(RaySensor))) name = name.Replace("Ray", "");
            else if (type.IsSubclassOf(typeof(RaySensor2D))) name = name.Replace("Ray2D", "");
            else if (type.IsSubclassOf(typeof(Detector))) name = name.Replace("Detector", "");
            else if (type.IsSubclassOf(typeof(Detector2D))) name = name.Replace("Detector2D", "");
            else if (type.IsSubclassOf(typeof(Planar))) name = name.Replace("Planar", "");
            else if (type.IsSubclassOf(typeof(Planar2D))) name = name.Replace("Planar2D", "");
            else if (type.IsSubclassOf(typeof(BaseCaster)))
            {
                name = name.Replace("Caster", "");
                name = name.Replace("2D", "");
            }
            else if (type.IsSubclassOf(typeof(Bullet)))
            {
                name = name.Replace("Bullet", "");
            }
            else if (type.IsSubclassOf(typeof(Bullet2D)))
            {
                name = name.Replace("Bullet2D", "");
            }

            style = RCProEditor.BoxStyle;
            style.wordWrap = true;
            style.padding = new RectOffset(0, 0, 0, 0);
            style.border = new RectOffset(0, 0, 0, 0);
            style.margin = new RectOffset(6, 6, 2, 2);
            
            GUILayout.Box($"<color=#2BC6D2>{name.ToRegex()}</color>", style, GUILayout.Width(60), GUILayout.Height(20));
            EditorGUILayout.EndVertical();
            GUI.contentColor = RCProEditor.Aqua;
            GUI.backgroundColor = RCProEditor.Violet;

            if (click) CreateCore(type);
            return click;
        }
        private static void CreateCore(Type type)
        {
            var obj = new GameObject();

            if (type.IsSubclassOf(typeof(Planar)))
            {
                DestroyImmediate(obj);
                obj = GameObject.CreatePrimitive(PrimitiveType.Quad);
                obj.transform.localScale = new Vector3(10f, 10f, 1f);
            }
            else if (type.IsSubclassOf(typeof(Planar2D)))
            {
                var spriteRenderer = obj.AddComponent<SpriteRenderer>();

                var tex = new Texture2D(20, 200);
                var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f),
                    100.0f);

                sprite.name = type.Name;

                spriteRenderer.sprite = sprite;
                spriteRenderer.color = DefaultColor;

                obj.transform.localScale = new Vector3(1, 1f, 1f);

                var boxCollider = obj.AddComponent<BoxCollider2D>();

                boxCollider.size = new Vector2(.2f, 2);
            }
            else if (type.IsSubclassOf(typeof(Bullet)))
            {
                DestroyImmediate(obj);

                obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

                obj.transform.localScale = new Vector3(.4f, .4f, 1f);
            }
            else if (type.IsSubclassOf(typeof(Bullet2D)))
            {
                var spriteRenderer = obj.AddComponent<SpriteRenderer>();

                var tex = new Texture2D(100, 60);
                var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f),
                    100.0f);

                sprite.name = type.Name;

                spriteRenderer.sprite = sprite;

                spriteRenderer.color = DefaultColor;
                obj.transform.localScale = new Vector3(1, 1f, 1f);

                var boxCollider = obj.AddComponent<BoxCollider2D>();

                boxCollider.size = new Vector2(1, .6f);
            }

            obj.name = type.Name.ToRegex();

            Undo.RegisterCreatedObjectUndo(obj, "create_core, ID: " + obj.GetInstanceID());

            var camera = SceneView.lastActiveSceneView.camera.transform;

            obj.transform.position = camera.position + camera.forward * 10f;

            obj.AddComponent(type);

            var activeSelection = Selection.activeTransform;

            if (activeSelection)
            {
                obj.transform.parent = activeSelection;
                obj.transform.localPosition = Vector3.zero;
                obj.transform.localRotation = Quaternion.identity;
                obj.transform.localScale = Vector3.one;
            }

            Selection.activeTransform = obj.transform;
        }

        [AttributeUsage(AttributeTargets.Field)]
        internal class SavePreference : Attribute { }

        internal enum CoreMode
        {
            RaySensors,
            Detectors,
            Planers,
            Casters,
            Bullets
        }

        internal enum Mode
        {
            TwoD,
            ThreeD
        }
    }
}
#endif