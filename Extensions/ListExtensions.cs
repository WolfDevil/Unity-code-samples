using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{
    public static bool IsEmpty<T>(this IEnumerable<T> self)
    {
        return !self.Any();
    }

    public static bool IsEmpty<T>(this T[] self)
    {
        return self.Length == 0;
    }

    public static T Last<T>(this IList<T> list)
    {
        return list[list.Count - 1];
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = RandomHelpers.Range(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T GetRandomValue<T>(this IList<T> list)
    {
        return list[RandomHelpers.Range(list.Count)];
    }

    public static TValue GetRandomValue<TKey, TValue>(this IDictionary<TKey, TValue> self)
    {
        return self.ElementAt(RandomHelpers.Range(self.Count)).Value;
    }

}
