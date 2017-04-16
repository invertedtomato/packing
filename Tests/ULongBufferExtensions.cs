using InvertedTomato.IO.Buffers;

namespace InvertedTomato {
    public static class ULongBufferExtensions {
        public static void Seed(this Buffer<ulong> target, ulong start, ulong end) {
            for (ulong i = start; i <= end; i++) {
                target.Enqueue(i);
            }
        }
    }
}
