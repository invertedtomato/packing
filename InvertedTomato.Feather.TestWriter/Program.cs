using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato.Feather.TestWriter {
    class Program {
        static void Main(string[] args) {


            using (var file=File.Open("test.dat", FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)) {


            }

        }
    }
}
