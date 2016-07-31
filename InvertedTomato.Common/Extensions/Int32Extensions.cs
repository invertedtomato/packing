using System;

namespace InvertedTomato.Extensions {
    public static class Int32Extensions {
        /// <summary>
        /// Converts a string to int32, returns 0 in case the input string is empty.
        /// </summary>
        [Obsolete("This is an anti-pattern. Should use int.TryParse() externally with a null check. This will be removed in a future release.")]
        public static int ToInt32(this string value) {
            if (string.IsNullOrEmpty(value)) {
                return 0;
            }
            return Convert.ToInt32(value);
        }
    }
}
