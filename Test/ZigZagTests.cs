namespace InvertedTomato.Packing;

public class ZigZagTests
{
    [Fact]
    public void CanEncodeDecodeMax()
    {
        var encoded = ZigZagUtility.Encode(Int64.MaxValue);
        Assert.Equal(Int64.MaxValue, ZigZagUtility.Decode(encoded));
    }

    [Fact]
    public void CanEncodeDecodeMin()
    {
        var encoded = ZigZagUtility.Encode(Int64.MinValue + 1);
        Assert.Equal(Int64.MinValue + 1, ZigZagUtility.Decode(encoded));
    }
}