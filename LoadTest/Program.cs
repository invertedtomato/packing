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

            // #3
            // Compress: 11801ms 6.47MB / s
            // Decompress: 6967ms 10.95MB / s

            // #4
            // Compress: 10291ms 7.41MB/s
            // Decompress: 5819ms 13.11MB / s

            ulong min = 100000;
            ulong count = 10000000;

            //////////////////////////////////////////
            Console.WriteLine("FIBONACCI");
            var stopWatch = Stopwatch.StartNew();
            IIntegerCodec codec = new FibonacciCodec();

            // Seed
            var input = new Buffer<ulong>((int)count);
            for (ulong v = min; v < min + count; v++) {
                input.Enqueue(v);
            }

            // Compress
            var compressed = new Buffer<byte>((int)count * 5);
            while (!codec.Compress(input, compressed)) {
                Console.Write("Expanding compression buffer... ");
                compressed = compressed.Resize(compressed.MaxCapacity * 2);
                Console.WriteLine(compressed.MaxCapacity);
            }

            stopWatch.Stop();
            Console.WriteLine("Compress: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((double)count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s Total " + (compressed.Used / 1024 / 1024) + "MB");
            stopWatch = Stopwatch.StartNew();

            // Decompress
            var output = new Buffer<ulong>((int)count);
            while (!codec.Decompress(compressed, output)) {
                Console.Write("Expanding decompression buffer...");
                output = output.Resize(output.MaxCapacity * 2);
                Console.WriteLine(output.MaxCapacity);
            }

            // Validate
            for (ulong v = min; v < min + count; v++) {
                if (output.Dequeue() != v) {
                    throw new Exception("Incorrect result. Expected " + v + " got " + v + ".");
                }
            }

            stopWatch.Stop();
            Console.WriteLine("Decompress: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((double)count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s");


            //////////////////////////////////////////
            /*
            Console.WriteLine("VLQ");
            stopWatch = Stopwatch.StartNew();
            codec = new VLQCodec();

            // Seed
            input = new Buffer<ulong>((int)count);
            for (ulong v = min; v < min + count; v++) {
                input.Enqueue(v);
            }

            // Compress
            compressed = new Buffer<byte>((int)count * 5);
            while (!codec.Compress(input, compressed)) {
                Console.Write("Expanding compression buffer... ");
                compressed = compressed.Resize(compressed.MaxCapacity * 2);
                Console.WriteLine(compressed.MaxCapacity);
            }

            stopWatch.Stop();
            Console.WriteLine("Compress: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((double)count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s Total " + (compressed.Used / 1024 / 1024) + "MB");
            stopWatch = Stopwatch.StartNew();

            // Decompress
            output = new Buffer<ulong>((int)count);
            while (!codec.Decompress(compressed, output)) {
                Console.Write("Expanding decompression buffer...");
                output = output.Resize(output.MaxCapacity * 2);
                Console.WriteLine(output.MaxCapacity);
            }

            // Validate
            for (ulong v = min; v < min + count; v++) {
                if (output.Dequeue() != v) {
                    throw new Exception("Incorrect result. Expected " + v + " got " + v + ".");
                }
            }

            stopWatch.Stop();
            Console.WriteLine("Decompress: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((double)count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s");

    */



            Console.WriteLine("Done.");
            Console.ReadKey(true);
        }
    }
}
