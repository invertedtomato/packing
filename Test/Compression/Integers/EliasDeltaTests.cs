using System;
using System.IO;
using Xunit;

namespace InvertedTomato.Compression.Integers {
	public class EliasDeltaTests {
		private readonly Codec EliasDelta = new EliasDeltaCodec();
		
		[Fact]
		public void EncodeDecode1000Series () {
			for (UInt64 input = 0; input< 1000; input++) {
				using (var stream = new MemoryStream()) {
					// Encode
					EliasDelta.EncodeSingle(new StreamWrapper( stream), input);
					stream.Seek(0, SeekOrigin.Begin);
					
					// Decode
					var output = EliasDelta.DecodeSingle(new StreamWrapper(stream));
					
					Assert.Equal(input,output);
				}
			}
		}


		[Fact]
		public void EncodeDecode1000Parallel1() {
			using (var stream = new MemoryStream()) {
				for (UInt64 input = 0; input < 1000; input++) {
					// Encode
					EliasDelta.EncodeSingle(new StreamWrapper(stream), input);
				}
				
				stream.Seek(0, SeekOrigin.Begin);

				for (UInt64 input = 0; input < 1000; input++) {

					// Decode
					var output = EliasDelta.DecodeSingle(new StreamWrapper(stream));

					Assert.Equal(input, output);
				}
			}
	}
		
		
		[Fact]
		public void EncodeDecode1000Parallel2() {
			var input = new UInt64[1000];
			for (UInt64 i = 0; i < 1000; i++) {
				input[i] = i;
			}
			
			
			using (var stream = new MemoryStream()) {
				// Encode
				EliasDelta.EncodeMany(new StreamWrapper(stream), input);
				
				stream.Seek(0, SeekOrigin.Begin);

				// Decode
				var output = new UInt64[1000];
				EliasDelta.DecodeMany(new StreamWrapper(stream), output);

				Assert.Equal(input, output);
			}
		}
	}
}