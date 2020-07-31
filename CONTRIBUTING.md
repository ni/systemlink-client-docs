# Contributing to SystemLink Client Docs

Contributions to SystemLink Client Docs are welcome from all!

systemlink-client-docs is managed via [git](https://git-scm.com), with the
canonical upstream repository hosted on
[GitHub](https://github.com/ni/systemlink-client-docs).

systemlink-client-docs follows a pull-request model for development. If you
wish to contribute, you will need to create a GitHub account, fork this
project, push a branch with your changes to your project, and then submit a
pull request.

See [GitHub's official documentation](https://help.github.com/articles/using-pull-requests/)
for more details.

## Getting Started

- To build and develop examples, download and install the
  [.NET Core SDK](https://dotnet.microsoft.com/download) for your platform.
- Use `dotnet build` and `dotnet run` commands in each example directory to
  build and run the example.
- Use `dotnet new` and `dotnet add package` commands to create new examples.

## Testing

- Each example must build and run independently using the `dotnet` command-line
  tool.
- PR builds will verify each example project builds, but developers are
  expected to verify they run with each configuration before submitting a
  pull request.

## Adding a New Example

When creating a new example project, developers should complete the following
steps:

- Create a directory named `examples/<API>/<example-name>`
- Use `dotnet new console` to create the a project within the new directory
- Use `dotnet add package Microsoft.CodeAnalysis.FxCopAnalyzers` to enable code
  analysis for the project
- Use `dotnet add package NationalInstruments.SystemLink.Clients.<API>` to
  include the API (or APIs) being demonstrated in the example
- Edit the created csproj file to include the standard ExampleConfiguration.cs
  source file:

  ```xml
  <ItemGroup>
    <Compile Include="../../ExampleConfiguration.cs" />
  </ItemGroup>
  ```

- Ensure the csproj is set to build `netcoreapp3.1` as the `TargetFramework`
- Rename the generated `Program.cs` to `<ExampleName>.cs` and the class to match
- Obtain a configuration:

  ```csharp
  /*
   * See the configuration example for how a typical application
   * might obtain a configuration.
   */
  var configuration = ExampleConfiguration.Obtain(args, allowCloud: ...);
  ```

- Implement the rest of the example, documenting the code
- Write a `README.md` describing the example
- Add links to the new example from the
  [top-level examples README](examples/README.md) and API-specific README (e.g.
  [message](examples/message/README.md))
- Add the example project to the
  [PR build workflow](.github/workflows/build-examples.yml)

## Developer Certificate of Origin (DCO)

   Developer's Certificate of Origin 1.1

   By making a contribution to this project, I certify that:

   (a) The contribution was created in whole or in part by me and I
       have the right to submit it under the open source license
       indicated in the file; or

   (b) The contribution is based upon previous work that, to the best
       of my knowledge, is covered under an appropriate open source
       license and I have the right under that license to submit that
       work with modifications, whether created in whole or in part
       by me, under the same open source license (unless I am
       permitted to submit under a different license), as indicated
       in the file; or

   (c) The contribution was provided directly to me by some other
       person who certified (a), (b) or (c) and I have not modified
       it.

   (d) I understand and agree that this project and the contribution
       are public and that a record of the contribution (including all
       personal information I submit with it, including my sign-off) is
       maintained indefinitely and may be redistributed consistent with
       this project or the open source license(s) involved.

(taken from [developercertificate.org](https://developercertificate.org/))

See [LICENSE](https://github.com/ni/systemlink-client-docs/blob/master/LICENSE)
for details about how systemlink-client-docs is licensed.
