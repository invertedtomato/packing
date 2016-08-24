using InvertedTomato.Compression.Integers;
using System;
using System.Diagnostics;
using System.IO;

namespace InvertedTomato.Compression.Integer.LoadTest {
    class Program {
        static void Main(string[] args) {
            using (var stream = new MemoryStream(10 * 1024 * 1024)) {
                ulong min = 100000;
                ulong count = 10000000;

                // Write
                var stopWatch = Stopwatch.StartNew();
                using (var writer = new FibonacciUnsignedWriter(stream)) {
                    for (ulong input = min; input < min + count; input++) {
                        writer.Write(input);
                    }
                }
                stopWatch.Stop();
                Console.WriteLine("Write: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((double)count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s");
                
                // Read
                stream.Position = 0;
                stopWatch = Stopwatch.StartNew();
                using (var reader = new FibonacciUnsignedReader(stream)) {
                    for (ulong expected = min; expected < min + count; expected++) {
                        var output = reader.Read();

                        if (output != expected) {
                            throw new Exception("Incorrect result. Expected " + expected + " got " + output + ".");
                        }
                    }
                }
                stopWatch.Stop();
                Console.WriteLine("Read: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((double)count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s");
                
                // Read ASync
                stream.Position = 0;
                ulong expected2 = min;
                stopWatch = Stopwatch.StartNew();
                var reader2 = new ASyncFibonacciUnsignedReader((output) => {
                    if (output != expected2) {
                        throw new Exception("Incorrect result. Expected " + expected2 + " got " + output + ".");
                    }
                    expected2++;

                    return true;
                });
                int b;
                while ((b = stream.ReadByte()) > -1) {
                    reader2.Insert((byte)b);
                }

                stopWatch.Stop();
                Console.WriteLine("ASync Read: " + stopWatch.ElapsedMilliseconds + "ms " + Math.Round((double)count * 1000 * 8 / 1024 / 1024 / stopWatch.ElapsedMilliseconds, 2) + "MB/s");
            }

            Console.ReadKey(true);
            //10,845ms
        }
    }
}
