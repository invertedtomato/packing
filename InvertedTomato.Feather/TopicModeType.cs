namespace InvertedTomato.Feather {
    public enum TopicModeType : byte {
        /// <summary>
        /// The most recent message per topic will be delivered to current and future clients.
        /// </summary>
        PublishStickyLossy = 1
    }
}
