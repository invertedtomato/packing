using System;
using System.IO;
using Xunit;

namespace InvertedTomato.Compression.Integers {
	public class ThompsonAlphaTests {
		private readonly Codec ThompsonAlpha = new ThompsonAlphaCodec();
		
		[Fact]
		public void EncodeDecode1000Series () {
			for (UInt64 input = 0; input< 1000; input++) {
				using (var stream = new MemoryStream()) {
					// Encode
					ThompsonAlpha.EncodeSingle(new StreamWrapper( stream), input);
					stream.Seek(0, SeekOrigin.Begin);
					
					// Decode
					var output = ThompsonAlpha.DecodeSingle(new StreamWrapper(stream));
					
					Assert.Equal(input,output);
				}
			}
		}


		[Fact]
		public void EncodeDecode1000Parallel1() {
			using (var stream = new MemoryStream()) {
				for (UInt64 input = 0; input < 1000; input++) {
					// Encode
					ThompsonAlpha.EncodeSingle(new StreamWrapper(stream), input);
				}
				
				stream.Seek(0, SeekOrigin.Begin);

				for (UInt64 input = 0; input < 1000; input++) {

					// Decode
					var output = ThompsonAlpha.DecodeSingle(new StreamWrapper(stream));

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
				ThompsonAlpha.EncodeMany(new StreamWrapper(stream), input);
				
				stream.Seek(0, SeekOrigin.Begin);

				// Decode
				var output = new UInt64[1000];
				ThompsonAlpha.DecodeMany(new StreamWrapper(stream), output);

				Assert.Equal(input, output);
			}
		}
	}
}