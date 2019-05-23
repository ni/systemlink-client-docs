Message LabVIEW Async Example
=============================

This is an example console application demonstrating how to use the SystemLink
Message Client API to asynchronously send and receive messages as part of the
Async Messaging LabVIEW example.

Additional Message Client examples are available in the
[root message examples directory](..).

Running the Example
-------------------

This example is designed to run in conjunction with a LabVIEW example. Because
the LabVIEW example uses AMQP, it does not support SystemLink Cloud. For best
results, do not run this example with the --cloud argument.

1. Install the SystemLink Client from NI Package Manager to add SystemLink
   Message support to LabVIEW.
2. Download and extract the [repository source](https://github.com/ni/systemlink-client-docs/archive/master.zip).
3. Install the [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core).
4. Navigate to the example's directory and use the [`dotnet run` command](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run?tabs=netcore21).
5. From the LabVIEW help menu, select Find Examples to open NI Example Finder.
   On the Search tab, search for messaging and open `Async Messaging.lvproj`.
6. Open and run `Host.vi` under My Computer to interact with the running example.
7. When finished, stop the VI and use Control+C to stop the dotnet command.

To run the example with a different configuration, use one of the following
commands instead:

```
dotnet run -- --cloud <api_key>
dotnet run -- --server <url> <username> <password>
```

For example: `dotnet run -- --server https://my_server admin "my password"`.

### Sample output

```
Opening message session...
Run Host.vi to send and receive messages from this example.
Press Ctrl+C to stop.

Received message:
        Date: 5/16/2019
        Time: 4:22:17 PM
        Topic: Example to Client
        Message: This is a message sent from LabVIEW

Stopping...
Message session closed
Published 8 and received 1 messages
```

About the Example
-----------------

This example creates a message session and subscribes to receive messages sent
by the LabVIEW Host VI. Additionally, the example publishes a message every
second that the Host VI receives and displays in the Received Messages list.
This demonstrates how separate applications connected to the same SystemLink
Server or SystemLink Cloud instance can communicate with each other.

See the [Message Send and Receive example](../send_receive) for more information
about using the SystemLink Message Client API and related concepts.
