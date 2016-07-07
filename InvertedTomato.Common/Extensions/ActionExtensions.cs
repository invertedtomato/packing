using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato {
    public static class ActionExtensions {
        /// <summary>
        /// Executes a given action, if it isn't null.
        /// </summary>
        public static bool TryInvoke(this Action evt) {
            if (null == evt) {
                return false;
            }

            evt();
            return true;
        }

        /// <summary>
        /// Executes a given action, if it isn't null.
        /// </summary>
        public static bool TryInvoke<T1>(this Action<T1> evt, T1 val1) {
            if (null == evt) {
                return false;
            }

            evt(val1);
            return true;
        }

        /// <summary>
        /// Executes a given action, if it isn't null.
        /// </summary>
        public static bool TryInvoke<T1, T2>(this Action<T1, T2> evt, T1 val1, T2 val2) {
            if (null == evt) {
                return false;
            }

            evt(val1, val2);
            return true;
        }

        /// <summary>
        /// Executes a given action, if it isn't null.
        /// </summary>
        public static bool TryInvoke<T1, T2, T3>(this Action<T1, T2, T3> evt, T1 val1, T2 val2, T3 val3) {
            if (null == evt) {
                return false;
            }

            evt(val1, val2, val3);
            return true;
        }
    }
}
