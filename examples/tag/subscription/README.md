Tag Subscription Example
========================

This is a simple example console application demonstrating how to use the
SystemLink Tag API to receive notifications when tag values change.

Additional tag Client examples are available in the [root tag examples directory](..).

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

### Sample output

```
Creating example tags...
Subscribing to tag value changes...
Writing tag values (1/2)...
[subscription] example.subscription.double value changed to 3.14159265358979
[subscription] example.subscription.int value changed to 3
Writing tag values (2/2)...
[subscription] example.subscription.double value changed to 12.123
[subscription] example.subscription.int value changed to 12
Unsubscribing...
Deleting example tags...
```

About the Example
-----------------

This example creates a Double tag and an Int32 tag. It then subscribes to value
changes for both tags. Finally, it writes values to each tag and waits for the
subscription to send the value change notifications.

### Bulk Operations

The Tag API enables creating tags individually via the `Open` method, or in bulk
using the `Update` method. In general, applications should use bulk operations
whenever possible to reduce the number of requests made to the server.
Similarly, tag value writes are buffered using the `CreateWriter` method.

### Selections

The Tag API enables grouping sets of tags using a selection. These selections
group tags based on path, whether by a full path such as `example.subscription.double`
or a wildcard path such as `example.subscription.*`. Selections are also another
way of performing bulk operations such as reading the values or metadata for
all tags that match the selection's paths. Finally, selections provide a means
to create a tag subscription, which is the focus of this example.

See the separate [tag selection](../selection) example for more information
about the uses of selections.

### Subscriptions

Tag subscriptions monitor all tags within a selection for value changes. Unlike
selections that dynamically update as tags are created and deleted, the set of
tags monitored by a subscription is fixed at creation time. `TagChanged` events
are raised periodically on the thread pool when the values of monitored tags
change on the server. The event contains information about the tag and its new
value. When a single tag's value changes multiple times between `TagChanged`
events, only a single event is raised containing the most recent value.
