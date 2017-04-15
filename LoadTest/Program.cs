using InvertedTomato.Compression.Integers;
using InvertedTomato.IO.Buffers;
using System;
using System.Diagnostics;
using System.IO;

namespace InvertedTomato.Compression.Integer.LoadTest {
    class Program {
        static void Main(string[] args) {

            //#1
            // Write: 7865ms 9.7MB/s
            // Read: 17392ms 4.39MB / s
            // ASync Read: 2935ms 25.99MB / s

            // #2
            // Write: 14,115ms 5.41MB/s
            // Read: 6,226ms 12.25MB/s


            ulong min = 100000;
            ulong count = 10000000;

            // Write
            var stopWatch = Stopwatch.StartNew();

            var codec = new FibonacciCodec();

            var decompressed = new Buffer<ulong>((int)(count));
            var compressed = new Buffer<byte>(10000000 * 2);
            for (ulong v = min; v < min + count; v++) {
                decompressed.Enqueue(v);
            }
            codec.CompressMany(decompressed,compressed);

            stopWatch.Stop();
            Console.WriteLine("Compress: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((double)count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s");

            // Read
            stopWatch = Stopwatch.StartNew();

            var decompressed2 = new Buffer<ulong>((int)(count));
            codec.DecompressMany(compressed, decompressed2);
            for (ulong output = min; output < min + count; output++) {
                if (decompressed2.Dequeue() != output) {
                    throw new Exception("Incorrect result. Expected " + output + " got " + output + ".");
                }
            }
            stopWatch.Stop();
            Console.WriteLine("Decompress: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((double)count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s");

            Console.WriteLine("Done.");
            Console.ReadKey(true);
        }
    }
}
