# Integer Compression

## TLDR
Here's how to compress 24 bytes of data down to 3 using VLQ:
```C#
using (var stream = new MemoryStream()) {
    // Make a writer to encode values onto your stream
    using (var writer = new VLQUnsignedWriter(stream)) { // VLQ is just one algorithm
        // Write 1st value
        writer.Write(1); // 8 bytes in memory

        // Write 2nd value
        writer.Write(2); // 8 bytes in memory

        // Write 3rd value
        writer.Write(3); // 8 bytes in memory
    }
                
    Console.WriteLine("Compressed data is " + stream.Length + " bytes");
    // Output: Compressed data is 3 bytes

    stream.Position = 0;

    // Make a reader to decode vallues from your stream
    using (var reader = new VLQUnsignedReader(stream)) {
        Console.WriteLine(reader.Read());
        // Output: 1
        Console.WriteLine(reader.Read());
        // Output: 2
        Console.WriteLine(reader.Read());
        // Output: 3
    }
}
```

## Introduction
Modern PCs have stacks of RAM, so it's usually not a problem that integers take 4-8 bytes 
of RAM each to store in memory. However there are times when this is a problem. Some big ones are:
 - When you want to store a large set of numbers in memory (100 million * 8 bytes = 760MB)
 - When you want to store a large set of numbers on disk
 - When you want to transmit numbers over a network (the Internet?) quickly

In almost all cases those numbers can be stored in a much lower number of bytes. Heck, its
**possible to store three integers in a single byte**.

## Readers and Writers
Here's how to store those numbers small. It's really simple - wrap an underlying stream in a writer, and write your values in:

```C#
using (var writer = new VLQUnsignedWriter(stream)) {
    writer.Write(/* ... */);
    // ...
}
```

When you want the values back, just use a reader:

```C#
using (var reader = new VLQUnsignedReader(stream)) {
    var a = reader.Read();
    // ...
}
```

Easy right?

## Algorithms
The examples so far are all using VLQ (Variable Length Quantity), but there's other
algorithms to choose from. VLQ is a great all-rounder algorithm, but consider the others.
Each has different characteristics, and none is "the best". It just depends what you're
trying to achieve.

