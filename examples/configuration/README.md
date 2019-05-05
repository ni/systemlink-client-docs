Configuration Example
=====================

This is a basic example console application demonstrating the different ways to
supply a configuration for SystemLink APIs.

Running the Example
-------------------

1. Download and extract the [repository source](https://github.com/ni/systemlink-client-docs/archive/master.zip)
2. Install the [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core)
3. Open a command-prompt within this directory and use the `dotnet run` command

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
using an API key. The recommendation is to supply the API key via a secure file
deployed with the application.

### HttpConfigurationManager

When deployed on a machine with SystemLink Server installed, or on a system
managed by a SystemLink Server, the `HttpConfigurationManager` provides client
APIs a way to communicate with the server machine automatically. This is the
recommended option for a deployed application.

Instead of using the `HttpConfigurationManager` directly, each client API has
a default constructor or method that uses the automatic configuration for
convenience.

### HttpConfiguration

During development, it's common to run an application on a machine that neither
has SystemLink Server installed, nor is a system managed by a SystemLink Server.
In this case, the `HttpConfiguration` class enables a custom connection to a
SystemLink Server by URL with an optional username and password.

When deploying an application that uses the `HttpConfiguration` class, the
recommendation is to supply the username and password via user input.
