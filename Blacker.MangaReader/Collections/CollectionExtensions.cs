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
            var enumerable = source as T[] ?? source.ToArray();

            int max = enumerable
                          .SelectMany(i => Regex.Matches(selector(i) ?? String.Empty, @"\d+").Cast<Match>().Select(m => (int?) m.Value.Length))
                          .Max() ?? 0;

            return enumerable.OrderBy(i => Regex.Replace(selector(i) ?? String.Empty, @"\d+", m => m.Value.PadLeft(max, '0')));
        }

        public static IEnumerable<string> OrderByAlphaNumeric(this IEnumerable<string> source)
        {
            return OrderByAlphaNumeric(source, t => t);
        }

        public static int FindIndex<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            var result = source.Select((v, i) => new {Item = v, Index = i}).FirstOrDefault(x => predicate(x.Item));
            return result == null ? -1 : result.Index;
        }
    }
}
