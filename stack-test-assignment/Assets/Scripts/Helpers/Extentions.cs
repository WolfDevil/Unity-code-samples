using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extentions
{
    public static float Map(this float self, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (self - oldMin) * (newMax - newMin) / (oldMax - oldMin) + newMin;
    }

    public static T RandomElement<T>(this IList<T> list)
    {
        var random = new System.Random();
        return list[random.Next(list.Count)];
    }
}
