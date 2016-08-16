using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InvertedTomato.VariableLengthIntegers;

namespace InvertedTomato.Common.Tests.VariableLengthIntegers {
    [TestClass]
    public class UVarInt2Tests {
        UnsignedOmega Focus;

        [TestInitialize]
        public void Initialize() {
            Focus = new UnsignedOmega();
        }

        /*
              0  0_______
              1  100_____  
              2  110_____  
              3  101000__  
              6  101110__  
              7  1110000_  
             14  1111110_  
             15  10100100 000_____ 
             31  10101100 0000____ 
             99  10110110 01000___ 
            999  11100111 11101000 0_______ 
          9,999  11110010 01110001 00000___ 
         99,999  10100100 00110000 11010100 0000____ 
        999,999  10100100 11111101 00001001 0000000_ 	
        */

        [TestMethod]
        public void Encode_0() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(0)));
        }
        [TestMethod]
        public void Encode_1() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10000000", 2),
            }), BitConverter.ToString(Focus.Encode(1)));
        }
        [TestMethod]
        public void Encode_2() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11000000", 2),
            }), BitConverter.ToString(Focus.Encode(2)));
        }
        [TestMethod]
        public void Encode_3() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10100000", 2),
            }), BitConverter.ToString(Focus.Encode(3)));
        }
        [TestMethod]
        public void Encode_6() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10111000", 2),
            }), BitConverter.ToString(Focus.Encode(6)));
        }
        [TestMethod]
        public void Encode_7() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11100000", 2),
            }), BitConverter.ToString(Focus.Encode(7)));
        }
        [TestMethod]
        public void Encode_14() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11111100", 2),
            }), BitConverter.ToString(Focus.Encode(14)));
        }
        [TestMethod]
        public void Encode_15() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10100100", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(15)));
        }
        [TestMethod]
        public void Encode_31() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10101100", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(31)));
        }
        [TestMethod]
        public void Encode_99() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10110110", 2),
                Convert.ToByte("01000000", 2),
            }), BitConverter.ToString(Focus.Encode(99)));
        }
        [TestMethod]
        public void Encode_999() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11100111", 2),
                Convert.ToByte("11101000", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(999)));
        }
        [TestMethod]
        public void Encode_9999() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11110110", 2), // Wiki says 11110010, but it seems it should actually be 11110110
                Convert.ToByte("01110001", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(9999)));
        }
        [TestMethod]
        public void Encode_99999() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10100100", 2),
                Convert.ToByte("00110000", 2),
                Convert.ToByte("11010100", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(99999)));
        }
        [TestMethod]
        public void Encode_999999() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10100100", 2),
                Convert.ToByte("11111101", 2),
                Convert.ToByte("00001001", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(Focus.Encode(999999)));
        }

        [TestMethod]
        public void Encode_Max() {
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("10101111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11100000", 2),
            }), BitConverter.ToString(Focus.Encode(ulong.MaxValue - 1))); // We support one less than the full range, because we chose to include 0
        }

        [TestMethod]
        public void Encode_2_2_2() {
            var position = 0;
            var offset = 0;
            var buffer = new byte[2];

            Focus.Encode(2, buffer, ref position, ref offset);
            Assert.AreEqual(0, position);
            Assert.AreEqual(3, offset);

            Focus.Encode(2, buffer, ref position, ref offset);
            Assert.AreEqual(0, position);
            Assert.AreEqual(6, offset);

            Focus.Encode(2, buffer, ref position, ref offset);
            Assert.AreEqual(1, position);
            Assert.AreEqual(1, offset);

            // 01101101 _______1
            Assert.AreEqual(BitConverter.ToString(new byte[] {
                Convert.ToByte("11011011", 2),
                Convert.ToByte("00000000", 2),
            }), BitConverter.ToString(buffer));
        }

        [TestMethod]
        public void EncodeDecode_Range() {
            var buffer = new byte[uint.MaxValue / 1000];
            int position = 0;
            int offset = 0;

            for (var input = uint.MinValue; input < uint.MaxValue / 10000; input++) {
                var previousPosition = position;
                var previousOffset = offset;
                Focus.Encode(input, buffer, ref position, ref offset);
                position = previousPosition;
                offset = previousOffset;
                var output = Focus.Decode(buffer, ref position, ref offset);

                Assert.AreEqual(input, output);
            }
        }

        [TestMethod]
        public void Decode_14_Midstream() {
            var position = 10;
            var offset = 9;
            Assert.AreEqual((ulong)14, Focus.Decode(new byte[] { 77, 69, 86, 93, 195, 151, 78, 222, 61, 124, 252, 0, 0 }, ref position, ref offset));
        }

        [TestMethod]
        public void Decode_0() {
            Assert.AreEqual((ulong)0, Focus.Decode(new byte[] {
                Convert.ToByte("00000000", 2)
            }));
        }
        [TestMethod]
        public void Decode_1() {
            Assert.AreEqual((ulong)1, Focus.Decode(new byte[] {
                Convert.ToByte("10000000", 2)
            }));
        }
        [TestMethod]
        public void Decode_2() {
            Assert.AreEqual((ulong)2, Focus.Decode(new byte[] {
                Convert.ToByte("11000000", 2)
            }));
        }
        [TestMethod]
        public void Decode_3() {
            Assert.AreEqual((ulong)3, Focus.Decode(new byte[] {
                Convert.ToByte("10100000", 2)
            }));
        }
        [TestMethod]
        public void Decode_6() {
            Assert.AreEqual((ulong)6, Focus.Decode(new byte[] {
                Convert.ToByte("10111000", 2)
            }));
        }
        [TestMethod]
        public void Decode_7() {
            Assert.AreEqual((ulong)7, Focus.Decode(new byte[] {
                Convert.ToByte("11100000", 2)
            }));
        }
        [TestMethod]
        public void Decode_14() {
            Assert.AreEqual((ulong)14, Focus.Decode(new byte[] {
                Convert.ToByte("11111100", 2)
            }));
        }
        [TestMethod]
        public void Decode_15() {
            Assert.AreEqual((ulong)15, Focus.Decode(new byte[] {
                Convert.ToByte("10100100", 2),
                Convert.ToByte("00000000", 2),
            }));
        }
        [TestMethod]
        public void Decode_31() {
            Assert.AreEqual((ulong)31, Focus.Decode(new byte[] {
                Convert.ToByte("10101100", 2),
                Convert.ToByte("00000000", 2),
            }));
        }
        [TestMethod]
        public void Decode_99() {
            Assert.AreEqual((ulong)99, Focus.Decode(new byte[] {
                Convert.ToByte("10110110", 2),
                Convert.ToByte("01000000", 2),
            }));
        }
        [TestMethod]
        public void Decode_999() {
            Assert.AreEqual((ulong)999, Focus.Decode(new byte[] {
                Convert.ToByte("11100111", 2),
                Convert.ToByte("11101000", 2),
                Convert.ToByte("00000000", 2),
            }));
        }
        [TestMethod]
        public void Decode_9999() {
            Assert.AreEqual((ulong)9999, Focus.Decode(new byte[] { // Wiki says 11110010, but it seems it should actually be 11110110
                Convert.ToByte("11110110", 2),
                Convert.ToByte("01110001", 2),
                Convert.ToByte("00000000", 2),
            }));
        }
        [TestMethod]
        public void Decode_99999() {
            Assert.AreEqual((ulong)99999, Focus.Decode(new byte[] {
                Convert.ToByte("10100100", 2),
                Convert.ToByte("00110000", 2),
                Convert.ToByte("11010100", 2),
                Convert.ToByte("00000000", 2),
            }));
        }
        [TestMethod]
        public void Decode_999999() {
            Assert.AreEqual((ulong)999999, Focus.Decode(new byte[] {
                Convert.ToByte("10100100", 2),
                Convert.ToByte("11111101", 2),
                Convert.ToByte("00001001", 2),
                Convert.ToByte("00000000", 2),
            }));
        }

        [TestMethod]
        public void Decode_Max() {
            Assert.AreEqual(ulong.MaxValue - 1, Focus.Decode(new byte[] { // We support one less than the full range, because we chose to include 0
                Convert.ToByte("10101111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11111111", 2),
                Convert.ToByte("11100000", 2),
            }));
        }

        [TestMethod]
        public void Decode_2_2_2() {
            var position = 0;
            var offset = 0;
            var buffer = new byte[] {
                Convert.ToByte("11011011", 2),
                Convert.ToByte("00000000", 2),
            };

            Assert.AreEqual((ulong)2, Focus.Decode(buffer, ref position, ref offset));
            Assert.AreEqual(0, position);
            Assert.AreEqual(3, offset);

            Assert.AreEqual((ulong)2, Focus.Decode(buffer, ref position, ref offset));
            Assert.AreEqual(0, position);
            Assert.AreEqual(6, offset);

            Assert.AreEqual((ulong)2, Focus.Decode(buffer, ref position, ref offset));
            Assert.AreEqual(1, position);
            Assert.AreEqual(1, offset);

        }
    }
}
