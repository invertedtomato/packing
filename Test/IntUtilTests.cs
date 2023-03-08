using FluentAssertions;

namespace InvertedTomato.Packing;

public class IntUtilTests
{
    [Fact]
    public void CanPower0() => IntegerUtil.Pow(0, 0).Should().Be(1); // See https://en.wikipedia.org/wiki/Zero_to_the_power_of_zero
    
    [Fact]
    public void CanPower1() => IntegerUtil.Pow(1, 1).Should().Be(1);
    
    [Fact]
    public void CanPower2() => IntegerUtil.Pow(2, 2).Should().Be(4);
    
    [Fact]
    public void CanPower10() => IntegerUtil.Pow(10, 10).Should().Be(10000000000);
}