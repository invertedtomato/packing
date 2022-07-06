using InvertedTomato.Compression.Integers;

var vlq = new VlqCodec();
var td = new ThompsonAlphaCodec();

using var stream = new MemoryStream();

// Encode
using (var writer = new StreamBitWriter(stream))
{
    td.EncodeInt32(1, writer);
    td.EncodeInt32(2, writer);
    vlq.EncodeInt32(3, writer);
    writer.Align();
    td.EncodeInt32(4, writer);
}

Console.WriteLine("Compressed data is " + stream.Length + " bytes");
stream.Seek(0, SeekOrigin.Begin);

// Decode
using (var reader = new StreamBitReader(stream))
{
    td.DecodeInt32(reader);
    td.DecodeInt32(reader);
    vlq.DecodeInt32(reader);
    reader.Align();
    td.DecodeInt32(reader);
}

Console.WriteLine("Done.");
Console.ReadKey(true);