using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace InvertedTomato {
    public static class HashSetExtensions {
        /// <summary>
        /// Add many items to the HashSet.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <param name="items"></param>
        public static void AddMany<T>(this HashSet<T> target, HashSet<T> items) {
            Contract.Requires(target != null, "v");
            Contract.Requires(items != null, "items");

            foreach (var item in items) {
                target.Add(item);
            }
        }
    }
}
