using System;

namespace InvertedTomato.Feather {
    public static class FeatherFile {
        /// <summary>
        /// Open Feather data file for reading.
        /// </summary>
        public static FileReader OpenRead(string filePath) { return OpenRead(filePath, new FileOptions()); }

        /// <summary>
        /// Open Feather data file for reading.
        /// </summary>
        public static FileReader OpenRead(string filePath, FileOptions options) {
            if (null == filePath) {
                throw new ArgumentNullException("fileName");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }

            return new FileReader(filePath, options);
        }

        /// <summary>
        /// Open Feather data file for reading.
        /// </summary>
        public static FileWriter OpenWrite(string filePath) { return OpenWrite(filePath, new FileOptions()); }

        /// <summary>
        /// Open Feather data file for reading.
        /// </summary>
        public static FileWriter OpenWrite(string filePath, FileOptions options) {
            if (null == filePath) {
                throw new ArgumentNullException("fileName");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }

            return new FileWriter(filePath, options);
        }
    }
}
