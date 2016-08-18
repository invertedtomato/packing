# InvertedTomato.IntegerCompression
## Introduction
There are times when it's really inconvenient that an integer is 4 bytes long (or 2 bytes, or 8 bytes, depending on the variant). In particular, when it comes to storing a large number of integers to disk, or when transmitting them over a network it would be really great if they could be smaller. Welcome to the field of integer compression! 

Did you know that it's possible to store *up to three integers in a single byte?* Yep, that's what this library is about. It's not simple, but if this is what you're looking for, read on.

## Algorithms
There is no one-size-fits-all algorithm for integer compression. There's a range that have different attributes depending 
on what you're trying to achieve. First lets look at unbounded algorithms - these algorithms can encode any number that .NET 
natively supports (0 to 18,446,744,073,709,551,615).

### Elias Omega


### Elias Gamma
The best algorithm if you have consistently tiny numbers (less than 16).

| Range | Values |
|---------|------|
| Supported (normal) | 1 to 18,446,744,073,709,551,615 |
| Supported (with zeros)| 0 to 18,446,744,073,709,551,614 |
| Recommended (normal) | 1 to 16 |
| Recommended (with zeros) | 0 to 15 |


NOT YET IMPLIMENTED

## Variable Length Quantities (VLQ)

### Mini-Fixed


## General Advice
Use delta encoding