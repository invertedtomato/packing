using InvertedTomato.IO.Buffers;

namespace InvertedTomato {
    public static class ULongBufferExtensions {
        public static void Seed(this Buffer<System.UInt64> target, System.UInt64 start, System.UInt64 end) {
            for (System.UInt64 i = start; i <= end; i++) {
                target.Enqueue(i);
            }
        }
    }
}
