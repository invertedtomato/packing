using InvertedTomato.Testable.Streams;
using System;

namespace InvertedTomato {
    public static class IStreamExtensions {
        /// <summary>
        /// If the IDisposable isn't null, dispose it. Avoids issue where dispose is called before constructor is finished.
        /// </summary>
        public static void DisposeIfNotNull(this IStream target) {
            if (null == target) {
                return;
            }

            target.Dispose();
        }
    }
}
