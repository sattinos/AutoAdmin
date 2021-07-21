using System;
using System.Collections.Generic;

namespace AutoAdmin.Core.Collections.Extensions {
    public static class StringList {
        public static bool HasAll(IEnumerable<string> set, IEnumerable<string> subSet, StringComparison stringComparison) {
            foreach (var column in subSet) {
                // ReSharper disable once PossibleMultipleEnumeration
                if (IndexOf(set, column, stringComparison) == -1) {
                    return false;
                }
            }
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
    }
}
