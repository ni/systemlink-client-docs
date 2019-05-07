/*
 * This file supports other examples. See configuration/Configuration.cs for
 * the configuration example.
 */

using System;
using NationalInstruments.SystemLink.Clients.Core;

namespace NationalInstruments.SystemLink.Clients.Examples
{
    /// <summary>
    /// Helper class to support client API examples obtaining a configuration.
    /// </summary>
    static class ExampleConfiguration
    {
        /// <summary>
        /// Helper method to support client API examples obtaining a
        /// configuration. Exits if a suitable configuration is not available.
        /// </summary>
        /// <param name="args">The arguments used to run the example.</param>
        /// <returns>A configuration to use for the example.</returns>
        public static IHttpConfiguration Obtain(string[] args)
        {
            if (args?.Length > 0)
            {
                switch (args[0])
                {
                    case "--cloud":
                        return ObtainCloudOrExit(args);
                    case "--server":
                        return ObtainServerOrExit(args);
                    case "--help":
                        return PrintUsageAndExit(string.Empty);
                    default:
                        return PrintUsageAndExit("Invalid argument " + args[0]);
                }
            }
            else
            {
                return ObtainDefaultOrExit();
            }
        }

        private static IHttpConfiguration ObtainCloudOrExit(string[] args)
        {
            if (args.Length != 2)
            {
                return PrintUsageAndExit("--cloud requires 1 API key argument");
            }

            return new CloudHttpConfiguration(args[1]);
        }

        private static IHttpConfiguration ObtainDefaultOrExit()
        {
            try
            {
                var manager = new HttpConfigurationManager();
                return manager.GetConfiguration();
            }
            catch (ApiException ex)
            {
                return PrintUsageAndExit(ex.Message);
            }
        }

        private static IHttpConfiguration ObtainServerOrExit(string[] args)
        {
            try
            {
                switch (args.Length)
                {
                    case 1:
                        return PrintUsageAndExit("--server requires URL and optional username and password");

                    case 2:
                        return new HttpConfiguration(new Uri(args[1]));

                    case 3:
                        return PrintUsageAndExit("--server requires password when specifying a username");

                    case 4:
                        return new HttpConfiguration(new Uri(args[1]), args[2], args[3]);

                    default:
                        return PrintUsageAndExit("--server does not take more arguments than URL, username, and password");
                }
            }
            catch (FormatException ex)
            {
                return PrintUsageAndExit("Invalid URL for --server: " + ex.Message);
            }
        }

        private static IHttpConfiguration PrintUsageAndExit(string error)
        {
            Console.Error.WriteLine("This example requires a configuration.");
            Console.Error.WriteLine(error);
            Console.Error.WriteLine();
            Console.Error.WriteLine("When running on a machine with SystemLink Server installed or on a system");
            Console.Error.WriteLine("managed by a SystemLink Server, do not specify any arguments. Otherwise,");
            Console.Error.WriteLine("specify a configuration using one of the following arguments:");
            Console.Error.WriteLine();
            Console.Error.WriteLine("\t--cloud <api_key>");
            Console.Error.WriteLine("\t--server <url> [<username> <password>]");
            Console.Error.WriteLine();
            Console.Error.WriteLine("Generate an API key on SystemLink Cloud. Go to https://www.systemlinkcloud.com,");
            Console.Error.WriteLine("log in with your NI User Account, and click Security to create an API key. To");
            Console.Error.WriteLine("run the example against a SystemLink Server, the URL should include the scheme,");
            Console.Error.WriteLine("host, and port if not default. For example:");
            Console.Error.WriteLine("dotnet run -- --server https://myserver:9091 admin my_password");
            Environment.Exit(1);
            return null;
        }
    }
}
