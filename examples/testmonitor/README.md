SystemLink Test Monitor Service API Examples
====================================

This directory holds self-contained example projects for the SystemLink Test Monitor service API. To make finding relevant examples easier, a summary of each one is listed below.

Examples for other SystemLink Client APIs are available in the
[root examples directory](..).

Running an Example
------------------

Unless otherwise specified by the example's README, each example is run in the
same way:

1. Download and extract the [repository source](https://github.com/ni/systemlink-client-docs/archive/master.zip).
2. Install the [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core).
3. Navigate to the example's directory and use the [`dotnet run` command](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run?tabs=netcore21).

To run the example, use the following command:

```
dotnet run -- --server <url> <username> <password>
```

For example: `dotnet run -- --server https://my_server admin "my password"`.

Test Monitor Examples
------------

- [Results](results): Demonstrates how to use the SystemLink Test Monitor API to create test results and steps.
- [File Attachment](fileattachment): Demonstrates how to use the SystemLink Test Monitor API to attach files that are uploaded to the SystemLink server using the SystemLink File API.

