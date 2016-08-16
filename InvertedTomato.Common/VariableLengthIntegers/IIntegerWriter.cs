namespace InvertedTomato.VariableLengthIntegers {
    public interface IIntegerWriter {
        void Write(ulong value);

        // static byte[] Write (params ulong value);
        // static byte[] Write (IEnumerable<ulong> values);
    }
}
