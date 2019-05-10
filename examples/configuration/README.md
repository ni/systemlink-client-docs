Configuration Example
=====================

This is a basic example console application demonstrating the different ways to
supply a configuration for SystemLink APIs.

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

There are three primary ways to configure which server the SystemLink client
APIs communicate with:

### CloudHttpConfiguration

Enables communication with [SystemLink Cloud](https://www.systemlinkcloud.com)
using an API key. NI recommends you supply the API key via a secure file
deployed with the application.

See [Getting Started with SystemLink Cloud](https://www.systemlinkcloud.com/gettingstarted)
for more information on setting up and communicating with SystemLink Cloud.

### HttpConfigurationManager

When deployed on a machine with SystemLink Server installed, or on a system
managed by a SystemLink Server, the `HttpConfigurationManager` provides client
APIs a way to communicate with the server machine automatically. NI recommends
this option for a deployed application.

Instead of using the `HttpConfigurationManager` directly, each client API has
a default constructor or method that uses the automatic configuration for
convenience.

See [Installing and Configuring SystemLink Server and Clients](http://www.ni.com/documentation/en/systemlink/latest/setup/configuring-systemlink-server-clients/)
for more information on installing SystemLink Server or a managed system.

### HttpConfiguration

During development, it's common to run an application on a machine that neither
has SystemLink Server installed, nor is a system managed by a SystemLink Server.
In this case, the `HttpConfiguration` class enables a custom connection to a
SystemLink Server by URL with an optional username and password.

When deploying an application that uses the `HttpConfiguration` class, NI
recommends you supply the username and password via user input.

See [Configuring NI Web Server](http://www.ni.com/documentation/en/ni-web-server/latest/manual/configuring-ni-web-server/)
for more information on configuring HTTP settings for a SystemLink Server.
