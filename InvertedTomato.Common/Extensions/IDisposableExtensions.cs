using System;

namespace InvertedTomato {
    public static class IDisposableExtensions {
        /// <summary>
        /// If the IDisposable isn't null, dispose it. Avoids issue where dispose is called before constructor is finished.
        /// </summary>
        public static void DisposeIfNotNull(this IDisposable target) {
            if (null == target) {
                return;
            }

            target.Dispose();
        }
    }
}
