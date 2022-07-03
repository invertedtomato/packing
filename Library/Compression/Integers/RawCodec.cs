using System;
using System.IO;

namespace InvertedTomato.Compression.Integers
{
    public class RawCodec : ICodec
    {
        public UInt64 MinValue => UInt64.MinValue;
        public UInt64 MaxValue => UInt64.MaxValue;

        public Int32? CalculateEncodedBits(UInt64 value)
        {
            return 8 * 8;
        }

        private void Encode(UInt64 value, IBitWriter buffer)
        {
            // Convert to raw byte array
            var raw = BitConverter.GetBytes(value);

            // Standardise endian
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(raw);
            }

            // Add to output
            foreach (var b in raw)
            {
                buffer.WriteBits((UInt64) b, 8);
            }
        }

        private UInt64 Decode(IBitReader buffer)
        {
            // Get next 8 bytes
            var b = new Byte[8];
            try
            {
                for (var j = 0; j < b.Length; j++)
                {
                    b[j] = (Byte) buffer.ReadBits(8);
                }
            }
            catch (ArgumentException)
            {
                throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
            }

            // Standardise endian
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(b);
            }

            // Convert to symbol
            var symbol = BitConverter.ToUInt64(b, 0);

            // Return symbol
            return symbol;
        }

        public void EncodeBit(bool value, IBitWriter buffer) => Encode(1, buffer);
        public void EncodeUInt8(byte value, IBitWriter buffer) => Encode(value, buffer);
        public void EncodeUInt16(ushort value, IBitWriter buffer) => Encode(value, buffer);
        public void EncodeUInt32(uint value, IBitWriter buffer) => Encode(value, buffer);
        public void EncodeUInt64(ulong value, IBitWriter buffer) => Encode(value, buffer);
        public void EncodeInt8(sbyte value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);
        public void EncodeInt16(short value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);
        public void EncodeInt32(int value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);
        public void EncodeInt64(long value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);

        public Boolean DecodeBit(IBitReader buffer) => Decode(buffer) > 0;
        public Byte DecodeUInt8(IBitReader buffer) => (Byte) Decode(buffer);
        public UInt16 DecodeUInt16(IBitReader buffer) => (UInt16) Decode(buffer);
        public UInt32 DecodeUInt32(IBitReader buffer) => (UInt32) Decode(buffer);
        public UInt64 DecodeUInt64(IBitReader buffer) => Decode(buffer);
        public SByte DecodeInt8(IBitReader buffer) => (SByte) ZigZag.Decode(Decode(buffer));
        public Int16 DecodeInt16(IBitReader buffer) => (Int16) ZigZag.Decode(Decode(buffer));
        public Int32 DecodeInt32(IBitReader buffer) => (Int32) ZigZag.Decode(Decode(buffer));
        public Int64 DecodeInt64(IBitReader buffer) => ZigZag.Decode(Decode(buffer));
    }
}