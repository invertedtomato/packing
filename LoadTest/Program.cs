using InvertedTomato.Compression.Integers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato.Compression.Integer.LoadTest {
    class Program {
        static void Main(string[] args) {
            var stopWatch = Stopwatch.StartNew();
            for (ulong input = 100000; input < 10000000; input++) {
                var encoded = FibonacciUnsignedWriter.WriteOneDefault(input);
                 //FibonacciUnsignedReader.ReadOneDefault(encoded);
            }
            stopWatch.Stop();
            Console.WriteLine(stopWatch.ElapsedMilliseconds + "ms");
            Console.ReadKey(true);
            //10,845ms
        }
    }
}
