using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RaycastPro.Casters;
using UnityEngine;

public abstract class GunCore : MonoBehaviour
{
    public AdvanceCaster caster;
    public Color alertColor = Color.red;
    public float alertDelay = 1f;

    protected Sequence sequence;

    protected bool IsPlaying => sequence != null && sequence.IsPlaying();

    private void Start()
    {
        TryGetComponent(out caster);
    }

    protected void Stop() => sequence.Kill();
    public abstract void Activate();
    public abstract void Deactivate();
}
