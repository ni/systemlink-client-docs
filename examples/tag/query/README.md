Tag Query Example
=================

This is a simple example console application demonstrating how to use the
SystemLink Tag API to query for tags by properties to retrieve metadata for
each tag.

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

Note that tag order is not guaranteed by the Query API and may differ between
SystemLink Cloud and SystemLink Server.

```
Creating example tags...

------------------+-----------+----------+--------------------------
Tag Path          | Data Type | Keywords | Properties
------------------+-----------+----------+--------------------------
example.query.094 | String    | example  | example=query, number=94
example.query.031 | DateTime  | example  | example=query, number=31
example.query.045 | Int32     | example  | example=query, number=45
example.query.062 | Double    | example  | example=query, number=62
example.query.012 | Bool      | example  | example=query, number=12
example.query.098 | Double    | example  | example=query, number=98
example.query.083 | UInt64    | example  | example=query, number=83
example.query.076 | String    | example  | example=query, number=76
example.query.024 | Bool      | example  | example=query, number=24
example.query.100 | String    | example  | example=query, number=100

10/100 tags - Press any key to load the next page or Esc to quit

Deleting example tags...
```

About the Example
-----------------

This example creates 100 tags under the `example.query.*` prefix, performs a
query to retrieve the metadata for each tag, then deletes the tags. The query
uses a `take` value to paginate the results, requesting a subset of tags from
the server each time the example iterates over the query result. The tag
metadata is written to the console in a tabular format.

### Tag Keywords and Properties

A tag's metadata includes keywords, which are simple strings, and properties,
which are string key-value pairs. These pieces of metadata enable storing
information about the tag, such as which node created the tag or its purpose.
Applications can query for tags with certain keywords or properties with certain
values. This example sets a property named `example` with value `query` on each
tag that it creates, then queries for all tags with that property value.

Tag queries can also include paths. Instead of using a property query, this
example could have used a path query of `example.query.*` to locate every tag
created by the example.

### Bulk Operations

The Tag API enables creating tags individually via the `Open` method, or in bulk
using the `Update` method. In general, applications should use bulk operations
whenever possible to reduce the number of requests made to the server.
Similarly, tag values can be read individually via the `TagValueReader` classes
or in bulk using an `ITagSelection`. Tag value writes are buffered using the
`CreateWriter` method.
