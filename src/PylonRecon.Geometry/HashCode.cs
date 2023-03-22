namespace PylonRecon.Geometry;

#if NETCOREAPP2_1_OR_GREATER
#elif NETCOREAPP2_1_OR_GREATER
#else
    
internal static class HashCode
{
    public static int Combine<T1, T2>(T1 v1, T2 v2)
    {
        int hashCode = ~0;
        if (v1 is not null) hashCode &= v1.GetHashCode();
        if (v2 is not null) hashCode &= v2.GetHashCode();
        return hashCode;
    }

    public static int Combine<T1, T2, T3>(T1 v1, T2 v2, T3 v3)
    {
        int hashCode = ~0;
        if (v1 is not null) hashCode &= v1.GetHashCode();
        if (v2 is not null) hashCode &= v2.GetHashCode();
        if (v3 is not null) hashCode &= v3.GetHashCode();
        return hashCode;
    }
}

#endif