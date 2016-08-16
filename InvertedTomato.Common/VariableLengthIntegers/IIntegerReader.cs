namespace InvertedTomato.VariableLengthIntegers {
    public interface IIntegerReader {
        bool TryRead(out ulong value);
        ulong Read();

        // static IEnumerable<ulong> Read(/* options */, byte[] input); 
    }
}
