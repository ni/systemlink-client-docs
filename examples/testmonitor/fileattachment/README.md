Test Monitor File Attachment Example
=================

This is an example console application demonstrating how to use the
SystemLink Test Monitor API attach a file to a test result.

Running the Example
-------------------

1. Download and extract the [repository source](https://github.com/ni/systemlink-client-docs/archive/master.zip).
2. Install the [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet-core).
3. Navigate to the example's directory and use the [`dotnet run` command](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-run?tabs=netcore21).

To run the example, use the following command:

```
dotnet run -- --server <url> <username> <password>
```

For example: `dotnet run -- --server https://my_server admin "my password"`.

About the Example
-----------------

This example creates a single test result and attaches a file.  It uses the SystemLink File client to upload an in-memory byte stream as the contents of a file.  The file client allows different methods of uploading files, but for the purpose of this example the stream upload is utilized.  The resulting file Id is added to the test result data and updated on the server.

### Sample output

The data published to the SystemLink server can be viewed in the [Test Monitor Web Application](https://localhost/#testmonitor).  The test result details and the attached files can be explored using a web browser:
![Test result and steps](./FileAttachment.png "Test result and steps")

Selecting the "Attachments" tab displays the list of files attached to the test result.  Attached files can be downloaded or removed.  Files with certain extensions (ex: txt, png, pdf) can be previewed in the browser.