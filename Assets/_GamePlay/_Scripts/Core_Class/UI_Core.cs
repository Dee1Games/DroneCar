using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class UI_Core : MonoBehaviour
{
    public static UI_Core _;

    public HealthUI carHealth;
    public Image giantIcon;
    public Image shieldIcon;
    public Track track;

    public MMCameraShaker shaker;
    void Start()
    {
        _ = this;
    }
    
    public void Shake(float duration = 1f, float amp = 1f)
    {
        shaker.ShakeCamera(1f, amp, 10, 2f, 2f, 2f, true);
    }
}
