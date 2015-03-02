using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace Helpers
{
    public static class LinqHelpers
    {
        public static IQueryable<T> ForEach<T>(this IQueryable<T> query, Action<T> action)
        {
            foreach (var obj in query)
                action(obj);

            return query;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> query, Action<T> action)
        {
            foreach (var obj in query)
                action(obj);

            return query;
        }

        public static IList<T> ForEach<T>(this IList<T> query, Action<T> action)
        {
            foreach (var obj in query)
                action(obj);

            return query;
        }

        public static bool RemoveFirst<T>(this ICollection<T> collection, Func<T, bool> condition)
        {
            foreach (var obj in collection)
                if (condition(obj))
                {
                    collection.Remove(obj);
                    return true;
                }

            return false;
        }

        public static string AggregateString<T>(this IEnumerable<T> query, Func<T, string> getString)
        {
            string result = String.Empty;

            foreach (var obj in query)
                if (result == String.Empty)
                    result = getString(obj); 
                else
                    result += ", " + getString(obj);

            return result;
        }

        public static string AggregateString<T>(this ObservableCollection<T> query, Func<T, string> getString)
        {
            return AggregateString(query.AsEnumerable(), getString);
        }
    }
}

