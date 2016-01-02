using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Blacker.MangaReader.Collections
{
    static class Extensions
    {
        public static IEnumerable<T> OrderByAlphaNumeric<T>(this IEnumerable<T> source, Func<T, string> selector)
        {
            return OrderByAlphaNumeric(source, selector, (x) => x);
        }

        public static IEnumerable<T> OrderByAlphaNumeric<T>(this IEnumerable<T> source, Func<T, string> selector, Func<string, string> filter)
        {
            var enumerable = source as T[] ?? source.ToArray();

            int max = enumerable
                          .SelectMany(i => Regex.Matches(filter(selector(i)) ?? String.Empty, @"\d+").Cast<Match>().Select(m => (int?) m.Value.Length))
                          .Max() ?? 0;

            return enumerable.OrderBy(i => Regex.Replace(filter(selector(i)) ?? String.Empty, @"\d+", m => m.Value.PadLeft(max, '0')));
        }

        public static IEnumerable<string> OrderByAlphaNumeric(this IEnumerable<string> source)
        {
            return OrderByAlphaNumeric(source, t => t, x => x);
        }    

        public static IEnumerable<string> OrderByAlphaNumeric(this IEnumerable<string> source, Func<string, string> filter)
        {
            return OrderByAlphaNumeric(source, t => t, filter);
        }

        public static int FindIndex<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var result = source.Select((v, i) => new {Item = v, Index = i}).FirstOrDefault(x => predicate(x.Item));
            return result == null ? -1 : result.Index;
        }
    }
}
