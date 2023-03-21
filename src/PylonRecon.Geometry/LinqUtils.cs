namespace PylonRecon.Geometry;

internal static class LinqUtils
{
#if NETCOREAPP3_1
    public static TSource? MinBy<TSource, TKey>(this IEnumerable<TSource> collection, Func<TSource, TKey> mapper) where TKey : IComparable<TKey>
    {
        var minElement = collection.FirstOrDefault();
        if (minElement is null) return default;
        foreach (var element in collection)
        {
            if (mapper(element).CompareTo(mapper(minElement)) < 0)
            {
                minElement = element;
            }
        }
        return minElement;
    }

    public static TSource? MaxBy<TSource, TKey>(this IEnumerable<TSource> collection, Func<TSource, TKey> mapper) where TKey : IComparable<TKey>
    {
        var minElement = collection.FirstOrDefault();
        if (minElement is null) return default;
        foreach (var element in collection)
        {
            if (mapper(element).CompareTo(mapper(minElement)) > 0)
            {
                minElement = element;
            }
        }
        return minElement;
    }

#endif
}