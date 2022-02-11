using System.Collections.Generic;
using System.Linq;

namespace AutoAdminLib.Collections.Extensions {
    public static class EnumerableExtensions {
        public static bool IsNullOrEmpty<T>(IEnumerable<T> collection) {
            return collection == null || !collection.Any();
        }
    }
}
