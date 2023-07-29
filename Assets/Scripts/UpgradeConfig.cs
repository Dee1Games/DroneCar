using UnityEngine;

[System.Serializable]
public class UpgradeConfig
{
    public UpgradeType Type;
    
    [SerializeField] private AnimationCurve accelerationCurve = new AnimationCurve();
    [SerializeField] private Vector2 accelerationRemap;
    [SerializeField] private AnimationCurve reverseAccelerationCurve = new AnimationCurve();
    [SerializeField] private Vector2 reverseAccelerationRemap;
    [SerializeField] private AnimationCurve maxSpeedCurve = new AnimationCurve();
    [SerializeField] private Vector2 maxSpeedRemap;
    [SerializeField] private AnimationCurve handlingCurve = new AnimationCurve();
    [SerializeField] private Vector2 handlingRemap;
    [SerializeField] private AnimationCurve bombCurve = new AnimationCurve();
    [SerializeField] private Vector2 bombRemap;
    [SerializeField] private AnimationCurve gunCurve= new AnimationCurve();
    [SerializeField] private Vector2 gunRemap;
    [SerializeField] private AnimationCurve jumpForceCurve = new AnimationCurve();
    [SerializeField] private Vector2 jumpForceRemap;

    public float GetAcceleration(int level)
    {
        float value = accelerationCurve.Evaluate(level);
        return Remap(value, accelerationCurve, accelerationRemap.x, accelerationRemap.y);
    }
    
    public float GetReverseAcceleration(int level)
    {
        float value = reverseAccelerationCurve.Evaluate(level);
        return Remap(value, reverseAccelerationCurve, reverseAccelerationRemap.x, reverseAccelerationRemap.y);
    }
    
    public float GetMaxSpeed(int level)
    {
        float value = maxSpeedCurve.Evaluate(level);
        return Remap(value, maxSpeedCurve, maxSpeedRemap.x, maxSpeedRemap.y);
    }
    
    public float GetHandling(int level)
    {
        float value = handlingCurve.Evaluate(level);
        return Remap(value, handlingCurve, handlingRemap.x, handlingRemap.y);
    }
    
    public float GetBomb(int level)
    {
        float value = bombCurve.Evaluate(level);
        return Remap(value, bombCurve, bombRemap.x, bombRemap.y);
    }
    
    public float GetGun(int level)
    {
        float value = gunCurve.Evaluate(level);
        return Remap(value, gunCurve, gunRemap.x, gunRemap.y);
    }
    
    public float GetJumpForce(int level)
    {
        float value = jumpForceCurve.Evaluate(level);
        return Remap(value, jumpForceCurve, jumpForceRemap.x, jumpForceRemap.y);
    }
    
    public static float Remap(float x, AnimationCurve curve, float newMin, float newMax)
    {
        float min = 9999f;
        float max = -9999f;
        for (int i = 0; i < curve.length; i++)
        {
            if (curve[i].value < min)
                min = curve[i].value;
            if (curve[i].value > max)
                max = curve[i].value;
        }

        if (max == min || curve.length==0)
        {
            return x;
        }
        else
        {
            float remappedValue = newMin + (x - min) / (max - min) * (newMax - newMin);
            return remappedValue;
        }
    }

}
