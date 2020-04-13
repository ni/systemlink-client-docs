SystemLink Client API Examples
==============================

This directory holds self-contained example projects for various aspects of the
SystemLink Client APIs. To make finding relevant examples easier, a summary of
each one is listed below.

Running an Example
------------------

Unless otherwise specified by the example's README, each example is run in the
same way:

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

Common Examples
---------------

The following examples demonstrate concepts common across all SystemLink Client
APIs:

- [configuration](configuration): Demonstrates the different ways to supply a
  configuration for SystemLink APIs.

API Examples
------------

### [Asset](asset)

- [Utilization](asset/utilization): Demonstrates how to use the SystemLink Asset Management
  Client API to track asset utilization.

### [Message](message)

- [Send and Receive](message/send_receive): Demonstrates how to use the
  SystemLink Message Client API to send and receive messages between sessions.
- [LabVIEW Async](message/labview): Demonstrates how to use the SystemLink Message
  Client API to asynchronously send and receive messages as part of the
  Async Messaging LabVIEW example.

### [Tag](tag)

- [Query](tag/query): Demonstrates how to use the SystemLink Tag services API to
  query for tags by properties to retrieve metadata for each tag.
- [Selection](tag/selection): Demonstrates how to use the SystemLink Tag API to
  read metadata and values for multiple tags at once.
- [Subscription](tag/subscription): Demonstrates how to use the SystemLink Tag API
  to receive notifications when tag values change.

### [Test Monitor](testmonitor)

- [Results](testmonitor/results): Demonstrates how to use the SystemLink Test Monitor API to publish test results to the server.