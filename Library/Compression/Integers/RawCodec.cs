using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers {
    public class RawCodec : IIntegerCodec {
        public byte[] Compress(long[] symbols) {
#if DEBUG
            if (null == symbols) {
                throw new ArgumentNullException("symbols");
            }
#endif

            // Prepare input buffer
            var input = new Buffer<ulong>(symbols.Length);

            // Perform signed conversion
            foreach (var i in symbols) {
                input.Enqueue(ZigZag.Encode(i));
            }

            // Compress
            var output = new Buffer<byte>(input.Used * 2);
            while (!Compress(input, output)) {
                output = output.Resize(output.MaxCapacity * 2);
            }

            return output.ToArray();
        }

        public bool Compress(Buffer<ulong> input, Buffer<byte> output) {
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException("input");
            }
            if (null == output) {
                throw new ArgumentNullException("output");
            }
#endif

            // Loop while there is still input
            while (!input.IsEmpty) {
                // Abort if there isn't enough free space
                if (output.Available < 8) {
                    return false; // INPUT isn't empty
                }

                // Convert symbol to bytes
                var raw = BitConverter.GetBytes(input.Dequeue());

                // Write to output
                output.EnqueueArray(raw);
            }

            // Return
            return true; // INPUT is empty
        }

        public long[] Decompress(byte[] raw) {
#if DEBUG
            if (null == raw) {
                throw new ArgumentNullException("raw");
            }
#endif

            // Prepare buffers
            var input = new Buffer<byte>(raw);
            var output = new Buffer<ulong>(raw.Length);

            // Decompress
            while (!Decompress(input, output)) {
                output = output.Resize(output.MaxCapacity * 2);
            }

            // Perform signed conversion
            var symbols = new long[output.Used];
            for (var i = 0; i < symbols.Length; i++) {
                symbols[i] = ZigZag.Decode(output.Dequeue());
            }

            return symbols;
        }

        public bool Decompress(Buffer<byte> input, Buffer<ulong> output) {
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException("input");
            }
            if (null == output) {
                throw new ArgumentNullException("output");
            }
#endif

            // Iterate through input
            while (input.Used >= 8) {
                // Check there is enough output space
                if (output.IsFull) {
                    return false;
                }

                // Get symbol
                var symbol = BitConverter.ToUInt64(input.GetUnderlying(), input.Start); // Low-level cheat to save allocating unnesscessary memory
                input.MoveStart(+8);

                // Add to output
                output.Enqueue(symbol);
            }

#if DEBUG
            if (input.Used > 0 && input.Used < 8) {
                throw new FormatException("Input ends with a partial symbol - bytes are missing or the input is corrupt.");
            }
#endif

            // Return
            return true;
        }

        public static readonly ulong MaxValue = ulong.MaxValue;
    }
}
