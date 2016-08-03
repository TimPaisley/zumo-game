using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Zumo {
    static class EnumerableHelper {
        public static T Sample<T>(this IEnumerable<T> items) {
            return items.ElementAt(Random.Range(0, items.Count()));
        }
    }
}
