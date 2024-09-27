using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
    {
        return list.OrderBy(item => Guid.NewGuid());
    }
}
