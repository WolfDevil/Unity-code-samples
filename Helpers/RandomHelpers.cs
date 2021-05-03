using System;

public static class RandomHelpers
{
    private static Random rng = new Random();

    public static bool Boolean()
    {
        return Convert.ToBoolean(rng.Next(0, 2));
    }

    public static int Range(int max) => Range(0, max);
    public static int Range(int min, int max)
    {
        return rng.Next(min, max);
    }

    public static float Range(float max) => Range(0f, max);
    public static float Range(float min, float max)
    {
        var value = rng.NextDouble() * (max - min) + min;
        return (float)value;
    }
}
