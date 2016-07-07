using System;

namespace InvertedTomato.Extensions {
    public static class Int32Extensions {
        //Converts a string to int32, returns 0 in case the input string is empty.
        public static int ToInt32(this string value) {
            if (string.IsNullOrEmpty(value))
                return 0;
            return Convert.ToInt32(value);
        }
    }
}
