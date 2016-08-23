using System;
using System.IO;

namespace InvertedTomato.Compression.Integers.Sample {
    class Program {
        static void Main(string[] args) {
            using (var stream = new MemoryStream()) {
                // Make a writer to encode values onto your stream
                using (var writer = new FibonacciUnsignedWriter(stream)) { // Fibonacci is just one algorithm
                    // Write 1st value
                    writer.Write(1); // 8 bytes in memory

                    // Write 2nd value
                    writer.Write(2); // 8 bytes in memory

                    // Write 3rd value
                    writer.Write(3); // 8 bytes in memory
                }

                Console.WriteLine("Compressed data is " + stream.Length + " bytes");
                // Output: Compressed data is 2 bytes

                stream.Position = 0;

                // Make a reader to decode values from your stream
                using (var reader = new FibonacciUnsignedReader(stream)) {
                    Console.WriteLine(reader.Read());
                    // Output: 1
                    Console.WriteLine(reader.Read());
                    // Output: 2
                    Console.WriteLine(reader.Read());
                    // Output: 3
                }
            }

            Console.ReadKey(true);
        }
    }
}
