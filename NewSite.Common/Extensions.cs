﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsSite.Common
{
    public static class Extensions
    {
        public static List<T> FluentAdd<T>(this List<T> list, T item)
        {
            list.Add(item);
            return list;
        }

        public static bool IsNullOrEmptyOrWhiteSpace(this string s)
        {
            return string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s);
        }
    }
}
