using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Helpers
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ConcatWith<T>(this IEnumerable<T> enumerable, params T[] entitiesToAdd)
        {
            return enumerable.Concat(entitiesToAdd);
        }

        public static bool In<T>(this T obj, IEnumerable<T> collection)
        {
            return collection.Contains(obj);
        }

        public static bool In<T>(this T obj, params T[] items)
        {
            return In(obj, collection: items);
        }

        public static IEnumerable<T> If<T>(this IEnumerable<T> enumerable, bool condition,
            Func<IEnumerable<T>, IEnumerable<T>> transform)
        {
            return condition ? transform.Invoke(enumerable) : enumerable;
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Where(x => x != null);
        }
    }
}