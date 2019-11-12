Asset Utilization Example
=====================

This is a basic example console application demonstrating how to track asset utilization
using SystemLink Asset Utilization APIs.

Running the Example
-------------------

1. Download and extract the [repository source](https://github.com/ni/systemlink-client-docs/archive/master.zip).
2. Install the [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core).
3. Navigate to the example's directory and use the [`dotnet run` command](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run?tabs=netcore21).


Run the example on a machine with SystemLink Client. If the system is not managed, utilization data is stored locally on the system.
Once the system is managed by a SystemLink Server, utilization data is synchronized.
See [Installing and Configuring SystemLink Server and Clients](http://www.ni.com/documentation/en/systemlink/latest/setup/configuring-systemlink-server-clients/)
for more information on installing SystemLink Server or a managed system.

About the Example
-----------------

This example tracks utilization for assets present in the current system. 
The utilization data is stored locally. Once the system is managed and a connection to the SystemLink Server is established,
the utilization data is synchronized.

### Starting and Ending Asset Utilization

To start tracking asset utilization, the `StartUtilization` method on the `AssetUtilizationStore` needs to be called.
It returns an `IStartedUtilization`, which represents a started utilization session. 
To end an ongoing asset utilization, the `IStartedUtilization` needs to be disposed.

### Heartbeats

Asset Utilization tracking can send heartbeats for ongoing utilizations at regular intervals. 
This is enabled by default and heartbeats are sent every 5 minutes for ongoing utilizations.
Heartbeats can be enabled/disabled and their interval can be configured by creating a new `StartUtilizationConfiguration` and 
specifying `AutogenerateHeartbeats` and `HeartbeatIntervalInMilliseconds`.