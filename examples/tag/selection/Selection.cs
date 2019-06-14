using System;
using System.Globalization;
using System.Text;
using NationalInstruments.SystemLink.Clients.Tag;
using NationalInstruments.SystemLink.Clients.Tag.Values;

namespace NationalInstruments.SystemLink.Clients.Examples.Tag
{
    /// <summary>
    /// Example for the SystemLink Tag client API that uses a selection to read
    /// metadata and values from a set of tags.
    /// </summary>
    class Selection
    {
        static void Main(string[] args)
        {
            /*
             * See the configuration example for how a typical application
             * might obtain a configuration.
             */
            var configuration = ExampleConfiguration.Obtain(args);

            /*
             * Create the TagManager for communicating with the server and the
             * tag writer for sending tag values to the server.
             */
            using (var manager = new TagManager(configuration))
            using (var writer = manager.CreateWriter(bufferSize: 10))
            {
                /*
                 * Initially create two tags and write values to them. Using
                 * Update enables creating both tags in a single API call.
                 */
                var doubleTag = new TagData("example.selection.double",
                    DataType.Double);
                var intTag = new TagData("example.selection.int",
                    DataType.Int32);

                // Currently, SystemLink Server does not return tag values in
                // selections for tags that do not collect aggregates.
                doubleTag.CollectAggregates = intTag.CollectAggregates = true;

                Console.WriteLine("Creating example tags...");
                manager.Update(new[] { doubleTag, intTag });

                var doubleWriter = new DoubleTagValueWriter(writer, doubleTag);
                var intWriter = new Int32TagValueWriter(writer, intTag);

                doubleWriter.Write(Math.PI);
                intWriter.Write((int)Math.PI);
                writer.SendBufferedWrites();

                /*
                 * Create a selection containing all tags whose paths begin
                 * with our example prefix.
                 */
                Console.WriteLine("Creating selection...");
                using (var selection =
                    manager.Open(new[] { "example.selection.*" }))
                {
                    /*
                     * Read each tag value. Since we're using a selection, this
                     * only results in a single request to the server.
                     */
                    var doubleReader = selection.Values[doubleTag.Path]
                        as DoubleTagValueReader;
                    var intReader = selection.Values[intTag.Path]
                        as Int32TagValueReader;

                    Console.WriteLine("Double tag value: {0}", doubleReader?.Read());
                    Console.WriteLine("Int tag value: {0}", intReader?.Read());

                    /*
                     * Create two more example tags and edit the metadata for
                     * the original example tags in a single API call.
                     */
                    var boolTag = new TagData("example.selection.bool",
                        DataType.Bool, keywords: new[] { "new" }, properties: null);
                    var stringTag = new TagData("example.selection.string",
                        DataType.String, keywords: new[] { "new" }, properties: null);

                    // Currently, SystemLink Server does not return tag values in
                    // selections for tags that do not collect aggregates.
                    boolTag.CollectAggregates = stringTag.CollectAggregates = true;

                    Console.WriteLine("Creating additional tags...");
                    manager.Update(new[]
                    {
                        new TagDataUpdate(boolTag, TagUpdateFields.All),
                        new TagDataUpdate(stringTag, TagUpdateFields.All),
                        new TagDataUpdate(doubleTag.Path, DataType.Double,
                            keywords: new[] { "edited" }, properties: null),
                        new TagDataUpdate(intTag.Path, DataType.Int32,
                            keywords: new[] { "edited" }, properties: null)
                    });

                    var boolWriter = new BoolTagValueWriter(writer, boolTag);
                    var stringWriter = new StringTagValueWriter(writer, stringTag);

                    doubleWriter.Write(Math.E);
                    intWriter.Write((int)Math.E);
                    boolWriter.Write(true);
                    stringWriter.Write("example");
                    writer.SendBufferedWrites();

                    /*
                     * Selections don't automatically update with changes made
                     * on the server. Iterating over all tags in the selection
                     * will only see the first two tags in their original state.
                     */
                    OutputSelection(selection);

                    /*
                     * Refreshing the selection will update with changes made
                     * on the server. We have the option to refresh only the
                     * metadata, only the values, or both metadata and values.
                     * Refreshing the values will detect new tags on the server
                     * but will not retrieve keywords and properties.
                     */
                    Console.WriteLine("Refreshing selection values...");
                    selection.RefreshValues();
                    OutputSelection(selection);

                    /*
                     * A metadata refresh will get keywords and properties.
                     */
                    Console.WriteLine("Refreshing selection metadata...");
                    selection.RefreshMetadata();
                    OutputSelection(selection);

                    /*
                     * Selections also enable deleting multiple tags from the
                     * server in a single request.
                     */
                    Console.WriteLine("Deleting example tags...");
                    selection.DeleteTagsFromServer();
                }
            }
        }

        static void OutputSelection(ITagSelection selection)
        {
            const string RowFormat = "{0,-24} | {1,-9} | {2,-8} | {3,-16}";
            var header = string.Format(CultureInfo.CurrentCulture, RowFormat,
                "Tag Path", "Data Type", "Keywords", "Value");
            var lineBuilder = new StringBuilder(header.Length);
            lineBuilder.Append('-', header.Length);
            int index = 0;

            while ((index = header.IndexOf('|', index)) >= 0)
            {
                lineBuilder[index] = '+';
                ++index;
            }

            var headerLine = lineBuilder.ToString();
            Console.WriteLine("Tags currently in the selection:");
            Console.WriteLine(headerLine);
            Console.WriteLine(header);
            Console.WriteLine(headerLine);

            foreach (var tag in selection.Metadata.Values)
            {
                var keywords = string.Join(", ", tag.Keywords);
                var value = ReadTagValueAsString(selection, tag.Path);
                Console.WriteLine(RowFormat, tag.Path, tag.DataType,
                    keywords, value);
            }

            Console.WriteLine(headerLine);
        }

        static string ReadTagValueAsString(ITagSelection selection, string path)
        {
            if (!selection.Values.TryGetValue(path, out TagValueReader reader))
            {
                return null;
            }

            switch (reader.DataType)
            {
            case DataType.Bool:
                return ((BoolTagValueReader)reader).Read()?.ToString(CultureInfo.CurrentCulture);
            case DataType.Double:
                return ((DoubleTagValueReader)reader).Read()?.ToString(CultureInfo.CurrentCulture);
            case DataType.Int32:
                return ((Int32TagValueReader)reader).Read()?.ToString(CultureInfo.CurrentCulture);
            case DataType.String:
                return ((StringTagValueReader)reader).Read();
            default:
                return "Unexpected data type";
            }
        }
    }
}
