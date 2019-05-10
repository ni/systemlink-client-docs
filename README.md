# SystemLink Client API Documentation

This repository contains getting started guides, examples, and reference
documentation for the SystemLink Client NuGet packages. These packages support
.NET Standard 2.0. Therefore, they support both .NET Core 2.0 and .NET
Framework 4.6.1 with Visual Studio 2017 or later.

## Client APIs

Each client API has its own NuGet package for interacting with a specific
SystemLink service or collection of related services:

| Service | Package name                                   | Package
|---------|------------------------------------------------|------------------
| Tag     | NationalInstruments.SystemLink.Clients.Tag     | Not yet published
| Message | NationalInstruments.SystemLink.Clients.Message | Not yet published

## Getting Started

To use a SystemLink client in your application, add a reference to the NuGet
package for the service or services you want to interact with.

#### .NET Core

```
dotnet add package NationalInstruments.SystemLink.Clients.Tag
```

#### .NET Framework

Use the NuGet Package Manager or the NuGet console to add a reference to the
package:

```
Install-Package NationalInstruments.SystemLink.Clients.Tag
```

### Examples

The below getting started example creates a [double tag](wiki/Tag) on
[SystemLink Cloud](https://www.systemlinkcloud.com), writes two values to it,
then reads back and outputs the current value. The tag is deleted at the end
of the example. See the [examples directory](examples) to browse additional
code examples.

```csharp
using System;
using NationalInstruments.SystemLink.Clients.Core;
using NationalInstruments.SystemLink.Clients.Tag;
using NationalInstruments.SystemLink.Clients.Tag.Values;

namespace GettingStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiKey = args[0]; // Not a secure way to load an API key.
            var config = new CloudHttpConfiguration(apiKey);

            using (var manager = new HttpTagManager(config))
            using (var writer = manager.CreateWriter(maxBufferTime: TimeSpan.FromSeconds(1)))
            {
                var tag = manager.Open("example.gettingstarted.double", DataType.Double);
                var tagReader = new DoubleTagValueReader(manager, tag);
                var tagWriter = new DoubleTagValueWriter(writer, tag);

                tagWriter.Write(Math.E);
                tagWriter.Write(Math.PI);
                writer.SendBufferedWrites();

                Console.WriteLine(tagReader.Read());
                manager.Delete(new[] { tag });
            }
        }
    }
}
```

## Documentation

See the [Wiki](wiki) for each client API's documentation.
