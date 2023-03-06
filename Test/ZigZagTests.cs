namespace InvertedTomato.Binary;

public class ZigZagTests
{
    [Fact]
    public void EncodeDecodeMax()
    {
        var encoded = ZigZagUtility.Encode(Int64.MaxValue);
        Assert.Equal(Int64.MaxValue, ZigZagUtility.Decode(encoded));
    }

    [Fact]
    public void EncodeDecodeMin()
    {
        var encoded = ZigZagUtility.Encode(Int64.MinValue + 1);
        Assert.Equal(Int64.MinValue + 1, ZigZagUtility.Decode(encoded));
    }
}