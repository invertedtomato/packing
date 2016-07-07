using System.Threading;

namespace InvertedTomato {
    public static class ThreadExtensions {
        /// <summary>
        /// Join if not null. Avoids issue while disposing when constructor isn't finished.
        /// </summary>
        public static void JoinIfNotNull(this Thread target) {
            if (null == target) {
                return;
            }

            target.Join();
        }
    }
}
