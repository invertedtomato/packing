using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Diagnostics;
using System.IO;

namespace InvertedTomato.Compression.Integer.LoadTest {
    class Program {
        static void Main(string[] args) {

            // #2
            // Write: 14,115ms 5.41MB/s
            // Read: 6,226ms 12.25MB/s


            ulong min = 100000;
            ulong count = 10000000;

            // Write
            var stopWatch = Stopwatch.StartNew();

            var compressor = new FibonacciCodec();
            compressor.DecompressedSet = new Buffer<ulong>((int)(count));
            for (ulong input = min; input < min + count; input++) {
                compressor.DecompressedSet.Enqueue(input);
            }
            compressor.Compress();

            stopWatch.Stop();
            Console.WriteLine("Write: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((double)count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s");

            // Read
            stopWatch = Stopwatch.StartNew();
            compressor.DecompressedSet = null;
            compressor.Decompress();
            for (ulong output = min; output < min + count; output++) {
                if (compressor.DecompressedSet.Dequeue() != output) {
                    throw new Exception("Incorrect result. Expected " + output + " got " + output + ".");
                }
            }
            stopWatch.Stop();
            Console.WriteLine("Read: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((double)count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s");

            Console.WriteLine("Done.");
            Console.ReadKey(true);
        }
    }
}
