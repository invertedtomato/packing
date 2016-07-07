The wing is an optional addition to feather that binds the payload together in a standard interface.
It specifies the payload should be defined as (byte) operation code,payload and it additionally defines how various formats should be encoded. eg:


DateTime				uint64 Seconds since Unix epoch (unix time)
timespan				uint32 seconds
String					uint16 length, followed by utf8 byte array
Guid					byte[16]
Boolean					uint8 (0,1) - either a 1 or a 0 encoded as a whole byte
fixed length byte[]		just byte array
variable length byte[]	uint16 length, followed by byte array
bit[]					ceiling(n/8) bytes
