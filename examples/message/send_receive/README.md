Message Send and Receive Example
================================

This is a simple example console application demonstrating how to use the
SystemLink Message client API to send and receive messages between sessions.

Additional message client examples are available in the
[root message examples directory](..).

Running the Example
-------------------

1. Download and extract the [repository source](https://github.com/ni/systemlink-client-docs/archive/master.zip).
2. Install the [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core).
3. Navigate to the example's directory and use the [`dotnet run` command](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run?tabs=netcore21).

To run the example with a different configuration, use one of the following
commands instead:

```
dotnet run -- --cloud <api_key>
dotnet run -- --server <url> <username> <password>
```

For example: `dotnet run -- --server https://my_server admin "my password"`.

About the Example
-----------------

This example creates two message sessions. From one session, it publishes both
JSON data messages and a control message. From the other session, it subscribes
and then reads the published messages.

### Message Sessions and Queues

A message session represents a queue for receiving messages published by other
sessions connected to the same SystemLink Server or SystemLink Cloud instance.
Each session receives its own copy of messages, allowing 1 to N communication
between applications, potentially running on different systems.

### Topics and Subscriptions

Every published message includes a topic, which is a simple, usually short,
string identifying the type of data contained in the message or the intended
type of recipient. For example, applications may use one topic for sending
control messages to systems and another for issuing status updates. Sessions
must subscribe to a topic before it receives any messages for that topic. When
unsubscribing from a topic, messages that are already queued but not yet read
are dropped.

Sessions are not required to subscribe to any topics if they are only used for
publishing messages. A session that publishes a message to a topic it is also
subscribed to will receive a copy of its own message.

### Reading Messages

When a session receives a message to a subscribed topic, it is queued on the
server. Applications must read from the session in order to dequeue a message
from the server. When reading a message from an empty queue, the API call will
block up to a given amount of time waiting for a message before eventually
returning null to indicate the queue is empty.

### Message Session Limits

Both SystemLink Cloud and SystemLink Server define a maximum number of message
sessions that may be open at a time. The server will automatically close idle
sessions that neither publish nor read messages after a few minutes.
Additionally, SystemLink Cloud has a maximum number of queued bytes per session
before received messages are dropped. For SystemLink Server, this maximum is
disabled by default, but system administrators can set a maximum.
