# InvertedTomato
InvertedTomato is a set of libaries we use in-house to power our applications. There's some pretty cool things in here. Individual libraries are published to NuGet seperately when they reach maturity.

## InvertedTomato.Feather
Feather is extremely fast and lightweight network messaging socket. Kinda like WCF, without the nonsense and scolding fast speeds. Great for applications communicating over a network when webAPI is too slow or inefficient. SSL encryption is optional.

Here's a chat server/client example to get you going.

### Client
```C#
class Program {
    static void Main(string[] args) {
        // Connect to server
        using (var client = FeatherTCP<Connection>.Connect("localhost", 777)) {
            // Get user's name
            Console.WriteLine("What's your name?");
            var userName = Console.ReadLine();

            // Go in a loop, sending any message the user types
            Console.WriteLine("Ready. Type your messages to send.");
            while (true) {
                Console.Write(userName + "> ");
                var message =  Console.ReadLine();
                if (!string.IsNullOrEmpty(message)) {
                    client.SendMessage(userName, message);
                }
            }
        }
    }
}

class Connection : ConnectionBase {
    public void SendMessage(string emailAddress, string password) {
        // Compose message payload ready for sending
        var payload = new PayloadWriter(5) // "5" is the opcode, identifying what type of message we're sending
            .Append(emailAddress)
            .Append(password);

        // Send it to the server
        Send(payload);
    }

    protected override void OnMessageReceived(PayloadReader payload) {
        // Detect what type of message has arrived
        switch (payload.OpCode) {
            case 5: // Oh, it's a chat message
                // Get parameters (in the same order they were sent)
                var userName = payload.ReadString();
                var message = payload.ReadString();

                // Print it on the screen
                Console.WriteLine(userName + "> " + message);
                break;
            default:
                // Report that an unknown opcode arrived
                Console.WriteLine("Unknown message arrived with opcode " + payload.OpCode);
                break;
        }
    }
}
```

### Server
```C#
class Program {
    public static ConcurrentDictionary<EndPoint, Connection> Connections = new ConcurrentDictionary<EndPoint, Connection>();

    static void Main(string[] args) {
        // Start listening for connections
        using (var server = FeatherTCP<Connection>.Listen(777)) {
            // Watch for connections
            server.OnClientConnected += OnConnect;

            // Keep running until stopped
            Console.WriteLine("Chat server running. Press any key to terminate.");
            Console.ReadKey(true);
        }
    }

    static void OnConnect(Connection connection) {
        // Get remote end point
        var remoteEndPoint = connection.RemoteEndPoint;

        // Add to list of current connections
        Connections[remoteEndPoint] = connection;
        Console.WriteLine(remoteEndPoint.ToString() + " has connected.");

        // Setup to remove from connections on disconnect
        connection.OnDisconnected += (reason) => {
            Connections.TryRemove(remoteEndPoint, out connection);
            Console.WriteLine(remoteEndPoint.ToString() + " has disconnected.");
        };
    }
}

class Connection : ConnectionBase {
    protected override void OnMessageReceived(PayloadReader payload) {
        // Detect what type of message has arrived
        switch (payload.OpCode) {
            case 5:// Oh, it's a chat message
                // Get parameters (in the same order they were sent)
                var userName = payload.ReadString();
                var message = payload.ReadString();

                // Print it on the screen
                Console.WriteLine(userName + "> " + message);

                // Forward message to all OTHER clients
                foreach (var connection in Program.Connections.Values.Where(a => a != this)) {
                    connection.Send(payload);
                }
                break;
            default:
                // Report that an unknown opcode arrived
                Console.WriteLine("Unknown message arrived with opcode " + payload.OpCode);
                break;
        }
    }
}
```


### Writing data file
Feather can also be used to write really small data files. It's great if you need to archive data and it doesn't fit the bill for SQL. We use this to archive large volumes of data in Amazon S3.
```C#
// Open file
using (var file = FeatherFile.OpenWrite("test.dat")) {
	for (var i = 1; i < 10000000; i++) {
		// Create payload to write to file
		var payload = new PayloadWriter(0)
			.Append("This is a peice of text.") // Param #1
			.Append(2); // Param #2

		// Write it
		file.Write();

		// Repeat this as many times as you want...
	}
}
```

### Reading data file
```C#
// Open file
using (var file = FeatherFile.OpenRead("test.dat")) {
	// Iterate through each record
    PayloadReader payload;
    while ((payload = file.Read()) != null) {
        // Read parameters (must be in the same order they were written!)
		var param1 = payload.ReadString(); // Param #1
        var param2 = payload.ReadInt32(); // Param #2

		// Do whatever with the data
    }
}
```