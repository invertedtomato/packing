namespace InvertedTomato.VariableLengthIntegers {
    public interface IIntegerWriter<T> {
        void Write(T value);

        // static byte[] Write (params ulong value);
        // static byte[] Write (IEnumerable<ulong> values);
    }
}
