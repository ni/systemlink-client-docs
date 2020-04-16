using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NationalInstruments.SystemLink.Clients.File;
using NationalInstruments.SystemLink.Clients.TestMonitor;

namespace NationalInstruments.SystemLink.Clients.Examples.TestMonitor
{
    /// <summary>
    /// Example for the SystemLink Test Monitor API that creates a test result,
    /// uploads a file to the SystemLink server, and attaches the file to the
    /// test result.
    /// </summary>
    class FileAttachment
    {
        static void Main(string[] args)
        {
            /*
             * See the configuration example for how a typical application
             * might obtain a configuration.
             */
            var configuration = ExampleConfiguration.Obtain(args, allowCloud: false);

            /*
             * Create the TestDataManager for communicating with the server.
             */
            var testDataManager = new TestDataManager(configuration);

            // Initialize a ResultData object.
            var resultData = new ResultData()
            {
                Operator = "John Smith",
                ProgramName = "File Attachment Test",
                Status = new Status(StatusType.Running),
                SerialNumber = Guid.NewGuid().ToString(),
                PartNumber = "NI-ABC-234-FILE",
                FileIds = new List<string>()
            };
            // Create the test result on the SystemLink server.
            var testResult = testDataManager.CreateResult(resultData);

            // Upload some sample data as the file contents.
            var fileId = UploadFileUsingStream(
                configuration,
                "stream.txt",
                Encoding.UTF8.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZ"));

            // Add the file ID to the test result's list of attached files.
            resultData.FileIds.Add(fileId);

            // Set the test result status to done.
            resultData.Status = new Status(StatusType.Done);

            // Update the test result on the SystemLink server.
            testResult.Update(resultData);
        }

        /// <summary>
        /// Uploads a file to the SystemLink server using an in-memory stream.
        /// This allows the file to be created on the server without writing it
        /// to disk first.
        /// </summary>
        /// <param name="configuration">The HTTP configuration to connect to the SystemLink server.</param>
        /// <param name="fileName">The name for the uploaded file.</param>
        /// <param name="fileContents">An array of bytes to send for the contents of the file.</param>
        /// <returns>The ID of the file that was uploaded.</returns>
        public static string UploadFileData(IHttpConfiguration configuration, string fileName, byte[] fileContents)
        {
            // Upload a file using the SystemLink File client
            var fileUploader = new FileUploader(configuration);

            // Use in-memory data for the file upload.
            using (var memoryStream = new MemoryStream())
            {
                // Write the file content as bytes to the stream.
                memoryStream.Write(fileContents, 0, fileContents.Length);

                // Reset the stream position to the beginning to upload from the beginning.
                memoryStream.Position = 0;

                // Upload the file to the SystemLink server and get the ID for the uploaded file.
                var fileId = fileUploader.UploadFile(memoryStream, fileName);
                return fileId;
            }
        }
    }
}
