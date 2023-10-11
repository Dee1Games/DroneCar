

namespace RaycastPro.Detectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
#if UNITY_EDITOR
    using Editor;
    using UnityEditor;
#endif

    [Serializable]
    public class AudioEvent : UnityEvent<AudioSource> { }

    [AddComponentMenu("RaycastPro/Detectors/" + nameof(SoundDetector))]
    public sealed class SoundDetector : Detector, IPulse
    {
        [Tooltip("Collider your sounds on the specified layer and assign a special Collider Detector to automatically feed the sounds to the filtering source.")]
        public ColliderDetector soundFinder;
        
        public List<AudioSource> sources = new List<AudioSource>();
        public readonly Dictionary<AudioSource, float> SoundLoudness = new Dictionary<AudioSource, float>();

        [Tooltip("The higher the hearing power, the lower the effect of wall thickness and distance.")]
        public float listeningPower = 1;
        [Tooltip("")]
        public int sampleWindow = 64;

        [Tooltip("Minimum loudness to pass detection.")]
        public float loudnessThreshold;

        [Tooltip("When it is enabled, the sound loudness calculation will be process, otherwise always return 1 when into range.")]
        public bool affectSoundLoudness = true;
        
        [Tooltip("The effect of the thickness of the wall is on the level of hearing, and as it increases, the power of hearing from behind the wall will decrease.")]
        public float wallThicknessInfluence = 2f;

        public AudioEvent onDetectSource;
        public AudioEvent onNewSource;
        public AudioEvent onLostSource;
        public override bool Performed
        {
            get => DetectedSources.Count > 0;
            protected set { }
        }

        #region Public Methods

        public AudioSource FirstMember => DetectedSources.First();
        
        public AudioSource NearestMember =>
            DetectedSources.OrderBy(s => (s.transform.position - transform.position).sqrMagnitude).First();
        
        public AudioSource FurthestMember =>
            DetectedSources.OrderBy(s => (s.transform.position - transform.position).sqrMagnitude).Last();




        public bool CheckHearing(Vector3 vector, float angle = 45f)
        {
            foreach (var detectedSource in DetectedSources)
            {
                var flat = Vector3.ProjectOnPlane(detectedSource.transform.position - transform.position, transform.up);
                if (Vector3.Angle(flat,vector) <= angle)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsHearingBack => CheckHearing(-transform.forward);
        public bool IsHearingRight => CheckHearing(transform.right);
        public bool IsHearingLeft => CheckHearing(-transform.right);
        public bool IsHearingForward => CheckHearing(transform.forward);
        
        public void EnableSource(AudioSource _s) => _s.enabled = true;
        public void DisableSource(AudioSource _s) => _s.enabled = false;
        public void MuteSource(AudioSource _s) => _s.mute = true;
        public void UnmuteSource(AudioSource _s) => _s.mute = false;
        public void MakeLoopEnable(AudioSource _s) => _s.loop = true;
        public void MakeLoopDisable(AudioSource _s) => _s.loop = false;
        #endregion

        #region Temps

        private float dist, loudness;

        #endregion
        private void Start() // Refreshing
        {
            DetectedSources = new List<AudioSource>();
            PreviousSources = Array.Empty<AudioSource>();
            Sync();
        }

        /// <summary>
        /// Sound Finder will be sync on source
        /// </summary>
        public void Sync()
        {
            soundFinder?.SyncDetection(sources);
        }

        public void UnSync()
        {
            soundFinder?.UnSyncDetection(sources);
        }
        public float GetLoudness(AudioClip clip, int position)
        {
            var startPosition = position - sampleWindow;
            if (startPosition < 0) return 0;
            var waveDate = new float[sampleWindow]; // ایجاد آرایه سمپلینگ صوت
            clip.GetData(waveDate, startPosition); // گرفتن سمپلینگ صدا و انتقال آن به آرایه
            loudness = 0;
            foreach (var f in waveDate) loudness += Mathf.Abs(f);
            return loudness / sampleWindow; // میانگین بلندی آرایه و خروج آن
        }

        private float distance, difference, _influence;
        private float GetSourceLoudness(AudioSource _source, in float _dis)
        {
            distance = _dis - _source.minDistance;
            difference = _source.maxDistance - _source.minDistance;
            _influence = 1 - Mathf.Clamp01(distance / difference);
            return GetLoudness(_source.clip, _source.timeSamples) * _source.volume * _influence;
        }
        protected override void OnCast()
        {
            PreviousSources = DetectedSources.ToArray();
#if UNITY_EDITOR
            GizmoGate = PanelGate = null;
#endif
            
            DetectedSources.Clear();
            SoundLoudness.Clear();
            foreach (var s in sources)
            {
                if (!s) continue;
                if (usingTagFilter && s.CompareTag(tagFilter)) continue;
                
                dist = Vector3.Distance(transform.position, s.transform.position);
                if (dist > s.maxDistance) continue; 
                Physics.Linecast(transform.position, s.transform.position, out var h1, detectLayer.value, triggerInteraction);
                Physics.Linecast(s.transform.position, transform.position, out var h2, detectLayer.value, triggerInteraction);
                
                loudness = (affectSoundLoudness ? GetSourceLoudness(s, dist) : 1f) * listeningPower;
                
                if (h1.transform && h2.transform)
                {
                    var hitDistance = Vector3.Distance(h1.point, h2.point);
                    loudness /= hitDistance * wallThicknessInfluence;
#if UNITY_EDITOR
                    GizmoGate += () =>
                    {
                        GizmoColor = DefaultColor;
                        DrawLine(transform.position, h1.point, this);
                        DrawLine(h2.point, s.transform.position, this);
                        GizmoColor = BlockColor;
                        DrawLine(h1.point, h2.point);
                    };
#endif
                }
                
                else if (h1.transform ^ h2.transform) continue;
                if (loudness < loudnessThreshold) continue;
                
                DetectedSources.Add(s);
                SoundLoudness.Add(s, loudness * listeningPower);
#if UNITY_EDITOR
                PanelGate += () =>
                {
                    EditorGUILayout.LabelField($"{s.name} {(s.clip ? s.clip.name : "N/A")} : {(loudness).ToString()}");
                };
                var _loudness = loudness;
                GizmoGate += () =>
                {
                    if (RCProPanel.ShowLabels)
                    {
                        Handles.Label(s.transform.position,
                            s.clip
                                ? s.transform.name + ": " + s.clip.name
                                : s.transform.name);
                    }

                    if (RCProPanel.DrawGuide)
                    {
                        Handles.color = HelperColor;
                        Handles.DrawWireDisc(s.transform.position, transform.up, s.maxDistance);
                        var disRange = s.maxDistance - s.minDistance;
                        var step = disRange / 4;
                        for (var i = 0; i < 4; i++)
                        {
                            var radius = step * (i + Time.realtimeSinceStartup) % disRange;
                            Handles.color = Color.Lerp(DefaultColor, DetectColor, radius / disRange).ToAlpha(_loudness * ClampedAlphaCharge * 2);
                            Handles.DrawWireDisc(s.transform.position, transform.up, radius + s.minDistance);
                        }
                    }
                };
#endif
            }
            #region Events Callback
            CallEvents(DetectedSources, PreviousSources, onDetectSource, onNewSource, onLostSource);
            #endregion
        }
#if UNITY_EDITOR
#pragma warning disable CS0414
        private static string Info = "Detection and filtering of scene sounds. In terms of sound reduction by sending Raycast to walls and obstacles."+HAccurate+HIPulse+HDependent;
#pragma warning restore CS0414
        
        private static readonly string[] events = {"onDetectSource", "onNewSource", "onLostSource"};
        internal override void OnGizmos() => EditorUpdate();
        internal override void EditorPanel(SerializedObject _so, bool hasMain = true, bool hasGeneral = true,
            bool hasEvents = true,
            bool hasInfo = true)
        {
            if (hasMain)
            {
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(soundFinder)));
                var prop = _so.FindProperty(nameof(sources));
                if (!soundFinder)
                {
                    BeginVerticalBox();
                    RCProEditor.PropertyArrayField(prop,
                        CAudio.ToContent(),
                        i => $"Audio {i + 1}".ToContent($"Index {i}"));
                    EndVertical();
                }

                PropertyMaxField(_so.FindProperty(nameof(listeningPower)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(affectSoundLoudness)));
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(loudnessThreshold)));
                PropertyMaxField(_so.FindProperty(nameof(wallThicknessInfluence)), 0.01f);
                EditorGUILayout.PropertyField(_so.FindProperty(nameof(sampleWindow)));
            }

            if (hasGeneral)
            {
                GeneralField(_so);
                BaseField(_so);
            }

            if (hasEvents)
            {
                EventField(_so);
                if (EventFoldout) RCProEditor.EventField(_so, events);
            }
            if (hasInfo) InformationField(PanelGate);
        }
        protected override void DrawDetectorGuide(Vector3 point) { }
#endif
        public List<AudioSource> DetectedSources { get; set; } = new List<AudioSource>();
        private AudioSource[] PreviousSources = Array.Empty<AudioSource>();
    }
}