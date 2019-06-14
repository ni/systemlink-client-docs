Tag Selection Example
=====================

This is a simple example console application demonstrating how to use the
SystemLink Tag API to read metadata and values for multiple tags at once.

Additional Tag Client examples are available in the [root tag examples directory](..).

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
Creating selection...
Double tag value: 3.14159265358979
Int tag value: 3
Creating additional tags...
Tags currently in the selection:
-------------------------+-----------+----------+-----------------
Tag Path                 | Data Type | Keywords | Value
-------------------------+-----------+----------+-----------------
example.selection.double | Double    |          | 3.14159265358979
example.selection.int    | Int32     |          | 3
-------------------------+-----------+----------+-----------------
Refreshing selection values...
Tags currently in the selection:
-------------------------+-----------+----------+-----------------
Tag Path                 | Data Type | Keywords | Value
-------------------------+-----------+----------+-----------------
example.selection.double | Double    |          | 2.71828182845905
example.selection.int    | Int32     |          | 2
example.selection.bool   | Bool      |          | True
example.selection.string | String    |          | example
-------------------------+-----------+----------+-----------------
Refreshing selection metadata...
Tags currently in the selection:
-------------------------+-----------+----------+-----------------
Tag Path                 | Data Type | Keywords | Value
-------------------------+-----------+----------+-----------------
example.selection.double | Double    | edited   | 2.71828182845905
example.selection.int    | Int32     | edited   | 2
example.selection.bool   | Bool      | new      | True
example.selection.string | String    | new      | example
-------------------------+-----------+----------+-----------------
Deleting example tags...
```

About the Example
-----------------

This example creates a Double tag and an Int32 tag. It then creates a selection
containing all tags whose path starts with `example.selection.`. Finally, the
example demonstrates how changes made to tag values and metadata are reflected
in the selection when refreshed.

### Bulk Operations

The Tag API enables creating tags individually via the `Open` method, or in bulk
using the `Update` method. In general, applications should use bulk operations
whenever possible to reduce the number of requests made to the server.
Similarly, tag value writes are buffered using the `CreateWriter` method.

### Selections

If you want to group sets of tags, use a selection. A selection groups tags
based on the path(s) provided, such as an exact tag path
(e.g. `example.subscription.double`) or with a wildcard path
(e.g. `example.subscription.*`). Selections are also another way of performing
bulk operations, such as reading the values or metadata for all tags that match
the selection's paths. Finally, selections provide a means to create a tag
subscription, which enables receiving events when tag values change.

See the separate [tag subscription](../subscription) example for more
information about the uses of subscriptions.

### Refreshing Selections

When a selection is refreshed, the server reevaluates the tag paths included in
the selection to determine any tags that have been created or deleted since the
previous refresh. Refreshing metadata retrieves each tag's keywords and
properties, while refreshing values retrieves each tag's current and aggregate
values. The Tag API provides methods to refresh metadata and values separately
or together in a single API call.

Creating a selection using the `Open` method automatically refreshes metadata,
while using the `CreateSelection` method does not. Additionally, selections
will automatically refresh values when reading a tag value for the first time
after creating a selection. This means when using selections with asynchronous
programming, applications should call `RefreshValuesAsync` prior to reading
values for the first time.
