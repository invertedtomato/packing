namespace InvertedTomato.VariableLengthIntegers {
    public interface IIntegerReader<T> {
        bool TryRead(out T value);
        T Read();

        // static IEnumerable<ulong> Read(/* options */, byte[] input); 
    }
}
