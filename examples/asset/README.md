SystemLink Asset Management Client API Examples
======================================

This directory holds self-contained example projects for the SystemLink Asset Management
Client API. To make finding relevant examples easier, a summary of each one is
listed below.

Examples for other SystemLink Client APIs are available in the
[root examples directory](..).

Running an Example
------------------

Unless otherwise specified by the example's README, each example is run in the
same way:

1. Download and extract the [repository source](https://github.com/ni/systemlink-client-docs/archive/master.zip).
2. Install the [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core).
3. Navigate to the example's directory and use the [`dotnet run` command](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run?tabs=netcore21).

Asset Management Examples
----------------

- [Utilization](utilization): Demonstrates how to use the SystemLink Asset Management
  Client API to track asset utilization.
  Run the example on a machine with SystemLink Client. If the system is not managed, utilization data is stored locally on the system. 
  Once the system is managed by a SystemLink Server, utilization data is synchronized.