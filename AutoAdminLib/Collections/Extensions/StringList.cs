using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoAdminLib.Collections.Extensions {
    public static class StringList {
        public static bool HasAll(IEnumerable<string> set, IEnumerable<string> subSet, StringComparison stringComparison, out string notFoundElement) {
            foreach (var column in subSet) {
                // ReSharper disable once PossibleMultipleEnumeration
                if (IndexOf(set, column, stringComparison) == -1)
                {
                    notFoundElement = column;
                    return false;
                }
            }

            notFoundElement = null;
            return true;
        }

        public static int IndexOf(IEnumerable<string> set, string element, StringComparison stringComparison) {
            int index = 0;
            foreach (var c in set) {
                if (string.Equals(c, element, stringComparison)) {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public static string ToString<T>(this IEnumerable<T> list)
        {
            var buffer = new StringBuilder();
            int index = 1;
            var items = list as T[] ?? list.ToArray();
            foreach (var item in items)
            {
                buffer.Append(item);
                if (index != items.Length)
                {
                    buffer.Append(", ");
                }
                index++;
            }
            return buffer.ToString();
        }
    }
}
