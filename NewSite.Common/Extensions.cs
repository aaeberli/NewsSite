using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common
{
    /// <summary>
    /// Static class for extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// List Add operation with fluent syntax behaviour
        /// </summary>
        /// <typeparam name="T">Type of the List</typeparam>
        /// <param name="list">The list</param>
        /// <param name="item">Item to add</param>
        /// <returns>List with added item</returns>
        public static List<T> FluentAdd<T>(this List<T> list, T item)
        {
            list.Add(item);
            return list;
        }

        /// <summary>
        /// Operation that merges IsNullOrEmpty and IsNullOrWhiteSpace conditions
        /// </summary>
        /// <param name="s">The string</param>
        /// <returns>Result of the check</returns>
        public static bool IsNullOrEmptyOrWhiteSpace(this string s)
        {
            return string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
        }
    }
}
