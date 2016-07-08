The "Wing" extensions are an optional addition to Feather that provides a standard method of encoding common data types in byte arrays.

DateTime					uint64 seconds since Unix epoch (unix time)
TimeSpan					uint32 seconds
String						uint16 length, followed by utf8 byte array
Guid						byte[16]
Boolean						uint8 (0,1) - either a 1 or a 0 encoded as a whole byte
byte[] (fixed-length)		just byte array
byte[] (variable-length)	uint16 length, followed by byte array
bit[]						ceiling(n/8) bytes
