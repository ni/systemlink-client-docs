using System.IO;
using System.Text;
using NationalInstruments.SystemLink.Clients.File;

namespace NationalInstruments.SystemLink.Clients.Examples.File
{
    /// <summary>
    /// Example for the SystemLink File API that uploads a file to the
    /// SystemLink server using an in-memory stream.
    /// </summary>
    class Stream
    {
        static void Main(string[] args)
        {
            /*
             * See the configuration example for how a typical application
             * might obtain a configuration.
             */
            var configuration = ExampleConfiguration.Obtain(args, false);

            // Use the FileUploader for communicating with the server.
            var fileUploader = new FileUploader(configuration);

            var fileName = "stream.txt";
            var fileContents = Encoding.UTF8.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZ");

            // Use in-memory data for the file upload.
            using (var memoryStream = new MemoryStream(fileContents))
            {
                // Upload the file to the SystemLink server and get the ID of the uploaded file.
                var fileId = fileUploader.UploadFile(memoryStream, fileName);
            }
        }
    }
}