![alt text](https://raw.githubusercontent.com/invertedtomato/integer-compression/master/images/comparison-1.png "Algorithm comparison")

### Variable Length Quantities (VLQ)
 - **Random access:** no *(can't jump ahead)*
 - **Universal:** yes *(can handle any number)*
 - **Details:** [Wikipedia](https://en.wikipedia.org/wiki/Variable-length_quantity)
 - **Writer:** `VLQUnsignedWriter`, `VLQSignedWriter`
 - **Reader:** `VLQUnsignedReader`, `VLQSignedReader`
 - **Options:** 
   - Packet size

It seems VLQ was originally invented by the designers of MIDI (you know, the old-school
MP3). The algorithm is really retro, there's stacks of variations of it's spec and
it smells a little musty, but it's awesome! It produces pretty good results.

### Elias-Omega
 - **Family:** [universal code](https://en.wikipedia.org/wiki/Universal_code_(data_compression))
 - **Random access:** no (can't jump ahead)
 - **Universal:** yes (can handle any number)
 - **Supported values:** all
 - **Details:** [Wikipedia](https://en.wikipedia.org/wiki/Elias_omega_coding)
 - **Writer:** `EliasOmegaUnsignedWriter`, `EliasOmegaSignedWriter`
 - **Reader:** `EliasOmegaUnsignedReader`, `EliasOmegaSignedReader`

Elias Omega is a sexy algorithm. It's well thought out and utterly brilliant. But I
wouldn't use it. If I knew my number set was going to be small, I'd use *Elias Gamma*
instead. If I knew my number set was large, I'd use *Thompson-Alpha* instead. Sorry Omega :-/.

### Elias-Gamma
 - **Family:** [universal code](https://en.wikipedia.org/wiki/Universal_code_(data_compression))
 - **Random access:** no (can't jump ahead)
 - **Universal:** yes *(can handle any number)*
 - **Supported values:**  all
 - **Details:** [Wikipedia](https://en.wikipedia.org/wiki/Elias_gamma_coding)
 - **Writer:** `EliasGammaUnsignedWriter`, `EliasGammaSignedWriter`
 - **Reader:** `EliasGammaUnsignedReader`, `EliasGammaSignedReader`

Like Elias-Omega, this is a very interesting algorithm. However it's only really useful for small integers (less than 16). And
even then I'd suggest using *Fibonacci* instead.

### Thompson-Alpha
 - **Family:** none
 - **Random access:** no
 - **Universal:** no *(can only handle a predefined range of numbers)*
 - **Details:** N/A
 - **Writer:** `ThompsonAlphaUnsignedWriter`, `ThompsonAlphaSignedWriter`
 - **Reader:** `ThompsonAlphaUnsignedReader`, `ThompsonAlphaSignedReader`
 - **Options:** 
   - Length bits

This is my own creation, I was unhappy with the results of all of the standard algorithms for large numbers. This is the best
algorithm in the pack for numbers consistently over around 128. Numbers have a fixed overhead (by default 6 bits). So it doesn't
matter how big the number is, the overhead will always be the same.

### Fibonacci
 - **Family:** [universal code](https://en.wikipedia.org/wiki/Universal_code_(data_compression))
 - **Random access:** yes *(can jump ahead)*
 - **Lossy:** no *(doesn't approximate)*
 - **Universal:** yes *(can handle any number)*
 - **Details:** [Wikipedia](https://en.wikipedia.org/wiki/Fibonacci_coding)
 - **Writer:** `FibonacciUnsignedWriter`, `FibonacciSignedWriter`
 - **Reader:** `FibonacciUnsignedReader`, `FibonacciSignedReader`
 - **Options:** 

This is a very interesting algo. It encodes the numbers against a Fibonacci sequence. It's the best algorithm in the pack for numbers up to around 8,000 and then
pretty good for numbers that are larger. This is my personal recommendation if your numbers are typically lower than 8,000.
  
## Comparing algorithms
In order to make an accurate assessment of an algorithm for your purpose, some
algorithms have a static method `CalculateBitLength` that allows you to know
how many bits a given value would consume when encoded. I recommend getting a set
of your data and running it through the `CalculateBitLength` methods of a few
algorithms to see which one is best.

## Signed and Unsigned
If your numbers are unsigned (eg, no negatives), be sure to use **unsigned** readers and 
writers. That way you'll get the best compression. Obviously fall back to **signed**
readers and writers if you must.

## Even better compression
There are a few techniques you can use to further increase the compression of your integers.
Following is a summary of each

### Use deltas
Even with compression, smaller numbers use less space. So take a moment to consider what
you can do to keep your numbers small. One common technique is to store the difference
between numbers instead of the numbers themselves. Consider if you wanted to store the
following sequence:
 - 10000
 - 10001
 - 10002
 - 10003
 - 10004

If you converted them to deltas you could instead store:
 - 1000
 - 1
 - 2
 - 3
 - 4

This sequence uses a stack less bytes!

Naturally this isn't suitable for all contexts. If the receiver has the potential to 
loose state (eg. UDP transport) you'll have to include a recovery mechanism (eg keyframes),
otherwise those deltas become meaningless.

### Make lossy
Sometimes it's okay to loose data in compression. Let's say that you're compressing a
list of distances in meters, however you only really care about the distance rounded
to the nearest 100 meters. You can save a heap of data by dividing your value by
100 before compressing it, and multiplying it by 100 after.

### Use a false floor
Sometimes all of your values are always going to be above zero. Let's say that you're 
storing the number of cars going over a busy bridge each hour. If it's safe to assume
there will never be 0 cars you could save some data by subtracting one from your
value before compression and adding one after decompression.

This may seem like a trivial optimization, however with most algorithms it will save
you one or two bits per number. If you have several million numbers that really 
adds up.

## NuGet
The [latest build is always on NuGet](https://www.nuget.org/packages/InvertedTomato.IntegerCompression/).

