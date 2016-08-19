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
 - **Lossy:** no *(doesn't approximate)*
 - **Universal:** yes *(can handle any number)*
 - **Supported values:**  0 to 18,446,744,073,709,551,615
 - **Details:** [Wikipedia](https://en.wikipedia.org/wiki/Variable-length_quantity)
 - **Writer:** `VLQUnsignedWriter`, `VLQSignedWriter`
 - **Reader:** `VLQUnsignedReader`, `VLQSignedReader`
 - **Options:** 
   - Specify minimum number of bytes

It seems VLQ was originally invented by the designers of MIDI (you know, the old-school
MP3). The algorithm is really retro, there's stacks of variations of it's spec and
it smells a little musty, but it's awesome! It produces consistently good results
for all input numbers.

If you don't know the average distribution of numbers (some might be tiny, some might
be huge) then this is my algorithm of choice. It's also very light on CPU, so if that's
important to you this might be your choice anyway.

Notes:
 - This implementation includes the "redundancy removal" variation. So don't expect
it to be compatible with other implementations.
 - The constructor lets you specify a minimum number of bytes. This lets you increase efficiency if you are expecting large numbers (set minBytes=2 if your numbers are always >127. Set minBytes=3 if your numbers are always >65,535, Set minBytes=4 if your numbers are always >16,777,215)
 - By the by, I hate the name "Variable Length Quantities".

### Elias Omega
 - **Family:** [universal code](https://en.wikipedia.org/wiki/Universal_code_(data_compression))
 - **Random access:** no (can't jump ahead)
 - **Lossy:** no (doesn't approximate)
 - **Universal:** yes (can handle any number)
 - **Supported values *(standard)*:**  1 to 18,446,744,073,709,551,615
 - **Supported values *(with zeros)*:** 0 to 18,446,744,073,709,551,614
 - **Details:** [Wikipedia](https://en.wikipedia.org/wiki/Elias_omega_coding)
 - **Writer:** `EliasOmegaUnsignedWriter`, `EliasOmegaSignedWriter`
 - **Reader:** `EliasOmegaUnsignedReader`, `EliasOmegaSignedReader`
 - **Options:** 
   - Allow zeros to be included

Elias Omega is a sexy algorithm. It's well thought out and utterly brilliant. But I
wouldn't use it. If I knew my number set was going to be small, I'd use *Elias Gamma*
instead. If I knew my number set was large, I'd use *VLQ* instead. Sorry Omega :-/.

Notes:
 - If you want zeros, you need to pass allowZeros=TRUE in the constructor. It decreases the efficiency slightly, so only use it if you need it.
 - Uses a fair bit more RAM/CPU than VLQ, but it's still modest. Think twice if you're trying to use it realtime with a huge set of data.

### Elias Gamma

***Coming soon. This algorithm isn't yet fully implemented.***

 - **Family:** [universal code](https://en.wikipedia.org/wiki/Universal_code_(data_compression))
 - **Random access:** no (can't jump ahead)
 - **Lossy:** no (doesn't approximate)
 - **Universal:** yes *(can handle any number)*
 - **Supported values *(standard)*:**  1 to 18,446,744,073,709,551,615
 - **Supported values *(with zeros)*:** 0 to 18,446,744,073,709,551,614
 - **Details:** [Wikipedia](https://en.wikipedia.org/wiki/Elias_gamma_coding)
 - **Writer:** `EliasGammaUnsignedWriter`, `EliasGammaSignedWriter`
 - **Reader:** `EliasGammaUnsignedReader`, `EliasGammaSignedReader`
 - **Options:** 
   - Allow zeros to be included

This is the best algorithm for when you're expecting consistently tiny numbers, but 
need to handle the occasional larger value. It beats all other algorithms for size
on numbers <=16.

Notes:
 - If you want zeros, you need to pass allowZeros=TRUE in the constructor. It decreases the efficiency slightly, so only use it if you need it.
 - Uses a fair bit more RAM/CPU than VLQ, but it's still modest. Think twice if you're trying to use it realtime with a huge set of data.

### FlexiFixed

***Coming soon. This algorithm isn't yet fully implemented.***

 - **Family:** none
 - **Random access:** yes *(can jump ahead)*
 - **Lossy:** optional
 - **Universal:** no *(can only handle a predefined range of numbers)*
 - **Supported values:**  0 to 18,446,744,073,709,551,615
 - **Details:** N/A
 - **Writer:** `FlexiFixedUnsignedWriter`, `FlexiFixedSignedWriter`
 - **Reader:** `FlexiFixedUnsignedReader`, `FlexiFixedSignedReader`
 - **Options:** 
   - Minimum value
   - Maximum value
   - Step between values


### Fibonacci

***Coming soon. This algorithm isn't yet fully implemented.***

 - **Family:** [universal code](https://en.wikipedia.org/wiki/Universal_code_(data_compression))
 - **Random access:** yes *(can jump ahead)*
 - **Lossy:** no *(doesn't approximate)*
 - **Universal:** yes *(can handle any number)*
 - **Supported values *(standard)*:**  1 to 18,446,744,073,709,551,615
 - **Supported values *(with zeros)*:** 0 to 18,446,744,073,709,551,614
 - **Details:** [Wikipedia](https://en.wikipedia.org/wiki/Fibonacci_coding)
 - **Writer:** `FibonacciUnsignedWriter`, `FibonacciSignedWriter`
 - **Reader:** `FibonacciUnsignedReader`, `FibonacciSignedReader`
 - **Options:** 
   - Allow zeros to be included

  
## Signed and Unsigned
If your numbers are unsigned (eg, no negatives), be sure to use **unsigned** readers and 
writers. That way you'll get the best compression. Obviously fall back to **signed**
readers and writers if you must.

## Warning about zeros
Not all algorithms support zeros natively. It's a compression thing. But we've included
work-around support for those algorithms. All you need to do is pass in AllowZeros=TRUE
on the constructors. It'll make the algorithm a tiny bit less efficient, but hey, you
need zeros.

## Use small numbers
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

## Comparing algorithms
In order to make an accurate assessment of an algorithm for your purpose, some
algorithms have a static method `CalculateBitLength` that allows you to know
how many bits a given value would consume when encoded. I recommend getting a set
of your data and running it through the `CalculateBitLength` methods of a few
algorithms to see which one is best.

## NuGet
The [latest build is always on NuGet](https://www.nuget.org/packages/InvertedTomato.IntegerCompression/).