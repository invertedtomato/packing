namespace InvertedTomato.Feather {
    public sealed class FeatherFileOptions {
        /// <summary>
        /// Flush buffer to disk every X appends. Disabled if less than 1.
        /// </summary>
        public int AppendFlushRate = 10;
    }
}
