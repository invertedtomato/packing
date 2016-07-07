using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

namespace InvertedTomato {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public static class DictionaryExtensions {
        /// <summary>
        /// Get from dictionary - and if it doesn't exist just return a default v.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="v"></param>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static T GetWithDefault<T>(this Dictionary<string, T> target, string key, T defaultValue) {
            Contract.Requires(target != null, "v");
            Contract.Requires(key != null, "key");

            // Try to load v
            T ret;
            if (!target.TryGetValue(key, out ret)) {
                ret = defaultValue;
            }

            return ret;
        }
    }
}
