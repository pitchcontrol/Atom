using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atom
{
    static public class Helpers
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            return source.SelectMany(x => Flatten(selector(x), selector))
                .Concat(source);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
                action(item);
        }
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int count = 0;
            foreach (T item in source)
                action(item, count++);
        }
        public static IEnumerable<TR> Select<T, TE, TR>(this TE source, Func<T, int, TR> funct) where TE : IEnumerable<T>
        {
            int count = 0;
            foreach (T item in source)
            {
                yield return funct(item, count);
                count++;
            }
        }
    }
}
