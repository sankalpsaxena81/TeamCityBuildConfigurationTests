using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core
{
    public static class EnumerableExtensions
    {
        private const string NullString = "null";

        public static string AsStringSeperatedBy<T>(this IEnumerable<T> collection, string delimiter)
        {
            var result = new StringBuilder();
            var first = true;
            foreach (var argument in collection)
            {
                if (!first)
                    result.Append(delimiter);

                result.Append(ConvertToString(argument));
                first = false;
            }
            return result.ToString();
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null) return;
            foreach (var item in collection)
            {
                action(item);
            }
        }
        public static bool Exists<T>(this IEnumerable<T> collection, Predicate<T> predicate)
        {
            if (collection == null) return false;
            foreach (var item in collection)
            {
                if (predicate(item)) return true;
            }
            return false;
        }


        public static bool ElementsEqualTo<T>(this IEnumerable<T> collection, IEnumerable<T> otherCollection)
        {
            if (collection == otherCollection) return true;

            if (otherCollection == null) return false;
            if (otherCollection.Count() != collection.Count()) return false;

            for (var i = 0; i < collection.Count(); i++)
            {
                if (!collection.ElementAt(i).Equals(otherCollection.ElementAt(i))) return false;
            }
            return true;
        }

        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> collection)
        {
            return collection != null && collection.Count() > 0;
        }

        public static bool IsEmpty(this IEnumerable collection)
        {
            return !collection.Cast<object>().Any();
        }

        private static string ConvertToString<T>(T argument)
        {
            if (argument == null)
            {
                return NullString;
            }

            return argument.ToString();
        }
    }
}