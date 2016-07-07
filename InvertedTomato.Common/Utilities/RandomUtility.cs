using System;

namespace InvertedTomato {
    public static class RandomUtility {
        /// <summary>
        /// Easy access to the random number generator.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly Random Current = new Random();
    }
}
