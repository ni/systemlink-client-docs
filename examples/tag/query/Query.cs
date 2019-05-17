using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using NationalInstruments.SystemLink.Clients.Tag;

namespace NationalInstruments.SystemLink.Clients.Examples.Tag
{
    /// <summary>
    /// Example for the SystemLink Tag client API that creates 100 tags and
    /// queries by property value to display each tag's metadata in a table.
    /// </summary>
    class Query
    {
        /// <summary>
        /// The number of tags to create for the duration of the example.
        /// </summary>
        private const int NumTags = 100;

        /// <summary>
        /// Format string for a row in the table.
        /// </summary>
        private const string RowFormat = "{0,-17} | {1,-9} | {2,-8} | {3,-25}";

        static void Main(string[] args)
        {
            /*
             * See the configuration example for how a typical application
             * might obtain a configuration.
             */
            var configuration = ExampleConfiguration.Obtain(args);

            /*
             * Create the TagManager for communicating with the server.
             */
            using (var manager = new TagManager(configuration))
            {
                Console.WriteLine("Creating example tags...");
                var createdTags = CreateTags(manager);

                Console.WriteLine();
                WriteHeader();
                Console.WriteLine("Retrieving tags...");

                /*
                 * Each created tag has an example property. We can query for
                 * all tags with that property to retrieve only our tags.
                 * Using a take value of five means each "page" of results will
                 * only contain at most five tags before returning to the
                 * server to retrieve more.
                 */
                var queryProperties = new Dictionary<string, string>
                {
                    ["example"] = "query"
                };
                var query = manager.Query(null, null, queryProperties, 0, 5);
                int tagsRead = 0;

                /*
                 * Each iteration over the query result returns a single page
                 * of results. Initially, only the first page is populated and
                 * each iteration after the first results in a new request to
                 * the server to populate the next page.
                 */
                foreach (var page in query)
                {
                    // Overwrite the "Retrieving tags" indicator.
                    Console.SetCursorPosition(0, Console.CursorTop - 1);

                    if (tagsRead > 0 && tagsRead % 20 == 0)
                    {
                        WriteHeader();
                    }

                    /*
                     * Iterating over a page of query results gives the metadata
                     * for each tag in the page. We output a row for each one.
                     */
                    foreach (var tag in page)
                    {
                        ++tagsRead;
                        var keywords = string.Join(", ", tag.Keywords);
                        var properties = string.Join(", ",
                            tag.Properties.Select(p => p.Key + "=" + p.Value));
                        Console.WriteLine(RowFormat,
                            tag.Path, tag.DataType, keywords, properties);
                    }

                    Console.WriteLine();

                    /*
                     * The TotalCount property indicates how many tags matched
                     * the query, so we can determine if there are more pages.
                     */
                    if (tagsRead >= query.TotalCount)
                    {
                        // There are no more pages; the foreach should exit.
                        Console.WriteLine("{0}/{1} tags", tagsRead, query.TotalCount);
                    }
                    else if (!PromptForNextPage(tagsRead, query.TotalCount))
                    {
                        // The user canceled early.
                        break;
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Deleting example tags...");
                manager.Delete(createdTags);
            }
        }

        /// <summary>
        /// Creates <see cref="NumTags"/> tags on the server of various types.
        /// Each tag will have an example keyword and property.
        /// </summary>
        /// <param name="manager">The tag manager to use to create the tags.</param>
        /// <returns>The <see cref="TagData"/> used to create the tags.</returns>
        static TagData[] CreateTags(ITagManager manager)
        {
            var tags = new TagData[NumTags];
            var keywords = new[] { "example" };
            var properties = new Dictionary<string, string>
            {
                ["example"] = "query"
            };
            var dataTypes = new[]
            {
                DataType.Bool, DataType.DateTime, DataType.Double,
                DataType.Int32, DataType.String, DataType.UInt64
            };

            for (int x = 1; x <= NumTags; x++)
            {
                properties["number"] = x.ToString(CultureInfo.InvariantCulture);
                tags[x - 1] = new TagData($"example.query.{x:D3}",
                    dataTypes[x % dataTypes.Length], keywords, properties);
            }

            /*
             * Create all of the tags in one request to the server. Fails if any
             * of the tags already exist with a different data type.
             */
            manager.Update(tags);
            return tags;
        }

        /// <summary>
        /// Prompts the user to proceed to the next page or quit.
        /// </summary>
        /// <param name="tagsRead">The number of tags read so far.</param>
        /// <param name="totalCount">The number of tags matched by the query.</param>
        /// <returns>True to continue to the next page or false to quit.</returns>
        static bool PromptForNextPage(int tagsRead, int totalCount)
        {
            var prompt = $"{tagsRead}/{totalCount} tags - Press any key to load the next page or Esc to quit";
            Console.Write(prompt);

            if (Console.ReadKey(true).Key == ConsoleKey.Escape)
            {
                Console.WriteLine();
                return false;
            }

            // Overwrite the prompt.
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.WriteLine("Retrieving tags...");
            Console.Write(new string(' ', prompt.Length));
            return true;
        }

        /// <summary>
        /// Writes the table header to the console.
        /// </summary>
        static void WriteHeader()
        {
            var header = string.Format(CultureInfo.CurrentCulture, RowFormat,
                "Tag Path", "Data Type", "Keywords", "Properties");
            var lineBuilder = new StringBuilder(header.Length);
            lineBuilder.Append('-', header.Length);
            int index = 0;

            while ((index = header.IndexOf('|', index)) >= 0)
            {
                lineBuilder[index] = '+';
                ++index;
            }

            var headerLine = lineBuilder.ToString();
            Console.WriteLine(headerLine);
            Console.WriteLine(header);
            Console.WriteLine(headerLine);
        }
    }
}
