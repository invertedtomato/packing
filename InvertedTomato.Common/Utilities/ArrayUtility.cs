using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato {
    public static class ArrayUtility {
        /// <summary>
        /// Returns True if contents are identical, False otherwise.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool Compare(byte[] left, byte[] right) {
            if (left == right) {
                return true;
            }
            if (null == left || null == right) {
                return false;
            }
            if (left.Length != right.Length) {
                return false;
            }
            for (int i = 0; i < left.Length; i++) {
                if (left[i] != right[i]) {
                    return false;
                }
            }

            return true;
        }
    }
}
