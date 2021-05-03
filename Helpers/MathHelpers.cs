using UnityEngine;

public class MathHelpers
{
    public static float ClampedMap(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return Mathf.Clamp(Map(value, oldMin, oldMax, newMin, newMax), newMin, newMax);
    }

    public static float Map(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (value - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
    }
}
