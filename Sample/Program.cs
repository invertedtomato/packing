using InvertedTomato.Compression.Integers;

var vlq = new VlqCodec();
var td = new ThompsonAlphaCodec();

using var stream = new MemoryStream();

// Compress
using (var writer = new BitWriter(stream))
{
    writer.WriteInt32(1, td);
    writer.WriteInt32(2, td);
    writer.WriteInt32(3, vlq);
    writer.WriteInt32(4, td);
    writer.AlignByte();
    writer.WriteInt32(2, td);
}

Console.WriteLine("Compressed data is " + stream.Length + " bytes"); // Output: Compressed data is 2 bytes
stream.Seek(0, SeekOrigin.Begin);

// Decompress
using (var reader = new BitReader(stream))
{
    reader.ReadInt32(td);
    reader.ReadInt32(td);
    reader.ReadInt32(vlq);
    reader.ReadInt32(td);
    reader.AlignByte();
    reader.ReadInt32(td);
}

Console.WriteLine("Done.");
Console.ReadKey(true);