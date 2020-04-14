using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NationalInstruments.SystemLink.Clients.File;

namespace NationalInstruments.SystemLink.Clients.Examples.File
{
    /// <summary>
    /// Example for the SystemLink Test Monitor API that creates a test result
    /// and associated steps simulating a current and voltage sweep.
    /// </summary>
    class Stream
    {
        static void Main(string[] args)
        {
            /*
             * See the configuration example for how a typical application
             * might obtain a configuration.
             */
            var configuration = ExampleConfiguration.Obtain(args);

            /*
             * Create the TestDataManager for communicating with the server.
             */
            var fileUploader = new FileUploader(configuration);

            // Upload some sample data as the file contents.
            var fileId = UploadFileUsingStream(
                configuration,
                "stream.txt",
                Encoding.UTF8.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        }

        public static string UploadFileUsingStream(Core.IHttpConfiguration configuration, string fileName, byte[] fileContents)
        {
            // Use the FileUploader from the SystemLink File client
            var fileUploader = new FileUploader(configuration);

            // Use in-memory data for the file upload.
            using (var memoryStream = new MemoryStream())
            {
                // Write the file content as bytes to the stream.
                memoryStream.Write(fileContents, 0, fileContents.Length);

                // Reset the stream position to the beginning to upload from the beginning.
                memoryStream.Position = 0;

                // Upload the file to the SystemLink server and get the uploaded file's Id.
                var fileId = fileUploader.UploadFile(memoryStream, fileName);
                return fileId;
            }
        }
    }
}