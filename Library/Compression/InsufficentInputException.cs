using System;

namespace InvertedTomato.Compression {
    public class InsufficentInputException : Exception {
        public InsufficentInputException() {
        }

        public InsufficentInputException(string message) : base(message) {
        }

        public InsufficentInputException(string message, Exception innerException) : base(message, innerException) {
        }
    }
}